using System;
using System.Threading;
using System.Threading.Tasks;
using Autolabor.PM1;

namespace ActionControl.Net {
    public static class Program {
        public static void Help()
            => Console.WriteLine(@"
- control
  - quit
  - lock
  - unlock
- actions
  - forward:  go forward for 1 meter
  - backward: go backward for 1 meter
  - left:     turn left for 90°
  - right:    turn right for 90°
  - alt + p:  pause the action
- action paused:
  - continue: continue for the action
  - break:    cancel the action"
               );

        public static void Main() {
            var progress = .0;
            var state = false;

            Console.Write("initialize: ");

            try {
                Console.WriteLine("{0} connected", Methods.Initialize("", out _));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine("press any key to continue...");
                Console.ReadKey();
                return;
            }

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

            Help();

            while (true) {
                progress = 0;
                state = true;

                Console.WriteLine();
                Console.Write(">> ");
                var cmd = Console.ReadLine().Trim();

                var progressTask = Task.Run(async () => {
                    var temp = 0;
                    while (state) {
                        if (progress > 0 && !Methods.Paused) {
                            Console.Write("[{0,3:N0}%]\b\b\b\b\b\b", progress * 100);
                            for (; temp < 100 * progress; ++temp) Console.Write('=');
                        }
                        await Task.Delay(50);
                    }
                    if (progress > 0) Console.WriteLine();
                });

                var task = Task.Run(() => {
                    while (state) {
                        var info = Console.ReadKey(true);
                        if (!state 
                         || char.ToLower(info.KeyChar) != 'p'
                         || info.Modifiers != ConsoleModifiers.Alt)
                            continue;

                        Methods.Paused = true;
                        Console.WriteLine();

                        while (Methods.Paused) {
                            Console.WriteLine();
                            Console.Write(">> ");
                            switch (Console.ReadLine().Trim()) {
                                case "help":
                                    Help();
                                    break;
                                case "break":
                                    progress = 0;
                                    Methods.CancelAction();
                                    Methods.Paused = false;
                                    break;
                                case "continue":
                                    Methods.Paused = false;
                                    break;
                                default:
                                    Console.WriteLine("unknown: enter \"help\" for help");
                                    break;
                            }
                        }
                    }
                });

                try {
                    switch (cmd) {
                        case "quit":
                            Methods.ShutdownSafety();
                            return;
                        case "help":
                            Help();
                            break;
                        case "lock":
                            Methods.State = StateEnum.Locked;
                            while (Methods.State != StateEnum.Locked) Thread.Sleep(100);
                            Console.WriteLine("locked");
                            break;
                        case "unlock":
                            Methods.State = StateEnum.Unlocked;
                            while (Methods.State != StateEnum.Unlocked) Thread.Sleep(100);
                            Console.WriteLine("unlocked");
                            break;
                        case "forward":
                            Methods.GoStraight(+0.1, 1, out progress);
                            break;
                        case "backward":
                            Methods.GoStraight(-0.1, 1, out progress);
                            break;
                        case "left":
                            Methods.TurnAround(+0.2, Math.PI / 2, out progress);
                            break;
                        case "right":
                            Methods.TurnAround(-0.2, Math.PI / 2, out progress);
                            break;
                        default:
                            Console.WriteLine("unknown: enter \"help\" for help");
                            break;
                    }
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                } finally {
                    state = false;
                    progressTask.Wait();
                    Console.WriteLine("press any key to continue...");
                    task.Wait();
                }
            }
        }
    }
}