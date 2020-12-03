using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessScheduler.Core.Pickers
{
    public class FirstInFirstOutPicker : ProcessPicker
    {
        protected readonly Queue<Process> _processQueue = new();

        public override ProcessToken? GetNext()
        {
            if (_processQueue.Any())
            {
                var process = _processQueue.Peek();

                return new(this, process, process.TotalExecutionTime);
            }

            return HasProcessesImpl() ? null : throw new InvalidOperationException("No incoming processes");
        }

        protected override bool HasProcessesImpl() => _processQueue.Count > 0;

        public override void RemoveProcess(Process process)
        {
            _processQueue.Dequeue();
        }

        protected override void AddImpl(Process process)
        {
            _processQueue.Enqueue(process);
        }

        public override IEnumerable<Process> GetAllProcesses()
        {
            return _processQueue.ToList();
        }
    }
}
