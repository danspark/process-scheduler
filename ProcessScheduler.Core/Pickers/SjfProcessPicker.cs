using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessScheduler.Core.Pickers
{
    public class SjfProcessPicker : ProcessPicker
    {
        private readonly SortedList<TimeSpan, Queue<Process>> _processQueue = new();

        public override bool HasProcesses() => base.HasProcesses() || _processQueue.Values.Any(q => q.Any());

        public override ProcessToken? GetNext()
        {
            if (_processQueue.Any())
            {
                var (totalTime, queue) = _processQueue.First();

                if (queue.Any())
                {
                    var process = queue.Peek();

                    return new(this, process, process.TotalExecutionTime);
                }
            }

            return HasProcesses() ? null : throw new InvalidOperationException("No incoming processes");
        }

        public override void RemoveProcess(Process process)
        {
            var queue = _processQueue[process.TotalExecutionTime];

            queue.Dequeue();

            if (queue.Any()) return;

            _processQueue.Remove(process.TotalExecutionTime);
        }

        protected override void AddImpl(Process process)
        {
            var executionTime = process.TotalExecutionTime;
            if (_processQueue.ContainsKey(executionTime) is false)
            {
                _processQueue[executionTime] = new Queue<Process>();
            }

            _processQueue[executionTime].Enqueue(process);
        }

        protected override IEnumerable<Process> GetAllProcesses()
        {
            return _processQueue.SelectMany(kvp => kvp.Value.ToList());
        }
    }
}
