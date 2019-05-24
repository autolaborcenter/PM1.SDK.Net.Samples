using System;
using Autolabor.PM1;

namespace Initialization.Net {
    public static class Program {
        public static void Main() {
            try {
                Console.Write("initialize:  ");
                // 连接串口
                Console.WriteLine("{0} connected", Methods.Initialize("", out _));
                // 解锁
                Methods.State = StateEnum.Unlocked;            
                // 以 0.25rad/s 的速度原地转 90°
                Console.Write("turn around: ");
                Methods.TurnAround(0.25, 1.57, out _);   
                Console.WriteLine("done");
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            } finally {
                // 断开连接
                Methods.ShutdownSafety();                      
            }
            Console.WriteLine("press any key to continue");
            Console.ReadKey();
        }
    }
}
