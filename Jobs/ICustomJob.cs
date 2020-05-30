using System;
using System.Collections.Generic;
using System.Text;

namespace Quartz.SimpleConsoleProject.Jobs
{
    public interface  ICustomJob
    {
        void Execute(UserJob userJob);

        string Name { get; }

        int Id { get; }
    }
}
