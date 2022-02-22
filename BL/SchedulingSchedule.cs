using DAL;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static async Task ScheduleAsync()
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
    .DailyAtHourAndMinute(13, 44))
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
            if (NewAssigningNeeded(b.ID))
                AssigningBL.AssigningActivity(b.ID);
            else
            {
                if (SendUpdateNeeded(b.ID))
                {
                    EmployeesBL.SendEmailToEmployees(b.Employees.ToList(), "תזכורת למילוי שיבוץ", "מחר נסגרת האפשרות לדרג משמרות, אם עדיין לא דרגת את כל המשמרות גש לבצע זאת לפני שהיומן יסגר");
                }
            }
        });
            Debug.WriteLine("Hi I am a asynchronize job!!!");
        }


        public bool NewAssigningNeeded(int code)
        {
            var department = ConnectDB.entity.Departments.FirstOrDefault(d => d.Business_Id == code);
            if (department == null)
            {
                return false;
            }

            double days = (department.Diary_Closing_Day - department.Diary_Opening_Day).TotalDays;
            var date = BusinessBL.GetBusinessById(code).LastAssigningDate;
            if (date == null)
            {
                return (department.Diary_Opening_Day - DateTime.Today).TotalDays == 0;

            }
            double daysFromLastAssigning = (DateTime.Today - date).Value.TotalDays;
            return days == daysFromLastAssigning;
        }

        public bool SendUpdateNeeded(int business_id)
        {
            var department = ConnectDB.entity.Departments.FirstOrDefault(d => d.Business_Id == business_id);
            if (department == null)
            {
                return false;
            }
            //הפרש הימים בין פתיחה לסגירה
            double days = (department.Diary_Closing_Day - department.Diary_Opening_Day).TotalDays;
            //תאריך שיבוץ אחרון
            var date = BusinessBL.GetBusinessById(business_id).LastAssigningDate;
            //אם אין תאריך שיבוץ אחרון
            if (date == null)
            {
               return (department.Diary_Opening_Day - DateTime.Today).TotalDays == 1;
                
            }
            double daysFromLastAssigning = (DateTime.Today - date).Value.TotalDays;
            return days-1 == daysFromLastAssigning;
        }
    }
}


