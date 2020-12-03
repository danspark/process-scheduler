using CommandLine;
using ProcessScheduler.Core;
using ProcessScheduler.Core.Pickers;
using System;

namespace ProcessScheduler.ConsoleApp
{
    public class ProcessSchedulerOptions
    {
        [Option('a', "algoritmo", Default = AlgorithmType.Fifo)]
        public AlgorithmType Algorithm { get; set; }

        [Option('p', "arquivo", Required = true)]
        public string FileName { get; set; }

        [Option('q', "tempo", Default = 50)]
        public int Quantum { get; set; }

        [Option('s', "steps", Default = false)]
        public bool Interactive { get; set; }

        public ProcessPicker GetProcessPicker() => Algorithm switch
        {
            AlgorithmType.Fifo => new FirstInFirstOutPicker(),
            AlgorithmType.Sjf => new ShortestJobFirstPicker(),
            AlgorithmType.Rr => new RoundRobinPicker(TimeSpan.FromMilliseconds(Quantum)),
            AlgorithmType.Garantido => new GuaranteedPicker(TimeSpan.FromMilliseconds(Quantum)),
            AlgorithmType.Loteria => new LotteryPicker(TimeSpan.FromMilliseconds(Quantum)),
            _ => throw new NotImplementedException()
        };
    }
}
