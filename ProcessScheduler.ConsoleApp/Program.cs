using CommandLine;
using ProcessScheduler.Core;
using ProcessScheduler.Core.Csv;
using System;
using System.Linq;
using System.Text;

namespace ProcessScheduler.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            new Parser(settings =>
            {
                settings.CaseSensitive = false;
                settings.CaseInsensitiveEnumValues = true;
                settings.HelpWriter = Console.Error;

            }).ParseArguments<ProcessSchedulerOptions>(args)
                .WithParsed(opt =>
                {
                    if (opt.Interactive) Console.WriteLine("Modo interativo, pressione ENTER após qualquer atualização para continuar.");
                    
                    var processes = ProcessParser.ParseProcesses(opt.FileName);
                    
                    Console.WriteLine($"Algorítmo:: {opt.Algorithm}");
                    Console.WriteLine($"Quantum: {opt.Quantum}ms");
                    Console.WriteLine($"Quantidade de processos: {processes.Count}");

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
                        Log(args, $"Processo {process} foi criado.");
                        Line();

                        ReadLineIfInteractive(opt);
                    }

                    void Manager_ProcessExecutionStopped(ProcessExecutionEventArgs args)
                    {
                        Log(args, $"Processo {args.Process} deixou de executar. Tempo de execução: {Ts(args.Process.CurrentExecutionTime)}");

                        ReadLineIfInteractive(opt);
                    }

                    void Manager_ProcessExecutionStarted(ProcessExecutionEventArgs args)
                    {
                        Log(args, $"Processo {args.Process} entrou em execução.");
                    }

                    void Manager_ProcessCompleted(ProcessExecutionEventArgs args)
                    {
                        Line();
                        Log(args, $"Processo {args.Process} completou sua execução.");
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
