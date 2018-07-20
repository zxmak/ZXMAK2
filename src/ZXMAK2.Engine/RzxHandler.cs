using ZXMAK2.Engine.Cpu;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


﻿﻿namespace ZXMAK2.Engine
  {
      public sealed class RzxHandler : IRzxState
      {
          private CpuUnit m_cpu;

          public bool IsPlayback;
          public bool IsRecording;

          private RzxFrame[] m_frameArray;
          private int m_playFrame;
          private int m_playIndex;
          private IRzxFrameSource m_frameSource;

          public RzxHandler(CpuUnit cpu)
          {
              m_cpu = cpu;
              IsPlayback = false;
              IsRecording = false;
          }

          public void Play(IRzxFrameSource frameSource)
          {
              m_frameSource = frameSource;
              IsPlayback = false; // avoid reenter for CheckInt
              m_frameArray = frameSource.GetNextFrameArray();
              m_playFrame = 0;
              m_playIndex = 0;
              m_cpu.RzxCounter = 0;
              IsPlayback = m_frameArray != null && m_frameArray.Length > 0;
              IsRecording = false;
          }

          public byte GetInput()
          {
              var frame = m_frameArray[m_playFrame];
              if (m_playIndex < frame.InputData.Length)
              {
                  //LogAgent.Info("RZX: get {0}:{1}=#{2:X2}  RZX={3} PC=#{4:X4}", m_playFrame, m_playIndex, frame.IOData[m_playIndex], m_cpu.RzxCounter, m_cpu.regs.PC);
                  return frame.InputData[m_playIndex++];
              }
              Logger.Error(
                  "RZX: frame={0}/{1}  fetch={2}/{3}  input={4}/{5}  PC=#{6:X4} - unexpected end of input",
                  Frame,
                  FrameCount,
                  Fetch,
                  FetchCount,
                  Input,
                  InputCount,
                  m_cpu.regs.PC);
              Locator.Resolve<IUserMessage>().Error(
                  "RZX playback stopped!\nReason: unexpected end of input - synchronization lost!\n\nFrame:\t{0}/{1}\nFetch:\t{2}/{3}\nInput:\t{4}/{5}\nPC:\t#{6:X4}", 
                  Frame, 
                  FrameCount, 
                  Fetch, 
                  FetchCount, 
                  Input, 
                  InputCount, 
                  m_cpu.regs.PC);
              IsPlayback = false;
              return m_cpu.BUS;
          }

          public void SetInput(byte value)
          {
          }

          public void Reset()
          {
              IsPlayback = false;
              IsRecording = false;
          }

          public bool CheckInt(int frameTact)
          {
              if (!IsPlayback)
                  return false;
              var frame = m_frameArray[m_playFrame];
              if (m_cpu.RzxCounter < frame.FetchCount)
              {
                  return false;
              }
              if (m_cpu.FX != CpuModeIndex.None || m_cpu.XFX != CpuModeEx.None)
              {
                  return true;
              }
              //LogAgent.Info("RZX: ---- int  RZX={0} PC=#{1:X4} ----", m_cpu.RzxCounter, m_cpu.regs.PC, m_busMgr.GetFrameTact());
              if (m_playIndex != frame.InputData.Length)
              {
                  Logger.Error(
                      "RZX: frame={0}/{1}  fetch={2}/{3}  input={4}/{5}  PC=#{6:X4} - unexpected frame",
                      Frame,
                      FrameCount,
                      Fetch,
                      FetchCount,
                      Input,
                      InputCount,
                      m_cpu.regs.PC);
                  Locator.Resolve<IUserMessage>().Error(
                      "RZX playback stopped!\nReason: unexpected frame - synchronization lost!\n\nFrame:\t{0}/{1}\nFetch:\t{2}/{3}\nInput:\t{4}/{5}\nPC:\t#{6:X4}", 
                      Frame, 
                      FrameCount, 
                      Fetch, 
                      FetchCount, 
                      Input, 
                      InputCount, 
                      m_cpu.regs.PC);
                  IsPlayback = false;
                  return false;
              }
              m_playIndex = 0;
              m_cpu.RzxCounter -= frame.FetchCount;
              if (++m_playFrame >= m_frameArray.Length)
              {
                  Play(m_frameSource);
                  if (!IsPlayback)
                  {
                      Locator.Resolve<IUserMessage>()
                          .Info("RZX playback end");
                  }
              }
              return true;
          }

          #region IRzxState Members

          bool IRzxState.IsPlayback { get { return IsPlayback; } }
          bool IRzxState.IsRecording { get { return IsRecording; } }
          public int Frame { get { return IsPlayback ? m_playFrame : 0; } }
          public int Fetch { get { return IsPlayback ? m_cpu.RzxCounter : 0; } }
          public int Input { get { return IsPlayback ? m_playIndex : 0; } }
          public int FrameCount { get { return IsPlayback ? m_frameArray.Length : 0; } }
          public int FetchCount { get { return IsPlayback ? m_frameArray[m_playFrame].FetchCount : 0; } }
          public int InputCount { get { return IsPlayback ? m_frameArray[m_playFrame].InputData.Length : 0; } }

          #endregion
      }
  }
