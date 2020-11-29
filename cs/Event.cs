using System;
using System.Threading;

namespace EventTest {

    delegate void Handler ();

    public int bagsNumber=0;

    class Counter {
        public event Handler pubEvent;//声明事件
        public void DoCount () {
            for (int i = 1; i < 100; i++) {
                Thread.Sleep(50);
                Console.WriteLine(" "+i);
                //bagsNumber++;
                if (i % 10 == 0 && pubEvent != null) {
                    Console.WriteLine(">> pubEvent()");
                    pubEvent ();//触发事件
                }
            }
        }
    }

    class Subber0
    {
        public int BagsCount{get;private set;}
        public Subber0(){
            BagsCount=0;
            //increr.pubEvent+=RunDozens;
        }
        public void Get_bag(){
            BagsCount++;
            bagsNumber++;
            Console.WriteLine("BagsCount++, -> {0}",BagsNumber);
        }
    }
    
    class Subber1
    {
        public MoveBag(){
            BagsNumber--;
            Console.WriteLine("bag moved");
        }
    }

    class Program
    {
        static void Main(){
            Counter pubber=new Counter();
            Subber0 subber0=new Subber0();
            pubber.pubEvent+=subber0.Get_bag;
            pubber.DoCount();
            Console.WriteLine("Number of Bag = {0}",subber0.BagsCount);
        }
    }
}