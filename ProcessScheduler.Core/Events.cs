using System;
using System.Collections.Generic;

namespace ProcessScheduler.Core
{
    public record ProcessExecutionEventArgs(Process Process, TimeSpan CurrentTime, TimeSpan Duration, IEnumerable<Process> Queue);

    public delegate void ProcessCreated(Process process, ProcessExecutionEventArgs args);

    public delegate void ProcessCompleted(ProcessExecutionEventArgs args);
    
    public delegate void ProcessExecutionStarted(ProcessExecutionEventArgs args);
                                                
    public delegate void ProcessExecutionStopped(ProcessExecutionEventArgs args);
}
