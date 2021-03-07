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
        public string rating { get; set; }
        public int shift_id { get; set; }
        public bool shift_approved { get; set; }
        public int shift_in_day { get; set; }


        //המרת דירוג מסוג המסד לסוג המחלקה
        public static RatingEntity ConvertDBToEntity(Rating r)
        {
            return new RatingEntity () {employee_id =r.Employee_ID ,  rating=r.Rating1 , shift_id=r.Shift_Id , shift_approved =r.Shift_Approved, shift_in_day =r.Shift_In_Day};
        }

        //המרת דירוג מסוג המחלקה לסוג המסד
        public static Rating ConvertEntityToDB(RatingEntity r)
        {
            return new Rating() { Employee_ID = r.employee_id, Rating1 = r.rating, Shift_Id = r.shift_id, Shift_Approved = r.shift_approved, Shift_In_Day = r.shift_in_day };
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
