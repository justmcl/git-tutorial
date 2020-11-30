/*
try catch finally
*/
using System;
class Test {
    static int Calc (int x) => 10 / x;
    static void Main () {
        try {
            int y = Calc (0);
            Console.WriteLine (y);
        } catch (System.Exception ex) {
            Console.WriteLine(ex.Message);
        }finally{
            Console.WriteLine("bad");
        }
        Console.WriteLine("program complete");
    }
}