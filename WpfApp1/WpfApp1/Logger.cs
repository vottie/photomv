using System;
using System.IO;
using System.Text;

namespace Photomv
{
    /*
     * 
     * Logger
     * original source from https://qiita.com/miku_minatsuki/items/e4aa142ee7e2ba371169
     * 
     */
    public class Logger
    {
        private static readonly string LOG_FORMAT = "{0} {1} {2}";
        private static readonly string DATETIME_FORMAT = "yyyy/MM/dd HH:mm:ss.fff";
        private StreamWriter stream = null;
        private readonly bool consoleOut;

        private static Logger singleton = null;

        public static Logger GetInstance(string logFilePath, bool consoleOut = false)
        {
            if(singleton == null)
            {
                singleton = new Logger(logFilePath, consoleOut);
            }
            return singleton;
        }

        public static void Init(string logFilePath, bool consoleOut = false)
        {
            singleton = new Logger(logFilePath, consoleOut);
        }

        private Logger(string logFilePath, bool consoleOut)
        {
            if (string.IsNullOrWhiteSpace(logFilePath))
            {
                throw new Exception("logFilePath is empty.");
            }

            var logFile = new FileInfo(logFilePath);
            if (!Directory.Exists(logFile.DirectoryName))
            {
                Directory.CreateDirectory(logFile.DirectoryName);
            }

            stream = new StreamWriter(logFile.FullName, true, Encoding.Default);
            stream.AutoFlush = true;
            this.consoleOut = consoleOut;
        }


        private void Write(Level level, string text)
        {
            char l = level.ToString()[0];
            string lvl = "|" + l + "|";
            string log = string.Format(LOG_FORMAT, DateTime.Now.ToString(DATETIME_FORMAT), lvl, text);
            stream.WriteLine(log);
            if (consoleOut)
            {
                Console.WriteLine(log);
            }
        }

        public void Error(string text)
        {
            Write(Level.ERROR, text);
        }

        public void Error(Exception ex)
        {
            Write(Level.ERROR, ex.Message + Environment.NewLine + ex.StackTrace);
        }

        public void Error(string format, object arg)
        {
            Error(string.Format(format, arg));
        }

        public void Error(string format, params object[] args)
        {
            Error(string.Format(format, args));
        }


        public void Trace(string text)
        {
            Write(Level.TRACE, text);
        }

        public void Trace(string format, object arg)
        {
            Trace(string.Format(format, arg));
        }

        public void Trace(string format, params object[] args)
        {
            Trace(string.Format(format, args));
        }

        public void Info(string text)
        {
            Write(Level.INFO, text);
        }

        public void Info(string format, object arg)
        {
            Info(string.Format(format, arg));
        }

        public void Info(string format, params object[] args)
        {
            Info(string.Format(format, args));
        }

        private enum Level
        {
            ERROR,
            WARN,
            INFO,
            DEBUG,
            TRACE
        }
    }
}
