using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Metadata;
using System.ComponentModel;
using System.Timers;
using System.Threading;
using System.Data;

namespace jakatest2
{
	class Program
    {
		static void Main(string[] args)
		{
            JakaInterface Jaka = new JakaInterface();
            Jaka.JakaStart();
            Thread.Sleep(60000);
        }
        public void csvrun()
        {
            JKTYPE.JointValue home_pos = new JKTYPE.JointValue();
            home_pos.jVal = new double[] { 1.18, 1.85, -2.49, 2.76, 1.70, 0.50 };
            DataTable cvD1 = CSVFileHelper.ReadCSV("qwe.csv");
            int i = 0;
            int a;
            a = jakaAPI.create_handler("192.168.2.18".ToCharArray(), ref i);//替换自己的ip
            Console.WriteLine($"机器人控制句柄{i}创建: {a}");
            jakaAPI.power_on(ref i);  //机器人上电
            jakaAPI.enable_robot(ref i);  //机器人上使能
            jakaAPI.set_rapidrate(ref i, 1);  //设置机器人运行倍率
            Console.WriteLine("home");
            jakaAPI.joint_move(ref i, ref home_pos, JKTYPE.MoveMode.ABS, true, 0.5);
            foreach (DataRow dr in cvD1.Rows)
            {
                int b;
                JKTYPE.CartesianPose pose = new JKTYPE.CartesianPose();
                pose.tran.x = double.Parse(dr[0].ToString());
                pose.tran.y = double.Parse(dr[1].ToString());
                pose.tran.z = 200.0;
                pose.rpy.rx = 3.14;
                pose.rpy.ry = 0;
                pose.rpy.rz = 0;
                Console.WriteLine(pose.tran.x);
                Console.WriteLine(pose.tran.y);
                JKTYPE.OptionalCond jc = new JKTYPE.OptionalCond();

                b = jakaAPI.linear_move_extend(ref i, ref pose, JKTYPE.MoveMode.ABS, false, 50, 100, 1, ref jc);


                Console.WriteLine(b);
                Console.WriteLine(pose.tran.x.GetType().Name);
            }
        }
	}
	//.gettype().typename
	//double numDouble1 = double.Parse(numStr);
}
