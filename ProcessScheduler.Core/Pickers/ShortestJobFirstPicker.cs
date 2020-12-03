using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessScheduler.Core.Pickers
{
    public class ShortestJobFirstPicker : ProcessPicker
    {
        private readonly SortedList<TimeSpan, Queue<Process>> _processQueue = new();

        protected override bool HasProcessesImpl() => _processQueue.Values.Any(q => q.Any());

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

            return HasProcessesImpl() ? null : throw new InvalidOperationException("No incoming processes");
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

        public override IEnumerable<Process> GetAllProcesses()
        {
            return _processQueue.SelectMany(kvp => kvp.Value.ToList());
        }
    }
}
