using System;
using System.Threading;

namespace AsyncTest {
    class Program {
        int num = 0;
        static readonly object _locker = new object ();
        //dog0.age=0;
        static void Main (string[] args) {
            adder dog0 = new adder ();

            Thread trd1 = new Thread (dog0.beginAdd);
            Thread trd2 = new Thread (dog0.beginAdd);
            Thread trd3 = new Thread (dog0.beginAdd);
            Thread trd4 = new Thread (dog0.beginAdd); 
            Thread trd5 = new Thread (dog0.beginAdd);
            Thread trd6 = new Thread (dog0.beginAdd);
            Thread trd7 = new Thread (dog0.beginAdd);
            Thread trd8 = new Thread (dog0.beginAdd);
            Thread trd10 = new Thread (dog0.beginAdd);
            Thread trd20 = new Thread (dog0.beginAdd);
            Thread trd30 = new Thread (dog0.beginAdd);
            Thread trd40 = new Thread (dog0.beginAdd);
            Thread trd50 = new Thread (dog0.beginAdd);
            Thread trd60 = new Thread (dog0.beginAdd);
            Thread trd70 = new Thread (dog0.beginAdd);
            Thread trd80 = new Thread (dog0.beginAdd);
            Thread trd100 = new Thread (dog0.beginAdd);
            Thread trd200 = new Thread (dog0.beginAdd);
            Thread trd300 = new Thread (dog0.beginAdd);
            Thread trd400 = new Thread (dog0.beginAdd);
            Thread trd500 = new Thread (dog0.beginAdd);
            Thread trd600 = new Thread (dog0.beginAdd);
            Thread trd700 = new Thread (dog0.beginAdd);
            Thread trd800 = new Thread (dog0.beginAdd);
            Thread trd1a = new Thread (dog0.beginAdd);
            Thread trd2a = new Thread (dog0.beginAdd);
            Thread trd3a = new Thread (dog0.beginAdd);
            Thread trd4a = new Thread (dog0.beginAdd);
            Thread trd5a = new Thread (dog0.beginAdd);
            Thread trd6a = new Thread (dog0.beginAdd);
            Thread trd7a = new Thread (dog0.beginAdd);
            Thread trd8a = new Thread (dog0.beginAdd);
            Thread trd10a = new Thread (dog0.beginAdd);
            Thread trd20a = new Thread (dog0.beginAdd);
            Thread trd30a = new Thread (dog0.beginAdd);
            Thread trd40a = new Thread (dog0.beginAdd);
            Thread trd50a = new Thread (dog0.beginAdd);
            Thread trd60a = new Thread (dog0.beginAdd);
            Thread trd70a = new Thread (dog0.beginAdd);
            Thread trd80a = new Thread (dog0.beginAdd);
            Thread trd100a = new Thread (dog0.beginAdd);
            Thread trd200a = new Thread (dog0.beginAdd);
            Thread trd300a = new Thread (dog0.beginAdd);
            Thread trd400a = new Thread (dog0.beginAdd);
            Thread trd500a = new Thread (dog0.beginAdd);
            Thread trd600a = new Thread (dog0.beginAdd);
            Thread trd700a = new Thread (dog0.beginAdd);
            Thread trd800a = new Thread (dog0.beginAdd);
            trd1.Start ();
            trd2.Start ();
            trd3.Start ();
            trd4.Start ();
            trd5.Start ();
            trd6.Start ();
            trd7.Start ();
            trd8.Start ();
            trd10.Start ();
            trd20.Start ();
            trd30.Start ();
            trd40.Start ();
            trd50.Start ();
            trd60.Start ();
            trd70.Start ();
            trd80.Start ();
            trd100.Start ();
            trd200.Start ();
            trd300.Start ();
            trd400.Start ();
            trd500.Start ();
            trd600.Start ();
            trd700.Start ();
            trd800.Start ();
            trd1a.Start ();
            trd2a.Start ();
            trd3a.Start ();
            trd4a.Start ();
            trd5a.Start ();
            trd6a.Start ();
            trd7a.Start ();
            trd8a.Start ();
            trd10a.Start ();
            trd20a.Start ();
            trd30a.Start ();
            trd40a.Start ();
            trd50a.Start ();
            trd60a.Start ();
            trd70a.Start ();
            trd80a.Start ();
            trd100a.Start ();
            trd200a.Start ();
            trd300a.Start ();
            trd400a.Start ();
            trd500a.Start ();
            trd600a.Start ();
            trd700a.Start ();
            trd800a.Start ();
            //Thread.Sleep(3000);
            Console.WriteLine (dog0.number);
        }

    }
    public class adder {
        object locker=new object();
        public int number;
        public void beginAdd () {
            DoAddL ();
        }
        void DoAddL () {
            lock (this) {
                for (int i = 0; i < 1000; i++) {
                    //changeColor (i % 3);
                    //lock ( ref num) {
                    number++;
                    //Thread.Sleep (1);
                    Console.WriteLine ("{0} {1}", i, number);
                    //}
                }
            }
        }
        void DoAdd () {
            for (int i = 0; i < 1000; i++) {
                //changeColor (i % 3);
                //lock ( ref num) {
                number++;
                //Thread.Sleep (1);
                Console.WriteLine ("{0} {1}", i, number);
                //}
            }

        }
        void changeColor (int i) {
            switch (i) {
                case 0:
                    //Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case 1:
                    //Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                default:
                    //Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
        }
    }
}