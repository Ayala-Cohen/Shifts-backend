using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Entity;

namespace BL
{
    public class RatingBL
    {
        //פונקציה לשליפת דירוג בודד על פי קוד
        public static RatingEntity GetRatingById(string e_id, int s_in_day)
        {
            RatingEntity r = RatingEntity.ConvertDBToEntity(ConnectDB.entity.Rating.First(x => x.Employee_ID == e_id && x.Shift_In_Day == s_in_day));
            return r;
        }
        //פונקציה לשליפת רשימת דירוגים
        public static List<RatingEntity> GetAllRating()
        {
            List<RatingEntity> l_rating = RatingEntity.ConvertListDBToListEntity(ConnectDB.entity.Rating.ToList());
            return l_rating;
        }

        //פונקציה למחיקת דירוג
        public static List<RatingEntity> DeleteRating(string e_id, int s_in_day)
        {
            Rating rating_for_deleting = ConnectDB.entity.Rating.First(x => x.Employee_ID == e_id && x.Shift_In_Day == s_in_day);
            ConnectDB.entity.Rating.Remove(rating_for_deleting);
            return RatingEntity.ConvertListDBToListEntity(ConnectDB.entity.Rating.ToList());
        }
        //פונקציה לעדכון דירוג
        public static List<RatingEntity> UpdateRating(RatingEntity r)
        {
            Rating rating_for_updating = ConnectDB.entity.Rating.First(x => x.Employee_ID == r.employee_id);
            rating_for_updating.Shift_Id = r.shift_id;
            rating_for_updating.Shift_Approved = r.shift_approved;
            rating_for_updating.Rating1 = r.rating;
            ConnectDB.entity.SaveChanges();
            return RatingEntity.ConvertListDBToListEntity(ConnectDB.entity.Rating.ToList());
        }


        //פונקציה להוספת דירוג
        public static List<RatingEntity> AddRating(RatingEntity r, string day)
        {
            //try
            //{
                int s_in_day_id = ConnectDB.entity.Shifts_In_Days.FirstOrDefault(x => x.Day == day && x.Shift_ID == r.shift_id).ID;
                r.shift_in_day = s_in_day_id;
                ConnectDB.entity.Rating.Add(RatingEntity.ConvertEntityToDB(r));
                ConnectDB.entity.SaveChanges();
            //}
            //catch { }
            return RatingEntity.ConvertListDBToListEntity(ConnectDB.entity.Rating.ToList());
        }

        //פונקציה לקביעת דירוג סטטיטי עבור עובד
        //change to type entity
        public static List<Satisfaction_Status> updateStatus()
        {
            Satisfaction_Status s = new Satisfaction_Status();
            foreach (var item in ConnectDB.entity.Rating)
            {
                s.Employee_ID = item.Employee_ID;
                switch (item.Rating1)
                {
                    case "לא יכול":
                        if (item.Shift_Approved == true)
                            s.Satisfaction_Status1 = 4;
                        else
                            s.Satisfaction_Status1 = 1;
                        break;
                    case "מעדיף שלא":
                        if (item.Shift_Approved == true)
                            s.Satisfaction_Status1 = 3;
                        else
                            s.Satisfaction_Status1 = 2;
                        break;
                    case "יכול":
                        if (item.Shift_Approved == true)
                            s.Satisfaction_Status1 = 2;
                        else
                            s.Satisfaction_Status1 = 3;
                        break;
                    case "מעדיף":
                        if (item.Shift_Approved == true)
                            s.Satisfaction_Status1 = 1;
                        else
                            s.Satisfaction_Status1 = 4;
                        break;
                }
                ConnectDB.entity.Satisfaction_Status.Add(s);
                ConnectDB.entity.SaveChanges();
            }
            return ConnectDB.entity.Satisfaction_Status.ToList();
        }
    }
}
