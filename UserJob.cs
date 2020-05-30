using System;
using System.Collections.Generic;
using System.Text;

namespace Quartz.SimpleConsoleProject
{
    public class UserJob
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public int JobId { get; set; }

        public string JobName { get; set; }
        public string Cron { get; set; }

        public List<UserJob> UserJobList => new List<UserJob>
        {
            new UserJob
               {
                   Id = 1,
                   UserName = "A",
                   Cron = "0 0/1 * 1/1 * ? *",
                   JobId = 1,
                   JobName = "EmailJob"
               },
                new UserJob
               {
                   Id = 2,
                   UserName = "B",
                   Cron = "0 0/1 * 1/1 * ? *",
                   JobId = 1,
                   JobName = "EmailJob"
               },
                 new UserJob
               {
                   Id = 3,
                   UserName = "C",
                   Cron = "0 0/1 * 1/1 * ? *",
                   JobId = 2,
                   JobName = "SMSJob"
               },
                  new UserJob
               {
                   Id = 4,
                   UserName = "D",
                   Cron = "0 0/1 * 1/1 * ? *",
                   JobId = 2,
                   JobName = "SMSJob"
               }
        };
    }

}
