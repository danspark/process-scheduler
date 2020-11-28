using System;

namespace ProcessScheduler.Core
{
    public abstract class ProcessPicker
    {
        public abstract bool HasProcesses();

        public abstract ProcessToken? GetNext();

        public abstract void RemoveProcess(Process process);

        public abstract void AddProcess(Process process, ProcessManager manager);
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
