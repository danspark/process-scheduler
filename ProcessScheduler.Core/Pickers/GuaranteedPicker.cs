﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessScheduler.Core.Pickers
{
    public class GuaranteedPicker : ProcessPicker
    {
        private readonly List<Process> _processes = new List<Process>();

        public TimeSpan Quantum { get; }

        public GuaranteedPicker(TimeSpan quantum)
        {
            Quantum = quantum;
        }

        public override IEnumerable<Process> GetAllProcesses()
        {
            return _processes;
        }

        public override ProcessToken? GetNext()
        {
            SortByExecutions();

            return new ProcessToken(this, _processes.First(), Quantum);
        }

        public override void RemoveProcess(Process process)
        {
            _processes.Remove(process);
        }

        protected override void AddImpl(Process process)
        {
            _processes.Add(process);

            SortByExecutions();
        }

        private void SortByExecutions()
        {
            _processes.Sort((a, b) => a.Executions.CompareTo(b.Executions));
        }
    }
}
