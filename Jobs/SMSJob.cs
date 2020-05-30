using System;
using System.Collections.Generic;
using System.Text;

namespace Quartz.SimpleConsoleProject.Jobs
{
    public class SMSJob : ICustomJob
    {
        public string Name => "SMSJob";

        public int Id => 2;

        public void Execute(UserJob userJob)
        {
            Console.WriteLine($"{userJob.UserName} SMS Gönderme işlemi tamamlandı {DateTime.Now}");
        }
    }
}
