using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace MyMovies.UI
{
    public class Log
    {
        private String _cat;
        public Log(String cat)
        {
            _cat = cat;
        }

        public void Info(String message, params String[] args)
        {
            Write(_cat, Level.Info, message, args);
        }

        public void Warn(String message, params String[] args)
        {
            Write(_cat, Level.Warn, message, args);
        }

        public enum Level
        {
            Debug,
            Info,
            Warn,
            Error
        };

        private static Action<String, String, Level> _listner;
        public static Action<String, String, Level> Listner
        {
            set
            {
                _listner = value;
                _listnerDispatcher = value == null ? null : Dispatcher.CurrentDispatcher;
            }
        }

        private static Dispatcher _listnerDispatcher;
        private static Object _lock = new Object();

        private static void Write(String cat, Level level, String message, params String[] args)
        {
            message = String.Format(message ?? "", args).Replace("\t", " ");
            lock (_lock)
            {
                using (var w = File.AppendText(DM.GetLocalFilePath("log.txt")))
                {
                    w.WriteLine(new[]{
                        DateTime.Now.ToString("D"),
                        cat,
                        level.ToString(),
                        message,
                    }.Join("\t"));
                } 
            }

            if(_listnerDispatcher != null)
            {
                _listnerDispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback(delegate{
                    if (_listner != null)
                        _listner(cat, message, level);
                    return null;
                }), null);
            }
        }

    }
}
