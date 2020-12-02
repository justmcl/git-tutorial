using System;
using System.IO;
using System.Collections.Generic;
using static System.Console;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogWriter
{
   
    /// <summary>
    /// 通过队列缓存来提高日志记录速度
    /// </summary>
   public class GLog
    {
       readonly object lockobj = new object();
        TimerCallback tc;
        Timer timer;
        Queue<LogQueueStruct> LogList = new Queue<LogQueueStruct>();
        public string LogPath { get; set; } = System.Windows.Forms.Application.StartupPath + @"\Logs\" + DateTime.Now.ToString("yyyy年MM月dd日");
        public  void RecordLog(LogType type,string msg)
        {
            LogQueueStruct log = new LogQueueStruct()
            {
                info = LogCommon.GetLog(type),
                Msg=msg
            };
            lock (lockobj)
            {
                LogList.Enqueue(log);
            }
            timer.Change(0, Timeout.Infinite);
        }
        public void RegiestLog()
        {
            //开启一个线程用以将队列中的日志记录到文件
         
                tc = new TimerCallback((o) =>
                {
                   
                  timer.Change(Timeout.Infinite, Timeout.Infinite);
                    lock (lockobj)
                    {
                        if (LogList.Count > 0)
                        {
                            LogQueueStruct log = LogList.Dequeue();
                            LogCommon.WriteLineToTimeFile(log.Msg, log.info,LogPath);
                        }
                    }
                });
                timer = new Timer(tc, null, Timeout.Infinite, Timeout.Infinite);           
        }
        public int LogListCount => LogList.Count;
    }
    internal class LogQueueStruct
    {
        internal LogInfo info { get; set; }
        internal string Msg { get; set; }
    }
}
/*
GLog log = new GLog();
            log.RegiestLog();//初始化记录线程（定时器）
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
           for(int i = 0; i++ < 1000;)
            {
                    log.RecordLog(LogType.Info, $"日志记录测试数据{i}");
            }
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed.TotalSeconds);
            Console.WriteLine("程序部分操作完成！");
            Console.ReadLine();
*/