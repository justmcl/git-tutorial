using System;
using System.Threading;

namespace AsyncTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "MainT";
            
            Thread trd1 = new Thread(WriteY);
            trd1.Name = "W_Y_T";
            trd1.Start();
            Thread.Sleep(1000);
            // for (int i = 0; i < 1000; i++)
            // {
            //     //Console.ForegroundColor = ConsoleColor.Green;
            //     //Console.Write(Thread.CurrentThread);
            //     Console.Write("##xxx##");
            //     //if (i == 10){trd1.Join();}
            // }
        }
        static void WriteY()
        {
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
