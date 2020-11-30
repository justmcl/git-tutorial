#define fstream
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
//using System.Data;
using System.IO;

namespace jakatest2 {

#if fstream
    class prog {
        static void Main () {
            Stream s = new FileStream ("text.txt", FileMode.Create);
            try {
                Console.WriteLine ("Read: {0}", s.CanRead);
                Console.WriteLine ("Write: {0}", s.CanWrite);
                Console.WriteLine ("Seek: {0}", s.CanSeek);
                s.WriteByte (101);
                s.WriteByte (102);
                byte[] block = { 1, 2, 3, 4, 5 };
                s.Write (block, 0, block.Length);
                Console.WriteLine (s.Length);
                string sFilePath = "~/" + DateTime.Now.ToString ("yyyyMM");
                string sFileName = "rizhi" + DateTime.Now.ToString ("dd") + ".log";
                sFileName = sFilePath + "/" + sFileName; //文件的绝对路径
                Console.WriteLine (sFileName);
            } finally {
                if (s != null) {
                    ((IDisposable) s).Dispose ();
                }
            }

            Thread.Sleep (1000);

            using (Stream ss = new FileStream ("text.txt", FileMode.Create)) {
                Console.WriteLine ("Read: {0}", ss.CanRead);
                Console.WriteLine ("Write: {0}", ss.CanWrite);
                Console.WriteLine ("Seek: {0}", ss.CanSeek);
            }

            WriteLog ("i have a apple");
        }

        public static void WriteLog (string strLog) {
            string sFilePath = "root/" + DateTime.Now.ToString ("yyyyMM");
            string sFileName = "rizhi " + DateTime.Now.ToString ("dd") + ".log ";
            sFileName = sFilePath + "/" + sFileName; //文件的绝对
            if (!Directory.Exists (sFilePath)) //验证路径是否存在
            {
                Directory.CreateDirectory (sFilePath);
                //不存在则创建
            }
            FileStream fs;
            StreamWriter sw;
            if (File.Exists (sFileName))
            //验证文件是否存在，有则追加，无则创建
            {
                fs = new FileStream (sFileName, FileMode.Append, FileAccess.Write);
            } else {
                fs = new FileStream (sFileName, FileMode.Create, FileAccess.Write);
            }
            sw = new StreamWriter (fs);
            //sw.WriteLine (DateTime.Now.ToString ("yy-MM-dd HH-mm-ss") + "---" + strLog);
            sw.WriteLine (DateTime.Now.ToString ("HH/mm/ss") + "---" + strLog);

            sw.Close ();
            fs.Close ();
        }

    }

#endif

#if frw
    class Prog {
        static void Main (string[] args) {
            Console.WriteLine ("please write your file name");
            string filename = Console.ReadLine ();
            Console.WriteLine ("############## creat ###############");
            Console.WriteLine (filename);
            Console.WriteLine ("write something");
            string filecont = Console.ReadLine ();
            if (!File.Exists (filename) || true) {
                File.WriteAllText (filename, filecont, Encoding.Default);
                //string text=File.ReadAllText(filename,Encoding.Default);
            }
        }
    }
#endif

#if jaka
    #region interface /**********/

    delegate void CallBackFuncType (int error_code);
    public class JakaInterface {
        //机器人控制句柄
        int i = 1;
        int num = 1;
        double last;
        int b = 0;
        //tcp的位置
        JKTYPE.CartesianPose TcpPosition = new JKTYPE.CartesianPose ();
        //机器人状态
        JKTYPE.RobotState state = new JKTYPE.RobotState ();
        JKTYPE.RobotStatus status = new JKTYPE.RobotStatus ();

        //定时器，用于更新tcp位置
        private System.Timers.Timer GetPosition_timer;
        private System.Timers.Timer UpdateStatue_timer;

        JKTYPE.JointValue home_pos = new JKTYPE.JointValue ();

        public JakaInterface () {
            int a;
            a = jakaAPI.create_handler ("192.168.1.100".ToCharArray (), ref i); //替换自己的ip
            Console.WriteLine ($"机器人控制句柄{i}创建: {a}");
            //jakaAPI.power_on(ref i);  //机器人上电
            //jakaAPI.enable_robot(ref i);  //机器人上使能
            //jakaAPI.set_rapidrate(ref i, 0.6);  //设置机器人运行倍率
        }
        public void JakaStart () //必须创建jakainterface时手动调用
        {

            //获取tcp的位置，间隔1ms，实时更新
            jakaAPI.get_tcp_position (ref i, ref TcpPosition);
            last = TcpPosition.tran.x;
            Thread.Sleep (3000);
            StartTimer0 ();
            StartTimer1 ();
            Console.WriteLine ("StartTimer1");

        }
        private void StartTimer0 () {
            GetPosition_timer = new System.Timers.Timer (20);
            GetPosition_timer.Elapsed += new ElapsedEventHandler (GetPosition);
            GetPosition_timer.AutoReset = true;
            GetPosition_timer.Enabled = false;
        }
        private void StartTimer1 () {
            UpdateStatue_timer = new System.Timers.Timer (20);
            UpdateStatue_timer.Elapsed += new ElapsedEventHandler (GetRobotStatues);
            UpdateStatue_timer.AutoReset = true;
            UpdateStatue_timer.Enabled = true;
        }

        private void GetPosition (object source, ElapsedEventArgs e) {
            try {
                jakaAPI.get_tcp_position (ref i, ref TcpPosition);
                DoSomething (TcpPosition); //在方法里面进行判断
            } catch (Exception ex) {
                Console.Write ("Primary getRobotState error: " + ex.Message + "\r\n");
            }
        }
        private void GetRobotStatues (object source, ElapsedEventArgs e) {
            try {
                jakaAPI.get_robot_status (ref i, ref status);
                DoSomething0 (status); //在方法里面进行判断
            } catch (Exception ex) {
                Console.Write ("Primary getRobotState error: " + ex.Message + "\r\n");
            }
        }
        private void DoSomething (JKTYPE.CartesianPose Position) {
            Console.WriteLine ("{0}", Position.tran.x);
            //判断位置是否到达
            if (Position.tran.x - last > 10 || Position.tran.x - last < -10) {
                last = TcpPosition.tran.x;
                Console.WriteLine ("############触发{0}##############", num);

                jakaAPI.set_digital_output (ref i, JKTYPE.IOType.IO_CABINET, 1, true); //设置电控柜数字输出1为高电平
                Thread.Sleep (10); //等待10ms
                jakaAPI.set_digital_output (ref i, JKTYPE.IOType.IO_CABINET, 1, false); //设置电控柜数字输出1为低电平

                jakaAPI.set_analog_output (ref i, JKTYPE.IOType.IO_CABINET, 1, 65535); //设置电控柜模拟输出1为65535
                Thread.Sleep (10); //等待10ms
                jakaAPI.set_analog_output (ref i, JKTYPE.IOType.IO_CABINET, 1, 0); //设置电控柜数字输出1为0

                num++;
            }
        }
        private void DoSomething0 (JKTYPE.RobotStatus status) {

            if (true) {
                Console.WriteLine ("{0}", status.torq_sensor_monitor_data.actTorque[2]);
                b++;
                Console.WriteLine (b);
            }
        }

        public void Home () {
            Console.WriteLine ("回到home点");

        }
        public void Move () {
            Console.WriteLine ("即将开始直线运动，距离：301mm");
            Thread.Sleep (3000);
            JKTYPE.CartesianPose movepose = new JKTYPE.CartesianPose (); //目标点在tcp坐标系下的位姿
            movepose.tran.x = 301;
            jakaAPI.linear_move (ref i, ref movepose, JKTYPE.MoveMode.INCR, true, 30);
        }
        public void Move0 () {
            Console.WriteLine ("即将开始直线运动，距离：301mm");
            Thread.Sleep (3000);
            JKTYPE.CartesianPose movepose = new JKTYPE.CartesianPose (); //目标点在tcp坐标系下的位姿
            for (int i = 0; i < 10001; i++) {
                movepose.tran.z = 100;
                jakaAPI.linear_move (ref i, ref movepose, JKTYPE.MoveMode.INCR, true, 90);
                movepose.tran.z = -100;
                jakaAPI.linear_move (ref i, ref movepose, JKTYPE.MoveMode.INCR, true, 90);
            }

        }

    }
    public class CSVFileHelper {
        /// <summary>
        /// 将DataTable中数据写入到CSV文件中
        /// </summary>
        /// <param name="dt">提供保存数据的DataTable</param>
        /// <param name="fileName">CSV的文件路径</param>
        public static void SaveCSV (DataTable dt, string fullPath) {
            FileInfo fi = new FileInfo (fullPath);
            if (!fi.Directory.Exists) {
                fi.Directory.Create ();
            }
            FileStream fs = new FileStream (fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            StreamWriter sw = new StreamWriter (fs, System.Text.Encoding.UTF8);
            string data = "";
            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++) {
                data += dt.Columns[i].ColumnName.ToString ();
                if (i < dt.Columns.Count - 1) {
                    data += ",";
                }
            }
            sw.WriteLine (data);
            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++) {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++) {
                    string str = dt.Rows[i][j].ToString ();
                    str = str.Replace ("\"", "\"\""); //替换英文冒号 英文冒号需要换成两个冒号
                    if (str.Contains (',') || str.Contains ('"') ||
                        str.Contains ('\r') || str.Contains ('\n')) //含逗号 冒号 换行符的需要放到引号中
                    {
                        str = string.Format ("\"{0}\"", str);
                    }

                    data += str;
                    if (j < dt.Columns.Count - 1) {
                        data += ",";
                    }
                }
                sw.WriteLine (data);
            }
            sw.Close ();
            fs.Close ();

        }

        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        //public static DataTable OpenCSV(string filePath)
        //{
        //    Encoding encoding = Common.GetType(filePath); //Encoding.ASCII;//
        //    DataTable dt = new DataTable();
        //    FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

        //    //StreamReader sr = new StreamReader(fs, Encoding.UTF8);
        //    StreamReader sr = new StreamReader(fs, encoding);
        //    //string fileContent = sr.ReadToEnd();
        //    //encoding = sr.CurrentEncoding;
        //    //记录每次读取的一行记录
        //    string strLine = "";
        //    //记录每行记录中的各字段内容
        //    string[] aryLine = null;
        //    string[] tableHead = null;
        //    //标示列数
        //    int columnCount = 0;
        //    //标示是否是读取的第一行
        //    bool IsFirst = true;
        //    //逐行读取CSV中的数据
        //    while ((strLine = sr.ReadLine()) != null)
        //    {
        //        //strLine = Common.ConvertStringUTF8(strLine, encoding);
        //        //strLine = Common.ConvertStringUTF8(strLine);

        //        if (IsFirst == true)
        //        {
        //            tableHead = strLine.Split(',');
        //            IsFirst = false;
        //            columnCount = tableHead.Length;
        //            //创建列
        //            for (int i = 0; i < columnCount; i++)
        //            {
        //                DataColumn dc = new DataColumn(tableHead[i]);
        //                dt.Columns.Add(dc);
        //            }
        //        }
        //        else
        //        {
        //            aryLine = strLine.Split(',');
        //            DataRow dr = dt.NewRow();
        //            for (int j = 0; j < columnCount; j++)
        //            {
        //                dr[j] = aryLine[j];
        //            }
        //            dt.Rows.Add(dr);
        //        }
        //    }
        //    if (aryLine != null && aryLine.Length > 0)
        //    {
        //        dt.DefaultView.Sort = tableHead[0] + " " + "asc";
        //    }

        //    sr.Close();
        //    fs.Close();
        //    return dt;
        //}
        public static DataTable ReadCSV (string filePath) {
            DataTable dt = new DataTable ();
            int lineNumber = 0;
            using (CsvFileReader reader = new CsvFileReader (filePath)) {
                CsvRow row = new CsvRow ();
                while (reader.ReadRow (row)) {

                    if (0 == lineNumber) {
                        foreach (string s in row) {
                            dt.Columns.Add (s.Replace ("\"", ""));
                        }
                    } else {
                        int index = 0;
                        DataRow dr = dt.NewRow ();
                        foreach (string s in row) {
                            dr[index] = s.Replace ("\"", "");
                            index++;
                        }
                        dt.Rows.Add (dr);
                    }
                    lineNumber++;
                }
            }
            return dt;
        }

        public class CsvRow : List<string> {
            public string LineText { get; set; }
        }
        public class CsvFileReader : StreamReader {
            public CsvFileReader (Stream stream) : base (stream) { }

            public CsvFileReader (string filename) : base (filename) { }

            /// <summary>  
            /// Reads a row of data from a CSV file  
            /// </summary>  
            /// <param name="row"></param>  
            /// <returns></returns>  
            public bool ReadRow (CsvRow row) {
                row.LineText = ReadLine ();
                if (String.IsNullOrEmpty (row.LineText))
                    return false;

                int pos = 0;
                int rows = 0;

                while (pos < row.LineText.Length) {
                    string value;

                    // Special handling for quoted field  
                    if (row.LineText[pos] == '"') {
                        // Skip initial quote  
                        pos++;

                        // Parse quoted value  
                        int start = pos;
                        while (pos < row.LineText.Length) {
                            // Test for quote character  
                            if (row.LineText[pos] == '"') {
                                // Found one  
                                pos++;

                                // If two quotes together, keep one  
                                // Otherwise, indicates end of value  
                                if (pos >= row.LineText.Length || row.LineText[pos] != '"') {
                                    pos--;
                                    break;
                                }
                            }
                            pos++;
                        }
                        value = row.LineText.Substring (start, pos - start);
                        value = value.Replace ("\"\"", "\"");
                    } else {
                        // Parse unquoted value  
                        int start = pos;
                        while (pos < row.LineText.Length && row.LineText[pos] != ',')
                            pos++;
                        value = row.LineText.Substring (start, pos - start);
                    }

                    // Add field to list  
                    if (rows < row.Count)
                        row[rows] = value;
                    else
                        row.Add (value);
                    rows++;

                    // Eat up to and including next comma  
                    while (pos < row.LineText.Length && row.LineText[pos] != ',')
                        pos++;
                    if (pos < row.LineText.Length)
                        pos++;
                }
                // Delete any unused items  
                while (row.Count > rows)
                    row.RemoveAt (rows);

                // Return true if any columns read  
                return (row.Count > 0);
            }

        }
    }
    #region jakaapi
    class jakaAPI {
        [DllImport ("jakaAPI.dll", EntryPoint = "get_controller_ip", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_controller_ip (char[] controller_name, char[] ip);
        [DllImport ("jakaAPI.dll", EntryPoint = "create_handler", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int create_handler (char[] ip, ref int handle);
        [DllImport ("jakaAPI.dll", EntryPoint = "destory_handler", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int destory_handler (ref int handle);
        [DllImport ("jakaAPI.dll", EntryPoint = "power_on", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int power_on (ref int handle);
        [DllImport ("jakaAPI.dll", EntryPoint = "power_off", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int power_off (ref int handle);
        [DllImport ("jakaAPI.dll", EntryPoint = "shut_down", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int shut_down (ref int handle);

        [DllImport ("jakaAPI.dll", EntryPoint = "enable_robot", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int enable_robot (ref int handle);
        [DllImport ("jakaAPI.dll", EntryPoint = "disable_robot", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int disable_robot (ref int handle);
        [DllImport ("jakaAPI.dll", EntryPoint = "jog", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int jog (ref int handle, int aj_num, JKTYPE.MoveMode move_mode, JKTYPE.CoordType coord_type, double vel_cmd, double pos_cmd);
        [DllImport ("jakaAPI.dll", EntryPoint = "jog_stop", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int jog_stop (ref int handle, ref int num);
        [DllImport ("jakaAPI.dll", EntryPoint = "joint_move", ExactSpelling = false, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern int joint_move (ref int handle, ref JKTYPE.JointValue joint_pos, JKTYPE.MoveMode move_mode, bool is_block, double speed);

        [DllImport ("jakaAPI.dll", EntryPoint = "linear_move", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int linear_move (ref int handle, ref JKTYPE.CartesianPose end_pos, JKTYPE.MoveMode move_mode, bool is_block, double speed);
        [DllImport ("jakaAPI.dll", EntryPoint = "servo_move_enable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int servo_move_enable (ref int handle, bool enable);
        [DllImport ("jakaAPI.dll", EntryPoint = "servo_j", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int servo_j (ref int handle, ref JKTYPE.JointValue joint_pos, JKTYPE.MoveMode move_mode);
        [DllImport ("jakaAPI.dll", EntryPoint = "servo_p", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int servo_p (ref int handle, ref JKTYPE.CartesianPose cartesian_pose, JKTYPE.MoveMode move_mode);
        [DllImport ("jakaAPI.dll", EntryPoint = "set_digital_output", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_digital_output (ref int handle, JKTYPE.IOType type, int index, bool value);
        [DllImport ("jakaAPI.dll", EntryPoint = "set_analog_output", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int set_analog_output (ref int handle, JKTYPE.IOType type, int index, float value);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_digital_input", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_digital_input (ref int handle, JKTYPE.IOType type, int index, ref bool result);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_digital_output", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int get_digital_output (ref int handle, JKTYPE.IOType type, int index, ref bool result);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_analog_input", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int get_analog_input (ref int handle, JKTYPE.IOType type, int index, ref float result);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_analog_output", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_analog_output (ref int handle, JKTYPE.IOType type, int index, ref float result);
        [DllImport ("jakaAPI.dll", EntryPoint = "is_extio_running", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int is_extio_running (ref int handle, ref bool is_running);
        [DllImport ("jakaAPI.dll", EntryPoint = "program_run", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int program_run (ref int handle);
        [DllImport ("jakaAPI.dll", EntryPoint = "program_pause", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int program_pause (ref int handle);
        [DllImport ("jakaAPI.dll", EntryPoint = "program_resume", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int program_resume (ref int handle);
        [DllImport ("jakaAPI.dll", EntryPoint = "program_abort", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int program_abort (ref int handle);
        [DllImport ("jakaAPI.dll", EntryPoint = "program_load", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int program_load (ref int handle, char[] file);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_loaded_program", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int get_loaded_program (ref int handle, StringBuilder file);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_current_line", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int get_current_line (ref int handle, ref int curr_line);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_program_state", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int get_program_state (ref int handle, ref JKTYPE.ProgramState status);
        [DllImport ("jakaAPI.dll", EntryPoint = "set_rapidrate", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int set_rapidrate (ref int handle, double rapid_rate);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_rapidrate", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int get_rapidrate (ref int handle, ref double rapid_rate);
        [DllImport ("jakaAPI.dll", EntryPoint = "set_tool_data", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int set_tool_data (ref int handle, int id, ref JKTYPE.CartesianPose tcp, char[] name);
        [DllImport ("jakaAPI.dll", EntryPoint = "set_tool_id", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int set_tool_id (ref int handle, int id);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_tool_id", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int get_tool_id (ref int handle, ref int id);
        [DllImport ("jakaAPI.dll", EntryPoint = "set_user_frame_data", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int set_user_frame_data (ref int handle, int id, ref JKTYPE.CartesianPose user_frame, char[] name);
        [DllImport ("jakaAPI.dll", EntryPoint = "set_user_frame_id", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int set_user_frame_id (ref int handle, int id);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_user_frame_id", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int get_user_frame_id (ref int handle, ref int id);
        [DllImport ("jakaAPI.dll", EntryPoint = "drag_mode_enable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int drag_mode_enable (ref int handle, bool enable);
        [DllImport ("jakaAPI.dll", EntryPoint = "is_in_drag_mode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int is_in_drag_mode (ref int handle, ref bool in_drag);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_robot_state", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int get_robot_state (ref int handle, ref JKTYPE.RobotState state);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_tcp_position", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int get_tcp_position (ref int handle, ref JKTYPE.CartesianPose tcp_position);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_joint_position", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int get_joint_position (ref int handle, ref JKTYPE.JointValue joint_position);
        [DllImport ("jakaAPI.dll", EntryPoint = "is_in_collision", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int is_in_collision (ref int handle, ref bool in_collision);
        [DllImport ("jakaAPI.dll", EntryPoint = "is_on_limit", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int is_on_limit (ref int handle, ref bool on_limit);
        [DllImport ("jakaAPI.dll", EntryPoint = "is_in_pos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int is_in_pos (ref int handle, ref bool in_pos);
        [DllImport ("jakaAPI.dll", EntryPoint = "collision_recover", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int collision_recover (ref int handle);
        [DllImport ("jakaAPI.dll", EntryPoint = "set_collision_level", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int set_collision_level (ref int handle, int level);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_collision_level", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_collision_level (ref int handle, ref int level);
        [DllImport ("jakaAPI.dll", EntryPoint = "kine_inverse", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int kine_inverse (ref int handle, ref JKTYPE.JointValue ref_pos, ref JKTYPE.CartesianPose cartesian_pose, ref JKTYPE.JointValue joint_pos);
        [DllImport ("jakaAPI.dll", EntryPoint = "kine_forward", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int kine_forward (ref int handle, ref JKTYPE.JointValue joint_pos, ref JKTYPE.CartesianPose cartesian_pose);
        [DllImport ("jakaAPI.dll", EntryPoint = "rpy_to_rot_matrix", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int rpy_to_rot_matrix (ref int handle, ref JKTYPE.Rpy rpy, ref JKTYPE.RotMatrix rot_matrix);
        [DllImport ("jakaAPI.dll", EntryPoint = "rot_matrix_to_rpy", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int rot_matrix_to_rpy (ref int handle, ref JKTYPE.RotMatrix rot_matrix, ref JKTYPE.Rpy rpy);
        [DllImport ("jakaAPI.dll", EntryPoint = "quaternion_to_rot_matrix", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int quaternion_to_rot_matrix (ref int handle, ref JKTYPE.Quaternion quaternion, ref JKTYPE.RotMatrix rot_matrix);
        [DllImport ("jakaAPI.dll", EntryPoint = "rot_matrix_to_quaternion", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int rot_matrix_to_quaternion (ref int handle, ref JKTYPE.RotMatrix rot_matrix, ref JKTYPE.Quaternion quaternion);
        [DllImport ("jakaAPI.dll", EntryPoint = "torque_control_enable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int torque_control_enable (ref int handle, bool enable);
        [DllImport ("jakaAPI.dll", EntryPoint = "torque_feedforward", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int torque_feedforward (ref int handle, JKTYPE.TorqueValue tor_val, int grv_flag);
        [DllImport ("jakaAPI.dll", EntryPoint = "set_payload", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int set_payload (ref int handle, ref JKTYPE.PayLoad payload);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_payload", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int get_payload (ref int handle, ref JKTYPE.PayLoad payload);
        [DllImport ("jakaAPI.dll", EntryPoint = "set_error_handler", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_error_handler (ref int i, CallBackFuncType func);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_sdk_version", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_sdk_version (ref int i, StringBuilder version);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_controller_ip", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_controller_ip (StringBuilder controller_name, StringBuilder ip_list);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_robot_status", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_robot_status (ref int i, ref JKTYPE.RobotStatus status);
        [DllImport ("jakaAPI.dll", EntryPoint = "motion_abort", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int motion_abort (ref int i);
        [DllImport ("jakaAPI.dll", EntryPoint = "set_errorcode_file_path", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_errorcode_file_path (ref int i, StringBuilder path);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_last_error", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_last_error (ref int i, ref JKTYPE.ErrorCode code);
        [DllImport ("jakaAPI.dll", EntryPoint = "set_debug_mode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_debug_mode (ref int i, bool mode);
        [DllImport ("jakaAPI.dll", EntryPoint = "set_traj_config", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_traj_config (ref int i, ref JKTYPE.TrajTrackPara para);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_traj_config", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_traj_config (ref int i, ref JKTYPE.TrajTrackPara para);
        [DllImport ("jakaAPI.dll", EntryPoint = "set_traj_sample_mode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_traj_sample_mode (ref int i, bool mode, char[] filename);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_traj_sample_status", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_traj_sample_status (ref int i, ref bool sample_statuse);
        [DllImport ("jakaAPI.dll", EntryPoint = "get_exist_traj_file_name", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_exist_traj_file_name (ref int i, ref JKTYPE.MultStrStorType filename);
        [DllImport ("jakaAPI.dll", EntryPoint = "rename_traj_file_name", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rename_traj_file_name (ref int i, ref char[] src, ref char[] dest);
        [DllImport ("jakaAPI.dll", EntryPoint = "remove_traj_file", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int remove_traj_file (ref int i, ref char[] filename);
        [DllImport ("jakaAPI.dll", EntryPoint = "generate_traj_exe_file", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int generate_traj_exe_file (ref int i, ref char[] filename);
        [DllImport ("jakaAPI.dll", EntryPoint = "joint_move_extend", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int joint_move_extend (ref int i, ref JKTYPE.JointValue joint_pos, JKTYPE.MoveMode move_mode, bool is_block, double speed, double acc, double tol, ref JKTYPE.OptionalCond option_cond);
        [DllImport ("jakaAPI.dll", EntryPoint = "linear_move_extend", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int linear_move_extend (ref int i, ref JKTYPE.CartesianPose cart_pos, JKTYPE.MoveMode move_mode, bool is_block, double speed, double acc, double tol, ref JKTYPE.OptionalCond option_cond);
        [DllImport ("jakaAPI.dll", EntryPoint = "linear_move_extend", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int circular_move (ref int i, ref JKTYPE.CartesianPose end_pos, ref JKTYPE.CartesianPose mid_pos, JKTYPE.MoveMode move_mode, bool is_block, double speed, double acc, double tol, ref JKTYPE.OptionalCond option_cond);
    }
    #endregion
    #endregion

#endif 
}