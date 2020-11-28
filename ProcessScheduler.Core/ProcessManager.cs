using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessScheduler.Core
{
    public class ProcessManager
    {
        private readonly Dictionary<TimeSpan, List<Action<Process?>>> _events = new();

        public event ProcessExecutionStarted? ProcessExecutionStarted;

        public event ProcessExecutionStopped? ProcessExecutionStopped;

        public event ProcessCompleted? ProcessCompleted;

        public void Run(IEnumerable<Process> processes, ProcessPicker picker)
        {
            AddProcesses(processes, picker);

            TimeSpan time = TimeSpan.Zero;

            while (picker.HasProcesses())
            {
                using var token = picker.GetNext();

                if (token is null)
                {
                    time = WaitForNextEvent();

                    continue;
                }

                time += ExecuteProcess(token.Process, picker, time, token.Duration);
            }
        }

        private TimeSpan ExecuteProcess(Process process, ProcessPicker picker, TimeSpan currentTime, TimeSpan duration)
        {
            ProcessExecutionStarted?.Invoke(new(process, currentTime, duration, picker.GetAllProcesses()));

            TimeSpan initialTime = currentTime;

            currentTime += process.ExecuteFor(duration);

            TriggerRegisteredEvents(initialTime, currentTime, process);

            ProcessExecutionStopped?.Invoke(new(process, currentTime, duration, picker.GetAllProcesses()));

            if (process.IsCompleted())
            {
                ProcessCompleted?.Invoke(new(process, currentTime, duration, picker.GetAllProcesses()));
            }

            return currentTime - initialTime;
        }

        internal TimeSpan WaitForNextEvent()
        {
            var (time, _) = _events.OrderBy(kvp => kvp.Key).FirstOrDefault();

            TriggerRegisteredEvents(time, time, null);

            return time;
        }

        private void TriggerRegisteredEvents(TimeSpan initialTime, TimeSpan finalTime, Process? currentProcess)
        {
            var events = _events.Where(kvp => kvp.Key >= initialTime && kvp.Key <= finalTime)
                .OrderBy(kvp => kvp.Key)
                .ToList();

            foreach (var (time, actions) in events)
            {
                foreach (var action in actions)
                {
                    action(currentProcess);
                }

                _events.Remove(time);
            }
        }

        public void RegisterEvent(TimeSpan time, Action<Process?> action)
        {
            if (_events.ContainsKey(time) is false)
            {
                _events[time] = new List<Action<Process?>>();
            }

            _events[time].Add(action);
        }

        private void AddProcesses(IEnumerable<Process> processes, ProcessPicker picker)
        {
            foreach (var process in processes) picker.AddProcess(process, this);
        }
    }
}
