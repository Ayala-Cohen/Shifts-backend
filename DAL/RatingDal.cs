using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class RatingDal
    {
        static ShiftEntities db = new ShiftEntities();
        //פונקציה לשליפת דירוג בודד על פי קוד
        public static Rating GetRatingById(string e_id, int s_in_day)
        {
            try
            {
                Rating r = db.Rating.First(x => x.Employee_ID == e_id && x.Shift_In_Day == s_in_day);
                return r;
            }
            catch
            {
                return null;
            }

        }
        //פונקציה לשליפת רשימת דירוגים של עובד מסוים
        public static List<Rating> GetAllRatingOfEmployee(string employee_id)
        {
            try
            {
                List<Rating> l_rating = db.Rating.Where(x => x.Employee_ID == employee_id).ToList();
                return l_rating;
            }
            catch
            {
                return null;
            }

        }
        //פונקציה לשליפת כל הדירוגים
        public static List<Rating> GetAllRatings()
        {
            try
            {
                return db.Rating.ToList();
            }
            catch (Exception)
            {
                return null;    
            }
        }

        //פונקציה למחיקת דירוג
        public static List<Rating> DeleteRating(string e_id, int s_in_day)
        {
            try
            {
                Rating rating_for_deleting = db.Rating.First(x => x.Employee_ID == e_id && x.Shift_In_Day == s_in_day);
                db.Rating.Remove(rating_for_deleting);
                db.SaveChanges();
                return GetAllRatingOfEmployee(e_id);
            }
            catch
            {
                return null;
            }
        }
        //פונקציה לעדכון דירוג
        public static List<Rating> UpdateRating(Rating r)
        {
            try
            {
                Rating rating_for_updating = db.Rating.First(x => x.Employee_ID == r.Employee_ID);
                rating_for_updating.Shift_Id = r.Shift_Id;
                rating_for_updating.Shift_Approved = r.Shift_Approved;
                rating_for_updating.Rating1 = r.Rating1;
                db.SaveChanges();
                return GetAllRatingOfEmployee(r.Employee_ID);
            }
            catch
            {
                return null;
            }
        }


        //פונקציה להוספת דירוג
        public static List<Rating> AddRating(Rating r, string day)
        {
            try
            {
                int s_in_day_id = ShiftsDal.GetShiftInDayId(r.Shift_Id, day);
                r.Shift_In_Day = s_in_day_id;
                db.Rating.Add(r);
                db.SaveChanges();
                return GetAllRatingOfEmployee(r.Employee_ID);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }
    }
}
