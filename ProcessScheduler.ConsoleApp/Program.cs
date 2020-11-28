using ProcessScheduler.Core;
using ProcessScheduler.Core.Pickers;
using System;
using System.Linq;

namespace ProcessScheduler.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var processes = new Process[]
            {
                new(1, "A", "Daniel", ts(2), ts(0)),
                new(2, "B", "Daniel", ts(1), ts(0)),
                new(3, "C", "Daniel", ts(1), ts(5)),
                new(4, "D", "Daniel", ts(1), ts(1)),
            };

            var picker = new RoundRobinPicker(TimeSpan.FromMilliseconds(700));

            picker.ProcessCreated += Picker_ProcessCreated;

            var manager = new ProcessManager();

            manager.ProcessCompleted += Manager_ProcessCompleted;
            manager.ProcessExecutionStarted += Manager_ProcessExecutionStarted;
            manager.ProcessExecutionStopped += Manager_ProcessExecutionStopped;

            manager.Run(processes, picker);

            static TimeSpan ts(int val) => TimeSpan.FromSeconds(val);
        }

        private static void Picker_ProcessCreated(Process process, ProcessExecutionEventArgs args)
        {
            Line();
            Log(args, $"Process {process} was created.");
            Line();
        }

        private static void Manager_ProcessExecutionStopped(ProcessExecutionEventArgs args)
        {
            Log(args, $"Process {args.Process} is now stopped. Elapsed: {Ts(args.Process.CurrentExecutionTime)}");
        }

        private static void Manager_ProcessExecutionStarted(ProcessExecutionEventArgs args)
        {
            Log(args, $"Process {args.Process} is now running.");
        }

        private static void Manager_ProcessCompleted(ProcessExecutionEventArgs args)
        {
            Line();
            Log(args, $"Process {args.Process} finished executing.");
            Line();
        }

        private static void Log(ProcessExecutionEventArgs args, string message)
        {
            var queue = string.Join("<-", args.Queue.Select(p => $"[{p.Id}]"));

            Console.WriteLine($"|{Ts(args.CurrentTime)}|{queue}|{message}");
        }

        private static void Line()
        {
            Console.WriteLine(new string(Enumerable.Range(0, Console.WindowWidth).Select(i => '=').ToArray()));
        }

        private static string Ts(TimeSpan value) => $"{value:hh\\:mm\\:ss\\.fff}";
    }
}
