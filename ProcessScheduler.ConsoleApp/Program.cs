using CommandLine;
using ProcessScheduler.Core;
using ProcessScheduler.Core.Csv;
using System;
using System.Linq;

namespace ProcessScheduler.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            new Parser(settings =>
            {
                settings.CaseSensitive = false;
                settings.CaseInsensitiveEnumValues = true;
                settings.HelpWriter = Console.Error;

            }).ParseArguments<ProcessSchedulerOptions>(args)
                .WithParsed(opt =>
                {
                    if (opt.Interactive) Console.WriteLine("Interactive mode, press enter after any update to continue.");

                    var processes = ProcessParser.ParseProcesses(opt.FileName);

                    var picker = opt.GetProcessPicker();

                    picker.ProcessCreated += Picker_ProcessCreated;

                    var manager = new ProcessManager();

                    manager.ProcessCompleted += Manager_ProcessCompleted;
                    manager.ProcessExecutionStarted += Manager_ProcessExecutionStarted;
                    manager.ProcessExecutionStopped += Manager_ProcessExecutionStopped;

                    manager.Run(processes, picker);

                    void Picker_ProcessCreated(Process process, ProcessExecutionEventArgs args)
                    {
                        Line();
                        Log(args, $"Process {process} was created.");
                        Line();

                        ReadLineIfInteractive(opt);
                    }

                    void Manager_ProcessExecutionStopped(ProcessExecutionEventArgs args)
                    {
                        Log(args, $"Process {args.Process} is now stopped. Elapsed: {Ts(args.Process.CurrentExecutionTime)}");

                        ReadLineIfInteractive(opt);
                    }

                    void Manager_ProcessExecutionStarted(ProcessExecutionEventArgs args)
                    {
                        Log(args, $"Process {args.Process} is now running.");
                    }

                    void Manager_ProcessCompleted(ProcessExecutionEventArgs args)
                    {
                        Line();
                        Log(args, $"Process {args.Process} finished executing.");
                        Line();

                        ReadLineIfInteractive(opt);
                    }
                });
        }

        private static void ReadLineIfInteractive(ProcessSchedulerOptions opt)
        {
            if (opt.Interactive) Console.ReadLine();
        }

        private static double CpuShare(Process process, TimeSpan totalTime)
        {
            if (process.CurrentExecutionTime == TimeSpan.Zero
                || totalTime == TimeSpan.Zero)
            {
                return 0;
            }

            return (process.CurrentExecutionTime / totalTime);
        }

        private static void Log(ProcessExecutionEventArgs args, string message)
        {
            var queue = string.Join("<-", args.Queue.Select(p => $"[{p.Name} ({CpuShare(p, args.CurrentTime):0.00})]"));

            Console.WriteLine($"|{Ts(args.CurrentTime)}|{queue}|{message}");
        }

        private static void Line()
        {
            Console.WriteLine(new string(Enumerable.Range(0, Console.WindowWidth - 1).Select(i => '=').Prepend('|').ToArray()));
        }

        private static string Ts(TimeSpan value) => $"{value:hh\\:mm\\:ss\\.fff}";
    }
}
