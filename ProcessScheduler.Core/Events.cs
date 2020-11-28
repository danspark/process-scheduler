using System;

namespace ProcessScheduler.Core
{
    public record ProcessEventArgs(Process Process, Process ProcessInExecution, TimeSpan CurrentTime);

    public record ProcessExecutionEventArgs(Process Process, TimeSpan CurrentTime, TimeSpan Duration) 
        : ProcessEventArgs(Process, Process, CurrentTime);

    public delegate void ProcessCreated(ProcessEventArgs args);

    public delegate void ProcessCompleted(ProcessEventArgs args);
    
    public delegate void ProcessExecutionStarted(ProcessExecutionEventArgs args);
                                                
    public delegate void ProcessExecutionStopped(ProcessExecutionEventArgs args);
}
