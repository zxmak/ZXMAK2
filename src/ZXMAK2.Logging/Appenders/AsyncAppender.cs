using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using log4net.Core;
using log4net.Appender;
using System.Diagnostics;


namespace ZXMAK2.Logging.Appenders
{
    public sealed class AsyncAppender : ForwardingAppender, IDisposable
    {
        private readonly ConcurrentQueue<LoggingEvent> _queue = new ConcurrentQueue<LoggingEvent>();
        private readonly AutoResetEvent _flushEvent = new AutoResetEvent(false);
        private Thread _thread;
        private bool _isStarted;
        private bool _isStopping;


        public AsyncAppender()
        {
            Fix = FixFlags.Message | FixFlags.ThreadName | FixFlags.Exception;
        }

        public void Dispose()
        {
            Stop();
            _flushEvent.Dispose();
        }



        public int GrowSize { get; set; }
        public FixFlags Fix { get; set; }


        #region IOptionHandler

        public override void ActivateOptions()
        {
            Stop();
            base.ActivateOptions();
            Start();
        }

        #endregion IOptionHandler

        
        #region ForwardingAppender Overrides

        protected override void OnClose()
        {
            Stop();
            base.OnClose();
            _flushEvent.Dispose();
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (loggingEvent != null)
            {
                loggingEvent.Fix = Fix;
                DebugAppend(loggingEvent);
            }
            OnAddEvent(loggingEvent, false);
        }

        [Conditional("DEBUG")]
        private void DebugAppend(LoggingEvent loggingEvent)
        {
            Debug.WriteLine(loggingEvent.RenderedMessage);
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            if (loggingEvents == null)
            {
                return;
            }
            foreach (var logEvent in loggingEvents)
            {
                Append(logEvent);
            }
        }

        #endregion ForwardingAppender Overrides


        #region Private

        private void Start()
        {
            if (_isStarted)
            {
                throw new InvalidOperationException("Asynchronous flushing is already started!");
            }
            _isStopping = false;
            _thread = new Thread(ThreadProc)
            {
                Name = Name,
                IsBackground = true,
                Priority = ThreadPriority.Lowest,
            };
            _thread.Start();
            _isStarted = true;
        }

        private void Stop()
        {
            if (!_isStarted)
            {
                return;
            }

            _isStopping = true;
            OnAddEvent(null, true);
            _flushEvent.Set();
            _thread.Join();

            _thread = null;
            _isStarted = false;
            _isStopping = false;
        }

        private void ThreadProc()
        {
            while (true)
            {
                try
                {
                    var isStop = false;
                    var logEvents = GetEvents();
                    if (logEvents != null)
                    {
                        foreach (var loggingEvent in logEvents)
                        {
                            if (loggingEvent == null)
                            {
                                // wait for null (stop event)
                                isStop = true;
                            }
                            else
                            {
                                base.Append(loggingEvent);
                            }
                        }
                        OnRelease();
                    }
                    if (_isStopping && isStop)
                    {
                        return;
                    }
                    if (!_isStopping)
                    {
                        _flushEvent.WaitOne();
                    }
                }
                catch (Exception ex)
                {
                    OnErrror("AsyncAppender.FlushThreadProc failed", ex);
                    return;
                }
            }
        }

        private IEnumerable<LoggingEvent> GetEvents()
        {
            var buffer = new List<LoggingEvent>();
            var record = default(LoggingEvent);
            while (_queue.TryDequeue(out record))
            {
                buffer.Add(record);
            }
            return buffer;
        }

        private void OnRelease()
        {
        }

        private void OnAddEvent(LoggingEvent loggingEvent, bool force)
        {
            if (!force && GrowSize > 0 && _queue.Count > GrowSize)
            {
                // TODO: implement overflow notification
                return;
            }
            _queue.Enqueue(loggingEvent);
            _flushEvent.Set();
        }

        private void OnErrror(string message, Exception ex)
        {
            var loggingEvent = new LoggingEvent(
                GetType(), 
                null, 
                Name, 
                Level.Error, 
                message, 
                ex);
            loggingEvent.Fix = Fix;
            OnAddEvent(loggingEvent, true);
        }

        #endregion Private
    }
}
