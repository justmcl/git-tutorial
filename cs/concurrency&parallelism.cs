#define g
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadTest {
    public class Program {
        static object locker = new object ();
        static public void Main (string[] args) {

#if a//Thread最基本使用
            Thread thread = new Thread (sayHi);
            thread.Start ();
            Thread thread0 = new Thread (sayHello);
            thread0.Start ();
#endif
#if b //属性和方法
            /*Thread属性:
                Name
                IsAlive
                静态属性：Thread.CurrentThread
            Thread方法：
                Join();
                Sleep();
            */
            Thread.CurrentThread.Name = "MainT";

            Thread t0 = new Thread (sayHi);
            t0.Name = "W_Y_T";
            t0.Start ();
            t0.Join ();
            Thread.Sleep (TimeSpan.FromSeconds (0.5));
            Console.WriteLine ("Thread end");

#endif
#if c//向线程传递参数,Lambda表达式,
            /*
            Thread构造器接收的委托：
                public delegate void ThreadStart ();
                public delegate void PrarmeterizedThreadStart (object obj)
            */
            Thread t0 = new Thread (sayHi);
            t0.Start ();

            Thread t1 = new Thread (Say);
            t1.Start ("i want you !");

            Thread.Sleep (1500);

            Stopwatch s = Stopwatch.StartNew ();
            Console.WriteLine (s.ElapsedMilliseconds + " 0");

            Thread t2 = new Thread (() => {
                Console.WriteLine (s.ElapsedMilliseconds + " begin");
                give ("你很棒棒哦!");
                Console.WriteLine (s.ElapsedMilliseconds + " end");

            });
            Console.WriteLine (s.ElapsedMilliseconds + " 1");

            t2.Start ();
            Console.WriteLine (s.ElapsedMilliseconds + " 2");
            Thread.Sleep (2500);

            new Thread (() => give ("👍")).Start ();
            new Thread (() => {
                give ("666🎉✨");
                give ("6！");
            }).Start ();

            for (int i = 0; i < 33; i++) {
                new Thread (() => Console.Write (" " + i)).Start ();
            }

            Thread.Sleep (1000);
            Console.Write ("\n");

            for (int i = 0; i < 33; i++) {
                int j = i;
                new Thread (() => Console.Write (" " + j)).Start ();
            }
            Thread.Sleep (1000);
            Console.Write ("\n");

            String str = "good";
            Thread t3 = new Thread (() => Console.WriteLine (str)); //str?
            str = "bad";
            Thread t4 = new Thread (() => Console.WriteLine (str)); //str?
            t3.Start ();
            t4.Start ();
            //good or bad ?

#endif
#if d//lock，线程安全
            adder dog0 = new adder ();
            Stopwatch s0 = Stopwatch.StartNew ();
            for (int i = 0; i < 64; i++) {
                Thread trd1 = new Thread (dog0.beginAdd);
                trd1.Start ();
                //trd1.Join();
            }

            Console.WriteLine (s0.ElapsedMilliseconds);

#endif
#if e//多线程异常处理
            //try
            //{
            new Thread (go).Start ();
            //}
            //catch (System.Exception ex)
            //{
            //Console.WriteLine("Exception!");
            //}
#endif
#if f//线程优先级
            //ElapsedTicks
            //Stopwatch.Frequency
            Stopwatch s = Stopwatch.StartNew ();
            Console.WriteLine (s.ElapsedMilliseconds + " 0");

            Thread t0 = new Thread (() => {
                Console.WriteLine (s.ElapsedMilliseconds + " 1begin");
                for (int i = 0; i < 100000; i++) {
                    changeColor (2);
                    Console.Write (" mamamama! " + i);
                    Thread.Sleep (0);
                }
                Console.WriteLine ("\n" + s.ElapsedMilliseconds + " end1");
            });

            Console.WriteLine (s.ElapsedMilliseconds + " 1");
            t0.Start ();
            Console.WriteLine (s.ElapsedMilliseconds + " 2");

            //t0.Join ();
            Console.WriteLine (s.ElapsedMilliseconds + " 3");

            for (int i = 0; i < 100000; i++) {
                changeColor (1);
                Console.Write (" wryyyyyy! ");
                Thread.Sleep (0);
            }
            Console.WriteLine ("\n" + s.ElapsedMilliseconds + " main end");

#endif
#if g//task
            Stopwatch s = Stopwatch.StartNew ();
            Console.WriteLine (s.ElapsedMilliseconds + " 0");

            Task.Run (() => {
                Console.WriteLine (s.ElapsedMilliseconds+" 1");
                give ("上上签");
            });
            //Thread.Sleep (0);
            Console.WriteLine (s.ElapsedMilliseconds + " 2");
#endif
        }
        static void go () {
            try {
                throw null;
            } catch (System.Exception) {
                Console.WriteLine ("Excception!");
            }
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
                        Console.ForegroundColor = ConsoleColor.Cyan;s;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                }
            }
        }
    }
    public class adder {
        object locker = new object ();
        public int number;

        public void beginAdd () {
            Stopwatch s = Stopwatch.StartNew ();
            DoAddL ();
            Console.WriteLine (s.ElapsedMilliseconds);
        }
        void DoAddL () {
            lock (this) {
                for (int i = 0; i < 10000; i++) {
                    changeColor (i % 3);
                    number++;
                    //Thread.Sleep (1);
                    Console.WriteLine ("{0} {1}", i, number);
                }
            }
        }
        void DoAdd () {
            for (int i = 0; i < 1000; i++) {
                changeColor (i % 3);
                number++;
                Console.WriteLine ("{0} {1}", i, number);
            }

        }
        void changeColor (int i) {
            lock (locker) {
                switch (i) {
                    case 0:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case 1:
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                }
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