using Quartz.SimpleConsoleProject.Jobs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quartz.SimpleConsoleProject.ExecuteJob
{
    public class ExecuteJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var dataMap = context.MergedJobDataMap;

                var Plugin = (ICustomJob)dataMap.Get("Plugin");

                var userJob = (UserJob)dataMap.Get("UserJob");

                Plugin.Execute(userJob);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                //Mevcut çalışan Trigger'ı context içerisinden yakalıyoruz.
                var currentTrigger = context.Trigger;

                //İş parçacığı tetiklenmesinde bir hata ile karşılaşılırsa Trigger tetiklenmesini durduruyoruz.
                context.Scheduler.PauseTrigger(currentTrigger.Key);

                Console.WriteLine($"{currentTrigger.Key} durduruldu");

                return Task.FromResult(TaskStatus.Faulted);
            }

        }
    }



    public class GlobalJobListener : IJobListener
    {
        public string Name => "SchedulerListener";

        public async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("{0} Job'ı {1} zamanında   ({2})  reddedildi", Name, DateTime.Now, context.JobDetail.Key);
        }

        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("{0} Job'ı {1} zamanında  ({2})  Çalıştırılacak", Name, DateTime.Now, context.JobDetail.Key);
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("{0} Job'ı {1} zamanında  ({2})  Çalıştırıldı", Name, DateTime.Now, context.JobDetail.Key);
        }
    }
}
