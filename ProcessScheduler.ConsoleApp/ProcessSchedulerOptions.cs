using CommandLine;
using ProcessScheduler.Core;
using ProcessScheduler.Core.Pickers;
using System;

namespace ProcessScheduler.ConsoleApp
{
    public class ProcessSchedulerOptions
    {
        [Option('a', "algorithm", Default = AlgorithmType.Fifo)]
        public AlgorithmType Algorithm { get; set; }

        [Option('f', "file", Required = true)]
        public string FileName { get; set; }

        [Option('q', "quantum", Default = 50)]
        public int Quantum { get; set; }

        [Option('i', "interactive", Default = false)]
        public bool Interactive { get; set; }

        public ProcessPicker GetProcessPicker() => Algorithm switch
        {
            AlgorithmType.Fifo => new FirstInFirstOutPicker(),
            AlgorithmType.Sjf => new ShortestJobFirstPicker(),
            AlgorithmType.Rr => new RoundRobinPicker(TimeSpan.FromMilliseconds(Quantum)),
            AlgorithmType.Guaranteed => new GuaranteedPicker(TimeSpan.FromMilliseconds(Quantum)),
            _ => throw new NotImplementedException()
        };
    }
}
