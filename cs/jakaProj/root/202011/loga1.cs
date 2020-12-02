using System;
using System.IO;
using System.Collections.Generic;
using static System.Console;
using System.Text;
using System.Diagnostics;

namespace LogWriter
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        Error,
        Info,
        Waring,
        Success,
        Failure
    }
    /// <summary>
    /// 日志参数信息
    /// </summary>
    internal struct LogInfo
    {
        internal string FileName { get; set; }
        internal string MethodName { get; set; }
        internal int Line { get; set; }
        internal int Column { get; set; }
        internal string LogType { get; set; }
    }
    /// <summary>
    /// 公共日之类
    /// </summary>
    internal  class LogCommon
    {
        static System.Threading.ReaderWriterLockSlim Slim = new System.Threading.ReaderWriterLockSlim(System.Threading.LockRecursionPolicy.SupportsRecursion);
        /// <summary>
        /// 获取日志参数信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        internal static LogInfo GetLog(LogType type)
        {
            StackTrace st = new StackTrace(2, true);
            StackFrame sf = st.GetFrame(0);
            LogInfo li = new LogInfo()
            {
                FileName = sf.GetFileName(),
                MethodName = sf.GetMethod().Name,
                Line = sf.GetFileLineNumber(),
                Column = sf.GetFileColumnNumber(),
            };
            string logType = "-Error";

            switch (type)
            {
                case LogType.Error:
                    logType = "-Error";
                    break;
                case LogType.Info:
                    logType = "-Info";
                    break;
                case LogType.Waring:
                    logType = "-Waring";
                    break;
                case LogType.Success:
                    logType = "-Success";
                    break;
                case LogType.Failure:
                    logType = "-Failure";
                    break;
                default:
                    logType = "-Error";
                    break;
            }
            li.LogType = logType;
            return li;
        }
        /// <summary>
  /// 写入以小时分割的日志文件
  /// </summary>
  /// <param name="Msg">要记录的消息</param>
  /// <param name="li">日志信息类</param>
  /// <param name="LogPath">日志所在文件夹</param>
  /// <returns></returns>
        internal static string WriteLineToTimeFile(string Msg, LogInfo li,string LogPath)
        {
            if (string.IsNullOrEmpty(Msg))
            {
                return "输入参数Msg为null或者值为空不符合记录要求！";
            }
            StreamWriter sw = null;
            try
            {
                Slim.EnterWriteLock();
               //string Dir = System.Windows.Forms.Application.StartupPath + @"\GLogs\" + DateTime.Now.ToString("yyyy年MM月dd日");
                checkLog(LogPath);
                string file = DateTime.Now.ToString("yyyy年MM月dd日HH时") + ".log";
                checkfile(LogPath, file);
                string fileName = LogPath + "\\" + file;
                sw = File.AppendText(fileName);
                sw.WriteLine("日志时间:" + DateTime.Now.ToString() + ",文件名:" + li.FileName + ",方法名：" + li.MethodName + "行号：" + li.Line + ",列：" + li.Column + ",日志类型:" + li.LogType);
                sw.WriteLine("日志内容:" + Msg);
                return nameof(WriteLineToTimeFile) + "日志记录操作成功！";
            }
            catch (Exception ex)
            {
                return nameof(WriteLineToTimeFile) + "日志记录发生错误:" + ex.Message;
            }
            finally
            {
                sw.Close();
                Slim.ExitWriteLock();
            }
        }
        /// <summary>
        /// 检查日志目录是否存在，不存在则创建
        /// </summary>
        /// <param name="Path">文件夹</param>
        internal static void checkLog(string Path)
        {
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
        }
        /// <summary>
        /// 传入路径名称和文件名称，创建日志文件
        /// </summary>
        /// <param name="DirName">文件夹</param>
        /// <param name="FileName">文件名</param>
       internal static void checkfile(string DirName, string FileName)
        {
            if (!File.Exists(DirName + @"\" + FileName))
            {
                File.Create(DirName + @"\" + FileName).Close();
            }
        }
    }
}