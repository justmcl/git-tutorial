using System;
using System.Timers;
using System.IO;
using System.Threading;
namespace tst{
	public class pro
	{
		int a = 666;
		public static void Main()
		{
			Console.WriteLine("hello,mono");
			Console.Write(35+"\n");
			
			System.Timers.Timer mtimer0;
			mtimer0 =new System.Timers.Timer(100);
			mtimer0.Elapsed+=new ElapsedEventHandler(go);
			mtimer0.AutoReset=true;
			mtimer0.Enabled=true;
			Thread.Sleep(3000);
		}

		public static void go(object o,ElapsedEventArgs e)
		{
			int a=666*666;
			Console.ForegroundColor=ConsoleColor.Red;
			Console.WriteLine("gogogo");
			Console.WriteLine(a);
		}
	}
}
