using System;
using System.Collections.Generic;

namespace ProcessScheduler.Core
{
    public class DefaultProcessScheduler
    {
        public void Run(IEnumerable<Process> processes, ProcessPicker picker, ProcessManager manager)
        {
            AddProcesses(processes, picker, manager);

            TimeSpan time = TimeSpan.Zero;

            while (picker.HasProcesses())
            {
                using var token = picker.GetNext();

                if (token is null)
                {
                    time = manager.WaitForNextEvent();

                    continue;
                }

                time += manager.ExecuteProcess(token.Process, time, token.Duration);
            }
        }

        private void AddProcesses(IEnumerable<Process> processes, ProcessPicker picker, ProcessManager manager)
        {
            foreach (var process in processes) picker.AddProcess(process, manager);
        }
    }
}
