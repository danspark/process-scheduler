using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessScheduler.Core.Pickers
{
    public class LotteryPicker : ProcessPicker
    {
        private readonly Random _random = new Random(Environment.TickCount);
        private readonly List<Process> _processes = new List<Process>();

        public LotteryPicker(TimeSpan quantum)
        {
            Quantum = quantum;
        }

        public TimeSpan Quantum { get; }

        public override IEnumerable<Process> GetAllProcesses()
        {
            return _processes;
        }

        public override ProcessToken? GetNext()
        {
            if (_processes.Any())
            {
                return new ProcessToken(this,
                    _processes[_random.Next(0, _processes.Count)],
                    Quantum);
            }

            return HasProcesses() ? null : throw new InvalidOperationException("No incoming processes");
        }

        public override void RemoveProcess(Process process)
        {
            _processes.Remove(process);
        }

        protected override void AddImpl(Process process)
        {
            _processes.Add(process);
        }

        protected override bool HasProcessesImpl()
        {
            return _processes.Any();
        }
    }
}
