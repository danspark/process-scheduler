using ProcessScheduler.Core;
using ProcessScheduler.Core.Pickers;
using System;

namespace ProcessScheduler.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var processes = new Process[]
            {
                new(1, "A", "Daniel", ts(1), ts(0)),
                new(2, "B", "Daniel", ts(2), ts(0)),
                new(3, "C", "Daniel", ts(1), ts(5)),
                new(4, "D", "Daniel", ts(1), ts(1)),
            };

            var picker = new FifoProcessPicker();

            picker.ProcessCreated += Picker_ProcessCreated;

            var manager = new ProcessManager();

            manager.ProcessCompleted += Manager_ProcessCompleted;
            manager.ProcessExecutionStarted += Manager_ProcessExecutionStarted;
            manager.ProcessExecutionStopped += Manager_ProcessExecutionStopped;

            var scheduler = new DefaultProcessScheduler();

            scheduler.Run(processes, picker, manager);

            static TimeSpan ts(int val) => TimeSpan.FromSeconds(val);
        }

        private static void Picker_ProcessCreated(ProcessEventArgs args)
        {
            Log(args.CurrentTime, $"{args.Process} was scheduled. Duration: {args.Process.TotalExecutionTime}");
        }

        private static void Manager_ProcessExecutionStopped(ProcessExecutionEventArgs args)
        {
            Log(args.CurrentTime, $"{args.Process} is now stopped. Duration: {args.Process.CurrentExecutionTime}");
        }

        private static void Manager_ProcessExecutionStarted(ProcessExecutionEventArgs args)
        {
            Log(args.CurrentTime, $"{args.Process} is now running. Duration: {args.Process.CurrentExecutionTime}");
        }

        private static void Manager_ProcessCompleted(ProcessEventArgs args)
        {
            Log(args.CurrentTime, $"{args.Process} finished executing. Duration: {args.Process.TotalExecutionTime}");
        }

        private static void Log(TimeSpan time, string message)
        {
            Console.WriteLine($"{time}| {message}");
        }
    }
}
