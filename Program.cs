using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.SimpleConsoleProject.ExecuteJob;
using Quartz.SimpleConsoleProject.Jobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Quartz.SimpleConsoleProject
{
       class Program
    {
         static IScheduler scheduler;
         static async Task Main(string[] args)
        {
            #region Scheduler Context'i ayağa Kaldırma işlemi
            var schedContext = new StdSchedulerFactory();

            // Context içerisinde mevcut halde çalışan İş parçacıklarını alıyoruz.
            scheduler = await schedContext.GetScheduler();

            //Daha önce başlatılmadıysa Koşturmaya başlıyoruz.
            if (!scheduler.IsStarted)
            {
                await scheduler.Start();

                Console.WriteLine("Scheduler Çalışmaya başladı");
            }

            #endregion

            string[] filePaths = Directory.GetFiles(@"C:\Users\tayfun.bolat\source\repos\Quartz.SimpleConsoleSol\Quartz.SimpleConsoleProject\Jobs\");


            var jobs = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetInterface(nameof(ICustomJob)) != null).ToList();

           var customJobList = await CreateJobs(jobs);

           await CreateTrigger(customJobList);

            Console.ReadLine();

        }


        /// <summary>
        /// Job Oluşturulması yapılıyor.
        /// </summary>
        /// <param name="jobs"></param>
        /// <returns></returns>
        private static async Task<List<CustomJob>> CreateJobs(List<Type> jobs)
        {
            var customJob = new List<CustomJob>();

            //Şuan için 2 farklı job tanımlaması yapıldı Email ve SMS reflection ile Instance üretip JobDetail oluşturuyoruz.
            jobs.ForEach(async job =>
            {
                var jobInstance = (ICustomJob)Activator.CreateInstance(job);

                //Trigger oluştururken IJobDetail' i bulmak için kullanacağız.
                customJob.Add(new CustomJob
                {
                    Id = jobInstance.Id,
                    Instance = jobInstance,
                    JobType = job,
                    Name = job.Name
                });

                ///Her bir Job için spesifik kendine özgü bir Name (SMSJob,EMAilJob) tanımlıyoruz. Bu alanları Job için ve Job'ın Key tanımlamasında kullanıyoruz. Bensersiz olmalı 
                JobKey jobKey = new JobKey(jobInstance.Name);

                //Eğer tanımlı Job Key Scheduler içerisinde varsa aynı Job'tan oluşturmamak için kontrol yapıyoruz.
                var JobIsExist = await scheduler.CheckExists(jobKey);

                if (!JobIsExist)
                {
                    IJobDetail jobDetail = JobBuilder.Create<ExecuteJob.ExecuteJob>()
                   .WithIdentity(jobKey).Build();

                    //Jobları ekliyoruz. Şuan tetiklenen bir  Trigger yok
                    await scheduler.AddJob(jobDetail, replace: true, true, cancellationToken: CancellationToken.None);

                    //Quartz Middleware
                    GlobalJobListener globalJobListener = new GlobalJobListener();
                    scheduler.ListenerManager.AddJobListener(new GlobalJobListener(), GroupMatcher<JobKey>.AnyGroup());
                }
            });
            return customJob;
        }

        /// <summary>
        /// Trigger Oluşturuluyor. CustomJob sınıfı içerisinden çalıştırılması gereken Job'ı yakalayıp Trigger'a ForJob tanımlaması yapılıyor.
        /// </summary>
        /// <param name="customJobs"></param>
        /// <returns></returns>
        private static async Task CreateTrigger(List<CustomJob> customJobs)
        {
            var userList = new UserJob();

            userList.UserJobList.ForEach(async userJob =>
            {

                var jobKey = new JobKey(userJob.JobName);

                //Her Bir Trigger içinde özel olarak bir key tanımlaması yapılması gerekiyor.
                var TriggerKey = new TriggerKey(userJob.Id.ToString());


                var TriggerIsExist = await scheduler.CheckExists(jobKey);

                //Çalışan aktif Trigger var mı kontrol ediyoruz. Yoksa yeni bir trigger olarak context'e kaydediyoruz.
                if (TriggerIsExist)
                {
                    //Triggerla eşleşen Job'ı buluyoruz. Ve bu Job instance' ını Parametre olarak Trigger'ımıza ekliyoruz.
                   var currentJob = customJobs.FirstOrDefault(x => x.Id == userJob.JobId);

                    JobDataMap jobdataMap = new JobDataMap();
                    jobdataMap.Add("Plugin", currentJob.Instance);
                    jobdataMap.Add("UserJob", userJob);


                    ITrigger jobTrigger = TriggerBuilder.Create()
                       .WithIdentity(TriggerKey)

                       .UsingJobData(jobdataMap) //Parametre olarak eklediğimiz kısım
                       .ForJob(jobKey) // JobKey uniq tanımladığımız bir alandı  Trigger hangi Job için tetikleneceğini burdan anlıyor.
                      .WithCronSchedule(userJob.Cron)
                      .Build();

                    var result = await scheduler.ScheduleJob(jobTrigger);

                  
                }

            });
        }

    }
}

