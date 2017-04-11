using log4net;
using log4net.Appender;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Song_Public
{
    /// <summary>
    /// 对net4log封装 调用方式 Log4netProvider.Logger.Debug(message);  
    /// Log4netProvider.Logger.Error(message);  
    /// </summary>
    public class Log4netProvider
    {
        /// <summary>
        /// 重置配置
        /// </summary>
        /// <param name="logconfig"></param>
        public static void ReplaceFileTag(string logconfig)
        {
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(logconfig, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);
                string str = sr.ReadToEnd();
                sr.Close();
                fs.Close();

                if (str.IndexOf("#LOG_PATH#") > -1)
                {
                    str = str.Replace(@"#LOG_PATH#", System.Environment.GetEnvironmentVariable("TEMP") + @"\");
                    System.IO.FileStream fs1 = new System.IO.FileStream(logconfig, System.IO.FileMode.Open, System.IO.FileAccess.Write);
                    StreamWriter swWriter = new StreamWriter(fs1, System.Text.Encoding.UTF8);
                    swWriter.Flush();
                    swWriter.Write(str);
                    swWriter.Close();
                    fs1.Close();
                }
            }
            catch (System.Exception ex)
            {

            }
        }

        /// <summary> 初始化日志 </summary>  
        public static void Init(string repository, string name)
        {

            string logconfig = @"log4net.config";

            ReplaceFileTag(logconfig);

            Stopwatch st = new Stopwatch();

            st.Start();

            log4net.GlobalContext.Properties["dynamicName"] = name;
            Logger = LogManager.GetLogger(name);

            st.Stop();

            if (st.ElapsedMilliseconds > 2000)
            {
                Logger.Info("log4net.config file ERROR!!!");

                string str;
                using (FileStream fs = new System.IO.FileStream(logconfig, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8))
                    {
                        str = sr.ReadToEnd();
                        str = str.Replace(@"ref=""SQLAppender""", @"ref=""SQLAppenderError""");
                    }
                }

                using (FileStream fs1 = new System.IO.FileStream(logconfig, System.IO.FileMode.Open, System.IO.FileAccess.Write))
                {
                    using (StreamWriter swWriter = new StreamWriter(fs1, System.Text.Encoding.UTF8))
                    {
                        swWriter.Flush();
                        swWriter.Write(str);
                    }
                }
            }

            InitLogPath(repository);
        }

        // Todo ：定义日志文件路径格式   
        static void InitLogPath(string repository)
        {
            RollingFileAppender appender = new RollingFileAppender();

            appender.File = repository + "\\Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\";

            appender.AppendToFile = true;

            appender.MaxSizeRollBackups = -1;

            //appender.MaximumFileSize = "1MB";    

            appender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;

            appender.DatePattern = "yyyy-MM-dd_HH\".log\"";

            appender.StaticLogFileName = false;

            appender.LockingModel = new log4net.Appender.FileAppender.MinimalLock();

            appender.Layout = new log4net.Layout.PatternLayout("%date [%thread] %-5level - %message%newline");

            appender.ActivateOptions();

            log4net.Config.BasicConfigurator.Configure(appender);

        }

        /// <summary> Log4net日志内核 </summary>  
        public static ILog Logger
        {
            get
            {
                // Todo ：没有初始化初始化   
                if (_log == null)
                {
                    Log4netProvider.Init(AppDomain.CurrentDomain.BaseDirectory, Process.GetCurrentProcess().ProcessName);
                }

                return _log;
            }
            set
            {
                _log = value;
            }
        }

        static ILog _log = null;

        /// <summary> 事件传送信息打印 </summary>  
        public static void EventsMsg(object sender, object e, System.Diagnostics.StackFrame SourceFile)
        {
            string msg = string.Format("[FILE:{0} ],LINE:{1},{2}] sender:{3},e:{4}", SourceFile.GetFileName(), SourceFile.GetFileLineNumber(), SourceFile.GetMethod(), sender.GetType(), e.GetType());

            if (Logger != null)
            {
                Logger.Info(msg);
            }
        }

        // Todo ：信息格式   
        static string GetFileMsg(System.Diagnostics.StackFrame SourceFile)
        {
            return string.Format("FILE: [{0}] LINE:[{1}] Method:[{2}]", SourceFile.GetFileName(), SourceFile.GetFileLineNumber(), SourceFile.GetMethod());
        }

        /// <summary> 记录错误日志 </summary>  
        private void Error(System.Diagnostics.StackFrame SourceFile, Exception ex)
        {
            if (ex == null) return;

            if (Logger == null) return;

            Logger.Info(GetFileMsg(SourceFile));

            Logger.Error(ex.Message);

            if (ex.InnerException == null) return;

            Logger.Fatal(ex.InnerException);
        }

        /// <summary> 过程日志 </summary>  
        private void Info(System.Diagnostics.StackFrame SourceFile, string infomsg)
        {
            if (infomsg == null) return;

            if (Logger == null) return;

            Logger.Info(GetFileMsg(SourceFile));

            Logger.Info(infomsg);
        }

    }
}
