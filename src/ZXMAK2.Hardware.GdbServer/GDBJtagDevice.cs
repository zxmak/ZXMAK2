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
using System.Xml;
using System.Text;
using System.Collections.Generic;

using ZXMAK2.Engine;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Hardware.GdbServer.Gdb;


namespace ZXMAK2.Hardware.GdbServer
{
    public class GDBJtagDevice : BusDeviceBase, IJtagDevice
    {
        private GDBNetworkServer server;
        private IDebuggable emulator;
        private IBusManager busManager;
        private readonly List<Breakpoint> accessBreakpoints = new List<Breakpoint>();
        private int _port;


        public GDBJtagDevice()
        {
            Category = BusDeviceCategory.Debugger;
            Name = "GDB-Z80 SERVER";
            Port = 2000;
            Log = true;
        }


        public int Port
        {
            get { return _port; }
            set
            {
                _port = value;

                var builder = new StringBuilder();
                builder.Append("Interface for interaction with gdb debugger");
                builder.Append(Environment.NewLine);
                builder.Append(Environment.NewLine);
                builder.Append(string.Format("Listening on: {0}:{1}", "localhost", _port));
                builder.Append(Environment.NewLine);
                builder.Append(Environment.NewLine);
                builder.Append("Use gdb-z80 to connect");
                Description = builder.ToString();
            }
        }

        public bool Log { get; set; }

        public void Attach(IDebuggable dbg)
        {
            emulator = dbg;
            emulator.Breakpoint += OnBreakpoint;

            // For memory read/write breakpoints:
            busManager.Events.SubscribeWrMem(0x0000, 0x0000, OnMemoryWrite);
            busManager.Events.SubscribeRdMem(0x0000, 0x0000, OnMemoryRead);


            server = new GDBNetworkServer(emulator, this);
        }

        public void Detach()
        {
            server.Dispose();
        }

        public override void BusConnect()
        {
        }

        public override void BusDisconnect()
        {
        }

        public override void BusInit(IBusManager bmgr)
        {
            this.busManager = bmgr;
        }

        void OnBreakpoint(object sender, EventArgs args)
        {
            server.Breakpoint(new Breakpoint(Breakpoint.BreakpointType.Execution, emulator.CPU.regs.PC));
        }

        void OnMemoryWrite(ushort addr, byte value)
        {
            Breakpoint breakPoint = accessBreakpoints.FirstOrDefault(bp => bp.Address == addr && (bp.Type == Breakpoint.BreakpointType.Write || bp.Type == Breakpoint.BreakpointType.Access));
            if (breakPoint != null)
                server.Breakpoint(breakPoint);
        }

        void OnMemoryRead(ushort addr, ref byte value)
        {
            Breakpoint breakPoint = accessBreakpoints.FirstOrDefault(bp => bp.Address == addr && (bp.Type == Breakpoint.BreakpointType.Read || bp.Type == Breakpoint.BreakpointType.Access));
            if (breakPoint != null)
                server.Breakpoint(breakPoint);
        }

        public void AddBreakpoint(Breakpoint.BreakpointType type, ushort address)
        {
            accessBreakpoints.Add(new Breakpoint(type, address));
        }

        public void RemoveBreakpoint(ushort address)
        {
            accessBreakpoints.RemoveAll(b => b.Address == address);
        }

        public void ClearBreakpoints()
        {
            accessBreakpoints.Clear();
        }

        protected override void OnConfigSave(XmlNode node)
        {
            base.OnConfigSave(node);
            Utils.SetXmlAttribute(node, "port", Port);
            Utils.SetXmlAttribute(node, "log", Log);
        }

        protected override void OnConfigLoad(XmlNode node)
        {
            base.OnConfigLoad(node);
            Port = Utils.GetXmlAttributeAsInt32(node, "port", Port);
            Log = Utils.GetXmlAttributeAsBool(node, "log", Log);
        }
    }
}

