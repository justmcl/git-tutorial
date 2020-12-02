 public delegate void debugTest (string info);
 //文件记录路径，应用程序目录下
 string pathfile = Environment.CurrentDirectory + "\\J600SCPI_Test.txt";
 /// <summary>
 /// 委托函数
 /// </summary>
 /// <param name="info">信息</param>
 public void debugout (string info) {
     try {
         string outinfo = DateTime.Now.ToString () + "\t" + info + "\r\n";
         if (File.Exists (pathfile)) {
             StreamWriter filewrite = File.AppendText (pathfile);
             filewrite.Write (outinfo);
             filewrite.Close ();
         } else {
             StreamWriter filewrite = File.CreateText (pathfile);
             filewrite.Write (outinfo);
             filewrite.Close ();
         }
     } catch (Exception ex) {
         Functions.Print (ex);
     }
 }
 //调用委托异步输出日志
 debugTest debug_test = new debugTest (debugout);
 debug_test.BeginInvoke ("开始", null, null);