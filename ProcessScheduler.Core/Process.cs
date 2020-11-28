using System;

namespace ProcessScheduler.Core
{
    public class Process// (int Id, string Name, string User, TimeSpan SubmissionTime, TimeSpan TotalExecutionTime)
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string User { get; set; } = "";

        public TimeSpan SubmissionTime { get; set; }

        public TimeSpan CurrentExecutionTime { get; private set; }

        public TimeSpan TotalExecutionTime { get; set; }

        public bool IsCompleted() => CurrentExecutionTime == TotalExecutionTime;

        public TimeSpan ExecuteFor(TimeSpan duration)
        {
            var executionTime = CurrentExecutionTime + duration;

            if (executionTime > TotalExecutionTime)
            {
                duration = TotalExecutionTime - CurrentExecutionTime;
                
                executionTime = TotalExecutionTime;
            }

            CurrentExecutionTime = executionTime;

            return duration;
        }

        //public Process(int id, string name, string user, TimeSpan totalExecutionTime, TimeSpan? submissionTime = null)
        //{
        //    Id = id;
        //    Name = name;
        //    User = user;
        //    TotalExecutionTime = totalExecutionTime;
        //    SubmissionTime = submissionTime ?? TimeSpan.Zero;
        //}

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}
