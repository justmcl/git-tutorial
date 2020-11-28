using System;
using System.IO;
using System.Threading;
using System.Timers;
namespace tst {
	public class pro {
		public int a = 0;
		public static void Main () {
			Console.WriteLine ("hello,mono");
			
			//dog kit = new dog ();
			computer tab=new computer();
			Thread.Sleep (1000);
		}
	}
	public class dog {
		public int age = 0;
		public int number=0;
		public dog () {
			System.Timers.Timer mtimer1;
			mtimer1 = new System.Timers.Timer (500);
			mtimer1.Elapsed += new ElapsedEventHandler (go);
			mtimer1.AutoReset = true;
			mtimer1.Enabled = true;
		}
		public void go (object o, ElapsedEventArgs e) {
			//int j = 0;
			Console.ForegroundColor = ConsoleColor.Red;
			age++;
			number++;
			//Console.WriteLine (age);
			//age++;
			//Console.WriteLine ("gogogo");
			//Console.WriteLine (j);
			//Console.WriteLine(Thread.CurrentThread.Name);
			int mnum=number;
			for (int i = 0; i <100; i++) {
				//for (int j = 0; j < mnum; j++) {
				//	Console.Write (" ");
				//}
				Thread.Sleep (100);
				Console.WriteLine ("({0},{1})",mnum,i);
			}
		}
	}
	public class computer {
		public int age = 0;
		public int number=0;
		public computer () {
			System.Timers.Timer mtimer1;
			mtimer1 = new System.Timers.Timer (1);
			mtimer1.Elapsed += new ElapsedEventHandler (go);
			mtimer1.AutoReset = false;
			mtimer1.Enabled = true;
		}
		public void go (object o, ElapsedEventArgs e) {
			for (double i = 0; i < 1000000; i=i+0.1)
            {
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.Write("##yyy##");
                double  j=i*i*i;
                Console.WriteLine("{0},{1}",i,j);
                //Thread.Sleep(0);
			}
		}
	}
	
}
public class BF_CheckUpdate
 2     {
 3         private static object LockObject = new Object();
 4 
 5         // 定义数据检查Timer
 6         private static Timer CheckUpdatetimer = new Timer();
 7 
 8         // 检查更新锁
 9         private static int CheckUpDateLock = 0;
10 
11         ///
12         /// 设定数据检查Timer参数
13         ///
14         internal static void GetTimerStart()
15         {
16             // 循环间隔时间(10分钟)
17             CheckUpdatetimer.Interval = 600000;
18             // 允许Timer执行
19             CheckUpdatetimer.Enabled = true;
20             // 定义回调
21             CheckUpdatetimer.Elapsed += new ElapsedEventHandler(CheckUpdatetimer_Elapsed);
22             // 定义多次循环
23             CheckUpdatetimer.AutoReset = true;
24         }
25 
26         ///
27         /// timer事件
28         ///
29         ///
30         ///
31         private static void CheckUpdatetimer_Elapsed(object sender, ElapsedEventArgs e)
32         {
33            // 加锁检查更新锁
34             lock (LockObject)
35             {
36                 if (CheckUpDateLock == 0) CheckUpDateLock = 1;
37                 else return;
38             }         
39           
40            //More code goes here.
41           //具体实现功能的方法
42            Check();
43               // 解锁更新检查锁
44             lock (LockObject)
45             {
46                 CheckUpDateLock = 0;
47             }            
48         }
49 }
