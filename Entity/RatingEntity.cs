using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
namespace Entity
{
    public class RatingEntity
    {
        public string employee_id { get; set; }
        public System.DateTime rating_start_date { get; set; }
        public System.DateTime rating_end_date { get; set; }
        public string rating { get; set; }
        public string day { get; set; }
        public int shift_id { get; set; }
        public bool shift_approved { get; set; }

        //המרת דירוג מסוג המסד לסוג המחלקה
        public static RatingEntity ConvertDBToEntity(Rating r)
        {
            return new RatingEntity () {employee_id =r.Employee_ID , rating_start_date=r.Rating_Start_Date , rating_end_date =r.Rating_End_Date , rating=r.Rating1 , day=r.Day , shift_id=r.Shift_Id , shift_approved =r.Shift_Approved };
        }

        //המרת דירוג מסוג המחלקה לסוג המסד
        public static Rating ConvertEntityToDB(RatingEntity r)
        {
            return new Rating() { Employee_ID = r.employee_id, Rating_Start_Date = r.rating_start_date, Rating_End_Date = r.rating_end_date, Rating1 = r.rating, Day = r.day, Shift_Id = r.shift_id, Shift_Approved = r.shift_approved };
        }



        //המרת רשימה מסוג המסד לרשימה מסוג המחלקה
        public static List<RatingEntity> ConvertListDBToListEntity(List<Rating> l)
        {
            List<RatingEntity>r_ratings = new List<RatingEntity>();
            foreach (var item in l)
            {
                r_ratings.Add(ConvertDBToEntity(item));
            }
            return r_ratings;
        }
        //המרת רשימה מסוג המחלקה לרשימה מסוג המסד
        public static List<Rating> ConvertListEntityToListDB(List<RatingEntity >l)
        {
            List<Rating> r_ratings = new List<Rating>();
            foreach (var item in l)
            {
                r_ratings.Add(ConvertEntityToDB(item));
            }
            return r_ratings;
        }
    }
}
