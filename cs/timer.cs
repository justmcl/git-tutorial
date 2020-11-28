using System;
using System.IO;
using System.Threading;
using System.Timers;
namespace tst {
	public class pro {
		int a = 666;
		public static void Main () {
			Console.WriteLine ("hello,mono");
			Console.Write (35 + "\n");

			System.Timers.Timer mtimer1;
			mtimer1 = new System.Timers.Timer (100);
			mtimer1.Elapsed += new ElapsedEventHandler (go);
			mtimer1.AutoReset = true;
			mtimer1.Enabled = true;
			Thread.Sleep (3000);
		}

		public static void go (object o, ElapsedEventArgs e) {
			int i=0;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine ("gogogo");
			Console.WriteLine (i);
			Console.WriteLine(Thread.CurrentThread.Name);
		}
	}
}