/*
 * Copyright 2011 Alexander Tsidaev
 * 
 * This file is part of z80gdbserver.
 * z80gdbserver is free software: you can redistribute it and/or modify it under the
 * terms of the GNU General Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 * 
 * z80gdbserver is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with z80gdbserver. 
 * If not, see http://www.gnu.org/licenses/.
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Hardware.GdbServer.Gdb
{
    public class GDBNetworkServer : IDisposable
    {
        private readonly ASCIIEncoding _encoder = new ASCIIEncoding();

        private readonly IDebuggable _emulator;
        private readonly GDBJtagDevice _jtagDevice;

        private readonly TcpListener _listener;
        private readonly Thread _socketListener;
        private readonly List<TcpClient> _clients = new List<TcpClient>();

        public GDBNetworkServer(IDebuggable emulator, GDBJtagDevice jtagDevice)
        {
            _emulator = emulator;
            _jtagDevice = jtagDevice;

            _listener = new TcpListener(IPAddress.Any, jtagDevice.Port);
            _listener.Start();

            _socketListener = new Thread(ListeningThread);
            _socketListener.Start();
        }

        public void Breakpoint(Breakpoint breakpoint)
        {
            // emulator.IsRunning= false;

            // We do not need old breakpoints because GDB will set them again
            _emulator.ClearBreakpoints();
            _jtagDevice.ClearBreakpoints();

            SendGlobal(GDBSession.FormatResponse(GDBSession.StandartAnswers.Breakpoint));
        }

        private void SendGlobal(string message)
        {
            foreach (var client in _clients.Where(c => c.Connected))
            {
                var stream = client.GetStream();
                if (stream != null)
                    SendResponse(stream, message);
            }
        }

        private void ListeningThread(object obj)
        {
            try
            {
                while (true)
                {
                    TcpClient client = _listener.AcceptTcpClient();

                    _clients.Add(client);
                    _clients.RemoveAll(c => !c.Connected);

                    Thread clientThread = new Thread(GDBClientConnected);
                    clientThread.Start(client);
                }
            }
            catch
            {
                // Here can be an exception because we interrupting blocking AcceptTcpClient()
                // call on Dispose. We should not fail here, so try/catching
            }
        }

        private void GDBClientConnected(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            GDBSession session = new GDBSession(_emulator, _jtagDevice);

            byte[] message = new byte[0x1000];
            int bytesRead;

            // log = new StreamWriter("c:\\temp\\log.txt");
            // log.AutoFlush = true;

            _emulator.DoStop();

            while (true)
            {
                bytesRead = 0;

                try
                {
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch (IOException iex)
                {
                    var sex = iex.InnerException as SocketException;
                    if (sex == null || sex.ErrorCode != 10004)
                    {
                        Logger.Error(sex);
                    }
                    break;
                }
                catch (SocketException sex)
                {
                    if (sex.ErrorCode != 10004)
                    {
                        Logger.Error(sex);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                if (bytesRead > 0)
                {
                    GDBPacket packet = new GDBPacket(message, bytesRead);
                    if (_jtagDevice.Log)
                    {
                        Logger.Info("--> {0}", packet);
                    }

                    bool isSignal;
                    string response = session.ParseRequest(packet, out isSignal);
                    if (response != null)
                    {
                        if (isSignal)
                            SendGlobal(response);
                        else
                            SendResponse(clientStream, response);
                    }
                }
            }
            tcpClient.Close();
        }

        private void SendResponse(Stream stream, string response)
        {
            if (_jtagDevice.Log)
            {
                Logger.Info("<-- {0}", response);
            }

            byte[] bytes = _encoder.GetBytes(response);
            stream.Write(bytes, 0, bytes.Length);
        }

        public void Dispose()
        {
            if (_socketListener != null)
            {
                _listener.Stop();

                foreach (var client in _clients)
                    if (client.Connected)
                        client.Close();

                _socketListener.Abort();
            }
        }
    }
}

