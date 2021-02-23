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
        public static RatingEntity GetRatingById(string id)
        {
            RatingEntity r = RatingEntity.ConvertDBToEntity(ConnectDB.entity.Rating.First(x => x.Employee_ID == id));
            return r;
        }
        //פונקציה לשליפת רשימת דירוגים
        public static List<RatingEntity> GetAllRating()
        {
            List<RatingEntity> l_rating = RatingEntity.ConvertListDBToListEntity(ConnectDB.entity.Rating.ToList());
            return l_rating;
        }

        //פונקציה למחיקת דירוג
        public static List<RatingEntity> DeleteRating(string id)
        {
            Rating rating_for_deleting = ConnectDB.entity.Rating.First(x => x.Employee_ID == id);
            ConnectDB.entity.Rating.Remove(rating_for_deleting);
            return RatingEntity.ConvertListDBToListEntity(ConnectDB.entity.Rating.ToList());
        }
        //פונקציה לעדכון דירוג
        public static List<RatingEntity> UpdateRating(RatingEntity r)
        {
            Rating rating_for_updating = ConnectDB.entity.Rating.First(x => x.Employee_ID == r.employee_id);
            rating_for_updating.Rating_End_Date = r.rating_end_date;
            rating_for_updating.Rating_Start_Date = r.rating_start_date;
            rating_for_updating.Shift_Id = r.shift_id;
            rating_for_updating.Shift_Approved = r.shift_approved;
            rating_for_updating.Rating1 = r.rating;
            rating_for_updating.Day = r.day;
            ConnectDB.entity.SaveChanges();
            return RatingEntity.ConvertListDBToListEntity(ConnectDB.entity.Rating.ToList());
        }


        //פונקציה להוספת דירוג
        public static List<RatingEntity> AddRating(RatingEntity r)
        {
            try
            {
                ConnectDB.entity.Rating.Add(RatingEntity.ConvertEntityToDB(r));
                ConnectDB.entity.SaveChanges();
            }
            catch { }
            return RatingEntity.ConvertListDBToListEntity(ConnectDB.entity.Rating.ToList());
        }
    }
}
