using System;
using System.Collections.Generic;
using System.Text;

namespace Quartz.SimpleConsoleProject.Jobs
{
    public class CustomJob
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Type JobType { get; set; }
        public ICustomJob Instance { get; set; }
    }
}
