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
        public string Employee_ID { get; set; }
        public System.DateTime Rating_Start_Date { get; set; }
        public System.DateTime Rating_End_Date { get; set; }
        public string Rating1 { get; set; }
        public string Day { get; set; }
        public int Shift_Id { get; set; }
        public bool Shift_Approved { get; set; }

        //המרת דירוג מסוג המסד לסוג המחלקה
        public static RatingEntity ConvertDBToEntity(Rating r)
        {
            return new RatingEntity () {Employee_ID =r.Employee_ID , Rating_Start_Date=r.Rating_Start_Date , Rating_End_Date =r.Rating_End_Date , Rating1=r.Rating1 , Day=r.Day , Shift_Id=r.Shift_Id , Shift_Approved =r.Shift_Approved };
        }

        //המרת דירוג מסוג המחלקה לסוג המסד
        public static Rating ConvertEntityToDB(RatingEntity r)
        {
            return new Rating() { Employee_ID = r.Employee_ID, Rating_Start_Date = r.Rating_Start_Date, Rating_End_Date = r.Rating_End_Date, Rating1 = r.Rating1, Day = r.Day, Shift_Id = r.Shift_Id, Shift_Approved = r.Shift_Approved };
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
