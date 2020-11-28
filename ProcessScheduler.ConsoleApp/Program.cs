using ProcessScheduler.Core;
using ProcessScheduler.Core.Pickers;
using System;
using System.Collections.Generic;
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

            var picker = new SjfProcessPicker();

            picker.ProcessCreated += Picker_ProcessCreated;

            var manager = new ProcessManager();

            manager.ProcessCompleted += Manager_ProcessCompleted;
            manager.ProcessExecutionStarted += Manager_ProcessExecutionStarted;
            manager.ProcessExecutionStopped += Manager_ProcessExecutionStopped;

            var scheduler = new DefaultProcessScheduler();

            scheduler.Run(processes, picker, manager);

            static TimeSpan ts(int val) => TimeSpan.FromSeconds(val);
        }

        private static void Picker_ProcessCreated(ProcessEventArgs args, IEnumerable<Process> processes)
        {
            var queue = string.Join("<-", processes.Select(p => $"[{p.Id}]"));
            Log(args.CurrentTime, $"{args.Process} was scheduled. {queue}");
        }

        private static void Manager_ProcessExecutionStopped(ProcessExecutionEventArgs args)
        {
            Log(args.CurrentTime, $"{args.Process} is now stopped.");
        }

        private static void Manager_ProcessExecutionStarted(ProcessExecutionEventArgs args)
        {
            Log(args.CurrentTime, $"{args.Process} is now running.");
        }

        private static void Manager_ProcessCompleted(ProcessEventArgs args)
        {
            Log(args.CurrentTime, $"{args.Process} finished executing.");
        }

        private static void Log(TimeSpan time, string message)
        {
            Console.WriteLine($"{time}| {message}");
        }
    }
}
