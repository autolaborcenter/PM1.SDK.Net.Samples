using System;
using System.Threading;
using Autolabor.PM1;

namespace ActionControl.Net {
    class Program {
        static void Main() {
            try {
                Console.Write("initialize: ");
                Console.WriteLine("{0} connected",Methods.Initialize("", out var progress));

                Console.Write("state:      ");
                switch (Methods.State) {
                    case StateEnum.Offline:
                        Console.WriteLine("offline");
                        break;
                    case StateEnum.Unlocked:
                        Console.WriteLine("unlocked");
                        break;
                    case StateEnum.Error:
                        Console.WriteLine("error");
                        break;
                    case StateEnum.Locked:
                        Console.WriteLine("locked");
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("COMMAND");
                Console.WriteLine("========");

                Console.WriteLine();
                Console.WriteLine("control:");
                Console.WriteLine("--------");
                Console.WriteLine("quit");
                Console.WriteLine("lock");
                Console.WriteLine("unlock");

                Console.WriteLine();
                Console.WriteLine("actions:");
                Console.WriteLine("--------");
                Console.WriteLine("forward:  go forward for 1 meter");
                Console.WriteLine("backward: go backward for 1 meter");
                Console.WriteLine("left:     turn left for 90°");
                Console.WriteLine("right:    turn right for 90°");

                new Thread(() => {
                    while (true) {
                        progress = 0;
                        while (progress == 0) Thread.Sleep(20);

                        var temp = 0;
                        while (progress < 1) {
                            Console.Write("[{0,3:N0}%]\b\b\b\b\b\b", progress * 100);
                            for (; temp < 100 * progress; ++temp) Console.Write('=');
                            Thread.Sleep(20);
                        }
                        Console.WriteLine("[100%]");
                    }
                }) { IsBackground = true }.Start();

                while (true) {
                    while (progress > 0) Thread.Sleep(20);
                    Console.WriteLine();
                    Console.Write(">> ");
                    switch (Console.ReadLine().Trim()) {
                        case "quit":
                            throw new Exception("over");
                        case "lock":
                            Methods.State = StateEnum.Locked;
                            while (Methods.State != StateEnum.Locked) Thread.Sleep(100);
                            Console.WriteLine("[locked]");
                            break;
                        case "unlock":
                            Methods.State = StateEnum.Unlocked;
                            while (Methods.State != StateEnum.Unlocked) Thread.Sleep(100);
                            Console.WriteLine("[unlocked]");
                            break;
                        case "forward":
                            Methods.GoStraight(0.1, 1, out progress);
                            break;
                        case "backward":
                            Methods.GoStraight(-0.1, 1, out progress);
                            break;
                        case "left":
                            Methods.TurnAround(0.2, Math.PI / 2, out progress);
                            break;
                        case "right":
                            Methods.TurnAround(-0.2, Math.PI / 2, out progress);
                            break;
                        default:
                            Console.WriteLine("[unknown command]");
                            break;
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            } finally {
                Methods.ShutdownSafety();
            }
            Console.WriteLine("press any key to continue");
            Console.ReadKey();
        }
    }
}