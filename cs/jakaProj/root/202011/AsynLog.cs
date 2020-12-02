using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
namespace Wyw.Utilities {
    /// <summary> 
    /// LogHelper 
    /// 异步日志类 
    ///  
    /// 修改记录 
    ///20120312 全部采用了静态，增加了取方法位置函数与每天新日志 
    ///20120404 增加了消息体结构及方法 
    /// 版本：1.0 
    /// <author> 
    ///     <name>Wyw308</name> 
    ///     <date>2012.03.11</date> 
    /// </author> 
    /// </summary> 
    public class LogHelper {
        private static string filePath = "log\\";
        private static string fileName = DateTime.Now.ToString ("yyyyMMdd") + ".log";
        /// <summary> 
        /// 文件路径 
        /// </summary> 
        public static string FilePath {
            get { return filePath; }
            set { filePath = value; }
        }
        /// <summary> 
        /// 文件名 
        /// </summary> 
        public static string FileName {
            get { return fileName; }
            set { fileName = value; }
        }

        static Mutex myMutex = new Mutex ();
        private delegate void WriteStrToFileDelegate (Msg msg);
        #region 将消息体写入日志文件 
        /// <summary> 
        /// 将消息体写入日志文件 
        /// </summary> 
        /// <param name="msg">消息体</param> 
        private static void WriteMsgToFile (Msg msg) {
            if (!Directory.Exists (LogHelper.FilePath))
                Directory.CreateDirectory (LogHelper.FilePath);
            StreamWriter sw = null;
            myMutex.WaitOne (); //线程互斥 
            FileInfo finfo = new FileInfo (filePath + "\\" + fileName);
            if (!finfo.Exists) {
                sw = File.CreateText (filePath + "\\" + fileName);
            } else {
                sw = new StreamWriter (finfo.OpenWrite ());
            }
            sw.BaseStream.Seek (0, SeekOrigin.End);
            sw.WriteLine (string.Format ("{0}", msg.Datetime) + "\t" + msg.Type + "\t" + msg.Location + "\t" + msg.Text);
            sw.Flush ();
            sw.Close ();
            myMutex.ReleaseMutex ();
        }
        #endregion 
        #region 外部调用 
        /// <summary> 
        /// 获取方法类名等位置 
        /// </summary> 
        /// <param name="frameIndex">方法级别，默认1为调用它的方法名</param> 
        /// <returns>返回这样形式：shenghua_riba.Form1.WriMsg()</returns> 
        public static string GetMethodLocal (int frameIndex) {
            string str = string.Empty;
            StackTrace ss = new StackTrace (true);
            if (ss.FrameCount < frameIndex)
                frameIndex = ss.FrameCount;
            MethodBase mb = ss.GetFrame (frameIndex).GetMethod ();
            ////取得方法命名空间 
            //str += mb.DeclaringType.Namespace + "\n"; 
            ////取得方法类名 
            //str += mb.DeclaringType.Name + "\n"; 
            //取得方法类全名 
            str += mb.DeclaringType.FullName + ".";
            //取得法名 
            str += mb.Name + "()";
            return str;
        }
        public static void Write (string msgText) {
            Write (DateTime.Now, MsgType.Information, "", msgText);
        }
        public static void Write (string msgLocation, string msgText) {
            Write (DateTime.Now, MsgType.Information, msgLocation, msgText);
        }
        public static void Write (MsgType msgType, string msgLocation, string msgText) {
            Write (DateTime.Now, msgType, msgLocation, msgText);
        }
        /// <summary> 
        /// 基础方法 
        /// </summary> 
        /// <param name="msgDataTime"></param> 
        /// <param name="msgType"></param> 
        /// <param name="msgLocation"></param> 
        /// <param name="msgText"></param> 
        public static void Write (DateTime msgDataTime, MsgType msgType, string msgLocation, string msgText) {
            new WriteStrToFileDelegate (WriteMsgToFile).BeginInvoke (new Msg (msgDataTime, msgType, msgLocation, msgText), null, null);
        }
        #endregion 

        #region Msg 
        /// <summary> 
        /// 表示一个日志记录的对象 
        /// </summary> 
        private class Msg {
            //日志记录的时间 
            private DateTime datetime;
            //日志记录的类型 
            private MsgType type;
            // 日志位置 
            private string location;
            //日志记录的内容 
            private string text;

            /// <summary> 
            /// 创建新的日志记录实例; 
            /// </summary> 
            /// <param name="msgDataTime"></param> 
            /// <param name="msgType"></param> 
            /// <param name="msgLocation"></param> 
            /// <param name="msgText"></param> 
            public Msg (DateTime msgDataTime, MsgType msgType, string msgLocation, string msgText) {
                datetime = msgDataTime;
                type = msgType;
                location = msgLocation;
                text = msgText;
            }

            /// <summary> 
            /// 获取或设置日志记录的时间 
            /// </summary> 
            public DateTime Datetime {
                get { return datetime; }
                set { datetime = value; }
            }
            public string Location {
                get { return location; }
                set { location = value; }
            }

            /// <summary> 
            /// 获取或设置日志记录的消息类型 
            /// </summary> 
            public MsgType Type {
                get { return type; }
                set { type = value; }
            }
            /// <summary> 
            /// 获取或设置日志记录的文本内容 
            /// </summary> 
            public string Text {
                get { return text; }
                set { text = value; }
            }

        }
        #endregion 
        #region MsgType 
        /// <summary> 
        /// 日志消息类型的枚举 
        /// </summary> 
        public enum MsgType {
            /// <summary> 
            /// 普通信息 
            /// </summary> 
            Information,
            /// <summary> 
            /// 指示警告信息类型的日志记录 
            /// </summary> 
            Warning,
            /// <summary> 
            /// 指示错误信息类型的日志记录 
            /// </summary> 
            Error,
            /// <summary> 
            /// 指示成功信息类型的日志记录 
            /// </summary> 
            Success,
            /// <summary> 
            /// 指示致命类型的日志记录 
            /// </summary> 
            Fatal
        }
        #endregion 
    }
}