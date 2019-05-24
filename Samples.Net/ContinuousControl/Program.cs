using System;
using Autolabor.PM1;

namespace ContinuousControl.Net {
    class Program {
        static void Main() {
            try {
                Console.Write("initialize: ");
                Console.WriteLine("{0} connected", Methods.Initialize("", out _));
                Methods.State = StateEnum.Unlocked;

                Console.WriteLine("--------------");
                Console.WriteLine("↖↑↗ | q w e");
                Console.WriteLine("←  → | a   d");
                Console.WriteLine("↙↓↘ | z x c");
                Console.WriteLine("--------------");
                Console.WriteLine("alt + s to exit");
                Console.WriteLine();

                var running = true;
                while (running) {
                    var info = Console.ReadKey(true);
                    switch (char.ToLower(info.KeyChar)) {
                        case 'q': // 左前
                            Methods.VelocityTarget = (+0.1, +0.2);
                            break;
                        case 'w': // 前
                            Methods.VelocityTarget = (+0.1, 0);
                            break;
                        case 'e': // 右前
                            Methods.VelocityTarget = (+0.1, -0.2);
                            break;
                        case 'a': // 左
                            Methods.VelocityTarget = (0, +0.2);
                            break;
                        case 's': // 终止
                            running = info.Modifiers != ConsoleModifiers.Alt;
                            break;
                        case 'd': // 右
                            Methods.VelocityTarget = (0, -0.2);
                            break;
                        case 'z': // 左后
                            Methods.VelocityTarget = (-0.1, -0.2);
                            break;
                        case 'x': // 后
                            Methods.VelocityTarget = (-0.1, 0);
                            break;
                        case 'c': // 右后
                            Methods.VelocityTarget = (-0.1, +0.2);
                            break;
                        default: break;
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
