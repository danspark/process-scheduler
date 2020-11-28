using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessScheduler.Core.Pickers
{
    public class RoundRobinPicker : ProcessPicker
    {
        private readonly LinkedList<Process> _processes = new();

        public TimeSpan Quantum { get; }

        public RoundRobinPicker(TimeSpan quantum)
        {
            Quantum = quantum;
        }

        public override ProcessToken? GetNext()
        {
            var nextNode = _processes.First;

            if (nextNode is null)
            {
                return HasProcesses() ? null : throw new InvalidOperationException("No incoming processes");
            }

            _processes.RemoveFirst();

            _processes.AddLast(nextNode);

            return new(this, nextNode.Value, Quantum);
        }

        public override void RemoveProcess(Process process)
        {
            _processes.Remove(process);
        }

        protected override void AddImpl(Process process)
        {
            _processes.AddLast(process);
        }

        protected override IEnumerable<Process> GetAllProcesses()
        {
            return _processes.ToList();
        }

        public override bool HasProcesses() => base.HasProcesses() || _processes.Any();
    }
}
