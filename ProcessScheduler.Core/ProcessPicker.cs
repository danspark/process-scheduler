using System;
using System.Collections.Generic;

namespace ProcessScheduler.Core
{
    public abstract class ProcessPicker
    {
        public int IncomingProcesses { get; protected set; }

        public event ProcessCreated? ProcessCreated;

        public virtual bool HasProcesses() => IncomingProcesses > 0;

        public abstract ProcessToken? GetNext();

        public abstract void RemoveProcess(Process process);

        public virtual void AddProcess(Process process, ProcessManager manager)
        {
            if (process.SubmissionTime > TimeSpan.Zero)
            {
                IncomingProcesses++;

                manager.RegisterEvent(process.SubmissionTime, (p) =>
                {
                    AddImpl(process);

                    IncomingProcesses--;

                    ProcessCreated?.Invoke(new(process, p, process.SubmissionTime), GetAllProcesses());
                });
            }
            else
            {
                AddImpl(process);

                ProcessCreated?.Invoke(new(process, null, process.SubmissionTime), GetAllProcesses());
            }
        }

        protected abstract void AddImpl(Process process);

        protected abstract IEnumerable<Process> GetAllProcesses();
    }

    public class ProcessToken : IDisposable
    {
        private readonly ProcessPicker _picker;

        public Process Process { get; }

        public TimeSpan Duration { get; }

        internal ProcessToken(ProcessPicker picker, Process process, TimeSpan duration)
        {
            _picker = picker;
            Process = process;
            Duration = duration;
        }

        public void Dispose()
        {
            if (Process.IsCompleted())
            {
                _picker.RemoveProcess(Process);
            }
        }
    }
}
