using System;
using System.Timers;
using System.IO;

namespace tst{
	public class pro
	{
		public static void Main()
		{
			Console.WriteLine("hello,mono");
			Console.Write(35);
			private System.Timers.Timer mtimer0;
			mtimer0 =new System.Timers.Timer(100);
			mtimer0.Elapsed+=new ElapsedEventHandler(go);
			mtimer0.AutoReset=true;
			mtimer0.Enabled=true;
		}

		private void go()
		{
			Console.WriteLine("gogogo");
		}
	}
}
