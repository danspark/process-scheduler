using System;

namespace ProcessScheduler.Core
{
    public abstract class ProcessPicker
    {
        public abstract bool HasProcesses();

        public abstract ProcessToken GetNextProcess();

        public abstract void RemoveProcess(Process process);

        public abstract void AddProcess(Process process, ProcessManager manager);
    }

    public class ProcessToken : IDisposable
    {
        private readonly ProcessPicker _picker;

        public Process Process { get; }

        internal ProcessToken(ProcessPicker picker, Process process)
        {
            _picker = picker;
            Process = process;
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
