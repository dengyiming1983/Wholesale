using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Runtime.CompilerServices;
using System.Web;

/// <summary>
/// 日志类
/// </summary>
public sealed class Logger
{
    #region Member Variables

    /// <summary>
    /// 用于Trace的组织输出的类别名称
    /// </summary>
    //private const string trace_sql = "\r\n***********************TRACE_SQL {0}*****************************\r\nTRACE_SQL";
    private const string trace_sql = "\r\n[{0}] General ";


    /// <summary>
    /// 用于Trace的组织输出的类别名称
    /// </summary>
    //private const string trace_exception = "\r\n***********************TRACE_EXCEPTION {0}***********************";
    private const string trace_exception = "\r\n[{0}] EXCEPTION ";

    private const string trace_info = "\r\n[{0}] Info: ";
    private const string trace_debug = "\r\n[{0}] Debug: ";
    private const string trace_warn = "\r\n[{0}] Warn: ";
    private const string trace_error = "\r\n[{0}] Error: ";

    private const int INFO = 0;
    private const int DEBUG = 1;
    private const int WARN = 2;
    private const int ERROR = 3;

    private static string lastLogMsg = "";

    /// <summary>
    /// 当前日志的日期
    /// </summary>
    private static DateTime CurrentLogFileDate = DateTime.Now;

    /// <summary>
    /// 日志对象
    /// </summary>
    private static TextWriterTraceListener twtl;

    /// <summary>
    /// 日志根目录
    /// </summary>
    private const string log_root_directory = @"\log";

    /// <summary>
    /// 日志子目录
    /// </summary>
    private static string log_subdir;


    /// <summary>
    /// "      {0} = {1}"
    /// </summary>
    private const string FORMAT_TRACE_PARAM = "      {0} = {1}";

    /// <summary>
    /// 1   仅控制台输出
    /// 2   仅日志输出
    /// 3   控制台+日志输出
    /// </summary>
    private static readonly int flag = 3;         //可以修改成从配置文件读取

    public static void info(string tag, string msg)
    {
        new AsyncLogMessage(BeginTraceMessage).BeginInvoke(Logger.INFO, tag, msg, null, null);
    }

    public static void debug(string tag, string msg)
    {
        new AsyncLogMessage(BeginTraceMessage).BeginInvoke(Logger.DEBUG, tag, msg, null, null);
    }

    public static void warn(string tag, string msg)
    {
        new AsyncLogMessage(BeginTraceMessage).BeginInvoke(Logger.WARN, tag, msg, null, null);
    }

    public static void error(string tag, string msg)
    {
        new AsyncLogMessage(BeginTraceMessage).BeginInvoke(Logger.ERROR, tag, msg, null, null);
    }


    #endregion

    #region Constructor

    static Logger()
    {
        System.Diagnostics.Trace.AutoFlush = true;

        switch (flag)
        {
            case 1:
                System.Diagnostics.Trace.Listeners.Add(new ConsoleTraceListener());
                break;
            case 2:
                System.Diagnostics.Trace.Listeners.Add(TWTL);
                break;
            case 3:
                System.Diagnostics.Trace.Listeners.Add(new ConsoleTraceListener());
                System.Diagnostics.Trace.Listeners.Add(TWTL);
                break;
        }
    }

    #endregion

    #region Method

    #region trace

    /// <summary>
    /// 异步错误日志
    /// </summary>
    /// <param name="value"></param>
    public static void Trace(Exception ex)
    {
        new AsyncLogException(BeginTraceError).BeginInvoke(ex, null, null);
    }

    #endregion

    #region delegate

    private delegate void AsyncLogMessage(int type,string tag,string msg);
    private delegate void AsyncLogException(Exception ex);

    private static void BeginTraceError(Exception ex)
    {
        if (null != ex)
        {
            //检测日志日期
            StrategyLog();

            //输出日志头
            System.Diagnostics.Trace.WriteLine(string.Format(trace_exception, DateTime.Now));
            while (null != ex)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("{0} {1}\r\n{2}\r\nSource:{3}", ex.GetType().Name, ex.Message, ex.StackTrace, ex.Source));
                ex = ex.InnerException;
            }
        }
    }

    private static void BeginTraceMessage(int type, string tag,string msg)
    {
        if (!string.IsNullOrEmpty(msg))
        {

            if (msg == lastLogMsg)
                return;

            //检测日志日期
            StrategyLog();

            //输出日志头
            string heard = "";
            switch (type)
            {
                case Logger.INFO:
                    heard = trace_info;
                    break;
                case Logger.DEBUG:
                    heard = trace_debug;
                    break;
                case Logger.WARN:
                    heard = trace_warn;
                    break;
                case Logger.ERROR:
                    heard = trace_error;
                    break;
                default:
                    break;
            }
           
            System.Diagnostics.Trace.WriteLine(string.Format(heard + " => {1}", DateTime.Now, tag));
            
            System.Diagnostics.Trace.WriteLine(msg);

            lastLogMsg = msg;
        }
    }

    #endregion

    #region helper

    /// <summary>
    /// 根据日志策略生成日志
    /// </summary>
    private static void StrategyLog()
    {
        //判断日志日期
        if (DateTime.Compare(DateTime.Now.Date, CurrentLogFileDate.Date) != 0)
        {
            DateTime currentDate = DateTime.Now.Date;

            //生成子目录
            //BuiderDir(currentDate);

            //更新当前日志日期
            CurrentLogFileDate = currentDate;

            System.Diagnostics.Trace.Flush();

            //更改输出
            if (twtl != null)
                System.Diagnostics.Trace.Listeners.Remove(twtl);

            System.Diagnostics.Trace.Listeners.Add(TWTL);
        }
    }

    /// <summary>
    /// 根据年月生成子目录
    /// </summary>
    /// <param name="currentDate"></param>
    private static void BuiderDir(DateTime currentDate)
    {
        int year = currentDate.Year;
        int month = currentDate.Month;
        //年/月
        string subdir = string.Concat(year, '\\', month);
        string path = Path.Combine(Directory.GetCurrentDirectory() + log_root_directory, subdir);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        log_subdir = subdir;
    }

    #endregion

    #endregion

    #region Properties

    /// <summary>
    /// 日志文件路径
    /// </summary>
    /// <returns></returns>
    private static string GetLogFullPath
    {
        get
        {
            //return string.Concat(Directory.GetCurrentDirectory() + log_root_directory, '\\', string.Concat(log_subdir, @"\log", CurrentLogFileDate.ToString("yyyyMMdd"), ".txt"));
            return string.Concat(Directory.GetCurrentDirectory() + log_root_directory, '\\', "err", ".log");

        }
    }

    /// <summary>
    /// 跟踪输出日志文件
    /// </summary>
    private static TextWriterTraceListener TWTL
    {
        get
        {
            if (twtl == null)
            {
                //if (string.IsNullOrEmpty(log_subdir))
                //    BuiderDir(DateTime.Now);
                //else
                //{
                //    string logPath = GetLogFullPath;

                //    if (!Directory.Exists(Path.GetDirectoryName(logPath)))
                //        BuiderDir(DateTime.Now);
                //}
                // twtl = new TextWriterTraceListener(GetLogFullPath);

             
                if (!Directory.Exists(Directory.GetCurrentDirectory() + log_root_directory))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + log_root_directory);

                }
                twtl = new TextWriterTraceListener(GetLogFullPath);
            }
            return twtl;
        }
    }

    #endregion



    public static void WriteLog(string msg)
    {
       // string path = Directory.GetCurrentDirectory() + @"\log";
        string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\log";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string filename = path + @"\wrds.log";
        if (!File.Exists( filename))
        {
           
            using (FileStream fs = File.Create(filename))
            {
                fs.Dispose();
                fs.Close();
            }
        }

        StreamWriter writer = null;
        try
        {
            writer = File.AppendText(filename);
            writer.WriteLine("{0}{1}", msg, DateTime.Now.ToString(" @ MMM dd,yyyy hh:mm:ss"));
            writer.Flush();
        }
        catch (Exception ex)
        {

            //throw ex;
        }
        finally
        {
            if (writer !=null)
            {
                writer.Close();
            }
        }
    }

}
