﻿#define de2//宏定义
using System;
using System.Threading;

namespace delegateTest {
    public delegate int Del (int x, int y); //自定义委托
    class Program {
        static void Main (string[] args) {

#if de0 //Func<>,Action<>和自定义delegate;
/*
预定义委托，Func<t>有返回值，Action无返回值
*/
            Cal caler1 = new Cal ();

            Del del1 = new Del (caler1.Add);
            Del del2 = new Del (caler1.Sub);
            Del del3 = new Del (caler1.Mul);
            Del del4 = new Del (caler1.Div);
            Del del5 = new Del (caler1.Div);
            Del del41 = calr1.Div;
            //Del del42 = delegate (int x, int y) { return x / y; };//匿名方法
            //Del del43 = (int x, int y) =>{ return x / y; }; //lambda表达式
            //Del del44 =  (x,y) => { return x / y; };//lambda表达式
            //Del del45 = (x, y) => x / y; //lambda表达式
            int a=8,b=3,c=0;
            c = del1.Invoke (a, b);
            Console.WriteLine (c);
            c = del1 (a, b);
            Console.WriteLine (c);

            Thread.Sleep(1500);

            Action action = new Action (caler1.Report);
            Action<int> action0 = new Action<int> (caler1.Report);
            action.Invoke ();
            action ();
            action0.Invoke(10086);
            action0(10086+1);

            Thread.Sleep (1500);

            Func<int, int, int> func1 = new Func<int, int, int> (caler1.Add);
            Func<int, int, int> func2 = new Func<int, int, int> (caler1.Sub);
            int x = 100;
            int y = 200;
            int z = x;
            z = func1.Invoke (x, y);
            Console.WriteLine (z);
            z = func2.Invoke (x, y);
            Console.WriteLine (z);
            z = func1 (x, y);
            Console.WriteLine (z);
            z = func2 (x, y);
            Console.WriteLine (z);

            Type t = typeof (Del);

            Console.WriteLine ("the name of t: {0}",t.Name);
            Console.WriteLine ("t is a class: {0}",t.IsClass);

#endif

#if de1 //milk and apple,模板方法和回调方法；

            WrapFcatory wf = new WrapFcatory();
            ProductFactory pf = new ProductFactory();
            Logger lgr1 = new Logger();
            Func<Product> func1 = new Func<Product>(pf.MakeMilk);
            Func<Product> func2 = new Func<Product>(pf.MakeApple);
            Action<Product> act1 = new Action<Product>(lgr1.Log);

            Box box1 = wf.WrapProduct(func1, act1);
            Box box2 = wf.WrapProduct(func2, act1);

            Console.WriteLine(box1.product.Name);
            Console.WriteLine(box2.product.Name);

#endif

#if de2 //委托高级

            Student stu1 = new Student () { ID = 1, cc = ConsoleColor.Green };
            Student stu2 = new Student () { ID = 2, cc = ConsoleColor.Yellow };
            Student stu3 = new Student () { ID = 3, cc = ConsoleColor.Red };
            //stu1.DoHomework();
            //stu2.DoHomework();
            //stu3.DoHomework();

            Action dh1 = new Action (stu1.DoHomework);
            Action dh2 = new Action (stu2.DoHomework);
            Action dh3 = new Action (stu3.DoHomework);

            //dh1.BeginInvoke(null,null);
            //dh2.BeginInvoke(null, null);
            //dh3.BeginInvoke(null, null);

            Thread th1 = new Thread (new ThreadStart (dh1.Invoke));
            Thread th2 = new Thread (new ThreadStart (dh2.Invoke));
            Thread th3 = new Thread (new ThreadStart (dh3.Invoke));

            th1.Start ();
            th2.Start ();
            th3.Start ();

            for (int i = 0; i < 15; i++) {
                Thread.Sleep (500);
                Console.WriteLine ("tick {0}", i);
                Console.ForegroundColor = ConsoleColor.Magenta;

            }

#endif
        }
    }
    class Cal {
        public void Report () {
            Console.WriteLine ("have three");
        }
        public void Report (int num) {
            Console.WriteLine ("have {0}",num);
        }
        public int Add (int x, int y) {
            return x + y;
        }
        public int Sub (int x, int y) {
            return x - y;
        }
        public int Mul (int x, int y) {
            return x * y;
        }
        public int Div (int x, int y) {
            return x / y;
        }
    }
    class Logger {
        public void Log (Product product) {
            if (product.Price > 5) {
                Console.WriteLine ("Product '{0}' created at {1}.Price is {2}.", product.Name, DateTime.UtcNow, product.Price);
            }
        }
    }
    class Product {
        public string Name { get; set; }
        public double Price { get; set; }
    }
    class Box {
        public Product product;
    }
    class WrapFcatory {
        public Box WrapProduct (Func<Product> getProduct, Action<Product> log) {
            Box box = new Box ();
            box.product = getProduct ();
            log.Invoke (box.product);
            return box;
        }
    }
    class ProductFactory {
        public Product MakeMilk () {
            Product milk = new Product ();
            milk.Name = "milk";
            milk.Price = 8.6;
            return milk;
        }
        public Product MakeApple () {
            Product apple = new Product ();
            apple.Name = "apple";
            apple.Price = 4.3;
            return apple;
        }

    }
    class Student {
        public int ID { get; set; }
        public ConsoleColor cc { get; set; }
        public void DoHomework () {
            for (int i = 0; i < 4; i++) {
                Thread.Sleep (500);
                Console.WriteLine ("Student{0} has done homework for {1} hours", this.ID, i);
                Console.ForegroundColor = this.cc;

            }
        }
    }
}