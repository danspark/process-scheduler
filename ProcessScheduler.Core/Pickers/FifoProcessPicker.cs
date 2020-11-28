using System;
using System.Collections.Generic;

namespace ProcessScheduler.Core.Pickers
{
    public class FifoProcessPicker : ProcessPicker
    {
        private int _incomingProcesses;
        private readonly Queue<Process> _processQueue = new();

        public event ProcessCreated? ProcessCreated;

        public override void AddProcess(Process process, ProcessManager manager)
        {
            if (process.SubmissionTime > TimeSpan.Zero)
            {
                _incomingProcesses++;

                manager.RegisterEvent(process.SubmissionTime, (p) =>
                {
                    _processQueue.Enqueue(process);

                    ProcessCreated?.Invoke(new(process, p, process.SubmissionTime));
                });
            }
            else
            {
                _processQueue.Enqueue(process);

                ProcessCreated?.Invoke(new(process, p, process.SubmissionTime));
            }
        }

        public override ProcessToken GetNextProcess()
        {
            return new ProcessToken(this, _processQueue.Dequeue());
        }

        public override bool HasProcesses() => _processQueue.Count > 0 || _incomingProcesses > 0;

        public override void RemoveProcess(Process process)
        {
            // dequeue already removes the process
        }
    }
}
