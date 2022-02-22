using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Entity;

namespace BL
{
    public class RatingBL
    {
        public enum Ratings { Prefere, Can, NotPrefere, CanNot}
        
        //פונקציה לשליפת דירוג בודד על פי קוד
        public static RatingEntity GetRatingById(string e_id, int s_in_day)
        {
            return RatingEntity.ConvertDBToEntity(RatingDal.GetRatingById(e_id, s_in_day));
        }
        //פונקציה לשליפת רשימת דירוגים של עובד מסוים
        public static List<RatingEntity> GetAllRatingOfEmployee(string employee_id)
        {
            return RatingEntity.ConvertListDBToListEntity(RatingDal.GetAllRatingOfEmployee(employee_id));
        }
        //פונקציה לשליפת כל הדירוגים
        public static List<RatingEntity> GetAllRatings()
        {
            return RatingEntity.ConvertListDBToListEntity(RatingDal.GetAllRatings());
        }

        //פונקציה למחיקת דירוג
        public static List<RatingEntity> DeleteRating(string e_id, int s_in_day)
        {
            return RatingEntity.ConvertListDBToListEntity(RatingDal.DeleteRating(e_id, s_in_day));
        }
        //פונקציה לעדכון דירוג
        public static List<RatingEntity> UpdateRating(RatingEntity r)
        {
            var r_db = RatingEntity.ConvertEntityToDB(r);
            return RatingEntity.ConvertListDBToListEntity(RatingDal.UpdateRating(r_db));
        }


        //פונקציה להוספת דירוג
        public static List<RatingEntity> AddRating(RatingEntity r, string day)
        {
            var r_db = RatingEntity.ConvertEntityToDB(r);
            return RatingEntity.ConvertListDBToListEntity(RatingDal.AddRating(r_db, day));
        }

        //פונקציה לקביעת דירוג סטטיטי עבור עובד
        public static List<Satisfaction_StatusEntity> SetStatus()
        {
            try
            {
                Satisfaction_StatusEntity s = new Satisfaction_StatusEntity();
                foreach (var item in ConnectDB.entity.Rating.ToList())
                {
                    s.employee_id = item.Employee_ID;
                    switch (item.Rating1)
                    {
                        case "לא יכול":
                            if (item.Shift_Approved == true)
                                s.satisfaction_status = 4;
                            else
                                s.satisfaction_status = 1;
                            break;
                        case "מעדיף שלא":
                            if (item.Shift_Approved == true)
                                s.satisfaction_status = 4;
                            else
                                s.satisfaction_status = 1;
                            break;
                        case "יכול":
                            if (item.Shift_Approved == true)
                                s.satisfaction_status = 1;
                            else
                                s.satisfaction_status = 4;
                            break;
                        case "מעדיף":
                            if (item.Shift_Approved == true)
                                s.satisfaction_status = 1;
                            else
                                s.satisfaction_status = 4;
                            break;
                    }
                    ConnectDB.entity.Satisfaction_Status.Add(Satisfaction_StatusEntity.ConvertEntityToDB(s));
                }
                ConnectDB.entity.SaveChanges();
                return Satisfaction_StatusEntity.ConvertListDBToListEntity(ConnectDB.entity.Satisfaction_Status.ToList());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        //פונקציה לעדכון מילון שביעות רצון (מקומי) במהלך תהליך השיבוץ
        public static void UpdateStatusLocal(int shift_in_day_id, string employee_id = "", int level_to_update = 0)
        {
            //if (employee_id != "" && level_to_update != 0)\
                foreach (var rating_level in AssigningBL.dic_shift_rating[shift_in_day_id])//מעבר על מילון הדירוגים במשמרת שהתקבלה
                {
                    foreach (var rating in rating_level.Value)//מעבר על רשימת העובדים שדירגו משמרת זו בדירוג ספציפי זה
                    {
                        if (rating_level.Key == "מעדיף" || rating_level.Key == "יכול")
                        {
                            if (rating.shift_approved == true)
                                AssigningBL.dic_of_satisfaction[rating.employee_id][1]++;
                            else
                                AssigningBL.dic_of_satisfaction[rating.employee_id][4]++;
                        }
                        else
                        {
                            if (rating.shift_approved == true)
                                AssigningBL.dic_of_satisfaction[rating.employee_id][4]++;
                            else
                                AssigningBL.dic_of_satisfaction[rating.employee_id][1]++;
                        }
                    }
                }
        }
    }
}
