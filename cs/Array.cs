using System;
using System.Collections;

namespace ArrayTest {
    public class prog {
        static void Main () {
            Sensor s1=new   Sensor();
            Console.WriteLine (s1.tor[0]);

            ArrayList a1=new ArrayList();
            Console.WriteLine (a1);

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(i);
                i++;
            }
        }

    }
    public class Sensor {
       public int[] tor = new int[5];

    }
}