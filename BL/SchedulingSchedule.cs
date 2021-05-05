using DAL;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public static class SchedulingSchedule
    {
        static SchedulingSchedule()
        {

            // scheduleAsync();

        }

        public static async Task scheduleAsync()
        {

            // Grab the Scheduler instance from the Factory
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            // and start it off
            await scheduler.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<AssigningJob>()
                .WithIdentity("job1", "group1")
                .Build();

            // Trigger the job to run now, and then repeat every 10 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
.WithSchedule(CronScheduleBuilder
    .DailyAtHourAndMinute(0, 0))
.Build();


            await scheduler.ScheduleJob(job, trigger);

        }
    }






    public class AssigningJob : IJob
    {
        //static int nextTime = 0;
        public async Task Execute(IJobExecutionContext context)
        {
            ConnectDB.entity.Business.ToList().ForEach(b =>
        {
            if (newAssigningNeeded(b.ID))
                AssigningBL.AssigningActivity(b.ID);
        });

        }

        public bool newAssigningNeeded(int code)
        {
            var department = ConnectDB.entity.Departments.FirstOrDefault(d => d.Business_Id == code);
            if (department == null)
            {
                return false;
            }
            double days = (department.Diary_Closing_Day - department.Diary_Opening_Day).TotalDays;
            var date = BusinessBL.GetBusinessById(code).LastAssigningDate;
            if (date == null) return true;
            double daysFromLastAssigning = (DateTime.Today - date).Value.TotalDays;
            return days == daysFromLastAssigning;
        }
    }
}


