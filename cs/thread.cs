/*
Thread类测试
*/
#define d
using System;
using System.Threading;

namespace ThreadTest {
    public class Program {
        int num = 0;
        static object locker = new object ();
        static public void Main (string[] args) {

            //Thread最基本使用
#if a
            Thread thread = new Thread (sayHi);
            thread.Start ();
            Thread thread0 = new Thread (sayHello);
            thread0.Start ();
#endif
            /*
            Thread属性:
                Name
                IsAlive
                静态属性：Thread.CurrentThread
            Thread方法：
                Join();
                Sleep();
            */
#if b
            Thread.CurrentThread.Name = "MainT";

            Thread t0 = new Thread (sayHi);
            t0.Name = "W_Y_T";
            t0.Start ();
            t0.Join ();
            Thread.Sleep (TimeSpan.FromSeconds (0.5));
            Console.WriteLine ("Thread end");

#endif
            /*向线程传递参数
            Thread构造器接收的委托：
                public delegate void ThreadStart ();
                public delegate void PrarmeterizedThreadStart (object obj)
            */
#if c
            Thread t0 = new Thread (sayHi);
            t0.Start ();
            Thread t1 = new Thread (Say);
            t1.Start ("i want you !");

            Thread.Sleep (1500);

            Thread t2 = new Thread (() => give ("你很棒棒哦!"));
            t2.Start ();
            new Thread (() => give ("👍")).Start ();
            new Thread (() => {
                give ("666🎉✨");
                give ("6！");
            }).Start ();

#endif
            /*Lambda表达式
             */
#if d
            for (int i = 0; i < 33; i++) {
                new Thread (() => Console.Write (" "+i)).Start ();
            }

            Thread.Sleep(1000);
            Console.Write("\n");

            for (int i = 0; i < 33; i++) {
                int j=i;
                new Thread (() => Console.Write (" "+j)).Start ();
            }
            Thread.Sleep(1000);
            Console.Write("\n");

            String str = "good";
            Thread t1 = new Thread (()=>Console.WriteLine(str));//str?
            str = "bad";
            Thread t2 = new Thread (()=>Console.WriteLine(str));//str?
            t1.Start();
            t2.Start();

#endif
        }
        static void Say (object obj) {
            String msg = (String) obj;
            for (int i = 0; i < 66; i++) {
                changeColor (i % 3);
                for (int j = 0; j < i % 8; j++) {
                    Console.Write ("    ");
                }
                Thread.Sleep (8);
                Console.WriteLine (msg);
            }
        }
        static void sayHi () {
            for (int i = 0; i < 66; i++) {
                changeColor (i % 3);
                for (int j = 0; j < i % 8; j++) {
                    Console.Write ("    ");
                }
                Thread.Sleep (8);
                Console.WriteLine ("Hi");
            }
        }
        static void sayHello () {
            for (int i = 0; i < 66; i++) {
                changeColor (i % 3);
                for (int j = 0; j < i % 8; j++) {
                    Console.Write ("    ");
                }
                Thread.Sleep (8);
                Console.WriteLine ("Hello");
            }
        }
        static void give (int int0) {
            for (int i = 0; i < 250; i++) {
                changeColor (i % 3);
                for (int j = 0; j < 6; j++) {
                    Console.Write (" " + int0);
                }
                Console.Write (" ");
                Thread.Sleep (8);
            }
        }
        static void give (String str) {
            for (int i = 0; i < 250; i++) {
                changeColor (i % 3);
                for (int j = 0; j < 6; j++) {
                    Console.Write (" " + str);
                }
                Console.Write (" ");
                Thread.Sleep (8);
            }
        }
        static public void changeColor (int i) {
            lock (locker) {
                switch (i) {
                    case 0:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                }
            }
        }
    }
    public class adder {
        int number;
        public void Add () {
            DoAdd (number);
        }
        void DoAdd (int number) {
            for (int i = 0; i < 100; i++) {
                //changeColor (i % 3);
                number = i;
                Thread.Sleep (5);
                Console.WriteLine ("{0} {1}", i, number);
            }
        }
    }
}
//    All the foreground colors except DarkCyan, the background color:
//       The foreground color is Black.
//       The foreground color is DarkBlue.
//       The foreground color is DarkGreen.
//       The foreground color is DarkRed.
//       The foreground color is DarkMagenta.
//       The foreground color is DarkYellow.
//       The foreground color is Gray.
//       The foreground color is DarkGray.
//       The foreground color is Blue.
//       The foreground color is Green.
//       The foreground color is Cyan.
//       The foreground color is Red.
//       The foreground color is Magenta.
//       The foreground color is Yellow.
//       The foreground color is White.