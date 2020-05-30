using System;
using System.Collections.Generic;
using System.Text;

namespace Quartz.SimpleConsoleProject.Jobs
{
    public class EmailJob : ICustomJob
    {
        public string Name => "EmailJob";

        public int Id => 1;

        public void Execute(UserJob userJob)
        {
            Console.WriteLine($"{userJob.UserName} Email Gönderme işlemi tamamlandı {DateTime.Now}");
        }
    }
}
