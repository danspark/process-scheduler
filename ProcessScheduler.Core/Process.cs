using System;

namespace ProcessScheduler.Core
{
    public class Process// (int Id, string Name, string User, TimeSpan SubmissionTime, TimeSpan TotalExecutionTime)
    {
        public int Id { get; init; }

        public string Name { get; init; } = "";

        public string User { get; init; } = "";

        public TimeSpan SubmissionTime { get; init; }

        public TimeSpan CurrentExecutionTime { get; private set; }

        public TimeSpan TotalExecutionTime { get; init; }

        public bool IsCompleted() => CurrentExecutionTime == TotalExecutionTime;

        public TimeSpan ExecuteFor(TimeSpan duration)
        {
            var executionTime = CurrentExecutionTime + duration;

            if (executionTime > TotalExecutionTime)
            {
                duration = TotalExecutionTime - executionTime;
                
                executionTime = TotalExecutionTime;
            }

            CurrentExecutionTime = executionTime;

            return duration;
        }
    }
}
