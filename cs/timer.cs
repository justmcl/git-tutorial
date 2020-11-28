using System;
using System.IO;
using System.Threading;
using System.Timers;
namespace tst {
	public class pro {
		public int a = 0;
		public static void Main () {
			Console.WriteLine ("hello,mono");
			dog kit = new dog ();
			Thread.Sleep (10000);
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
}