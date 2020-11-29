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
			computer tab = new computer ();
			Thread.Sleep (1000);
		}
	}
	public class dog {
		public int age = 0;
		public int number = 0;
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
			int mnum = number;
			for (int i = 0; i < 100; i++) {
				//for (int j = 0; j < mnum; j++) {
				//	Console.Write (" ");
				//}
				Thread.Sleep (100);
				Console.WriteLine ("({0},{1})", mnum, i);
			}
		}
	}
	public class computer {
		public int age = 0;
		public int number = 0;
		public computer () {
			System.Timers.Timer mtimer1;
			mtimer1 = new System.Timers.Timer (1);
			mtimer1.Elapsed += new ElapsedEventHandler (go);
			mtimer1.AutoReset = false;
			mtimer1.Enabled = true;
		}
		public void go (object o, ElapsedEventArgs e) {
			for (double i = 0; i < 1000000; i = i + 0.1) {
				//Console.ForegroundColor = ConsoleColor.Red;
				//Console.Write("##yyy##");
				double j = i * i * i;
				Console.WriteLine ("{0},{1}", i, j);
				//Thread.Sleep(0);
			}
		}
	}
	public class BF_CheckUpdate {
		private static object LockObject = new Object ();

		// 定义数据检查Timer
		private static Timer CheckUpdatetimer = new Timer ();

		// 检查更新锁
		private static int CheckUpDateLock = 0;

		///
		/// 设定数据检查Timer参数
		///
		internal static void GetTimerStart () {
			// 循环间隔时间(10分钟)
			CheckUpdatetimer.Interval = 600000;
			// 允许Timer执行
			CheckUpdatetimer.Enabled = true;
			// 定义回调
			CheckUpdatetimer.Elapsed += new ElapsedEventHandler (CheckUpdatetimer_Elapsed);
			// 定义多次循环
			CheckUpdatetimer.AutoReset = true;
		}

		///
		/// timer事件
		///
		///
		///
		private static void CheckUpdatetimer_Elapsed (object sender, ElapsedEventArgs e) {
			// 加锁检查更新锁
			lock (LockObject) {
				if (CheckUpDateLock == 0) CheckUpDateLock = 1;
				else return;
			}

			//More code goes here.
			//具体实现功能的方法
			Check ();
			// 解锁更新检查锁
			lock (LockObject) {
				CheckUpDateLock = 0;
			}
		}

	}
}