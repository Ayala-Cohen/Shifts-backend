using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
namespace Entity
{
    class ConstraintsEntity
    {
        public string Employee_Id { get; set; }
        public int Shift_Id { get; set; }
        public string Day { get; set; }


        //המרת קטגוריה בודדת מסוג המסד לסוג המחלקה

        public static ConstraintsEntity ConvertDBToEntity(Constraints c)
        {
            return new ConstraintsEntity() { Employee_Id =c.Employee_Id , Shift_Id =c.Shift_Id , Day =c.Day  };
        }


        //המרת קטגוריה בודדת מסוג המחלקה לסוג המסד
        public static Constraints  ConvertEntityToDB(ConstraintsEntity c)
        {
            return new Constraints () { Employee_Id = c.Employee_Id, Shift_Id = c.Shift_Id, Day = c.Day }; 
        }

        //המרת רשימה מסוג המסד לרשימה מסוג המחלקה
        public static List<ConstraintsEntity > ConvertListDBToListEntity(List<Constraints > l)
        {
            List<ConstraintsEntity > c_constraints = new List<ConstraintsEntity >();
            foreach (var item in l)
            {
                c_constraints.Add(ConvertDBToEntity(item));
            }
            return c_constraints;
        }

        //המרת רשימה מסוג המסד לרשימה מסוג המחלקה
        public static List<Constraints> ConvertListEntityToListDB(List<ConstraintsEntity> l)
        {
            List<Constraints> c_constraints = new List<Constraints>();
            foreach (var item in l)
            {
                c_constraints.Add(ConvertEntityToDB(item));
            }
            return c_constraints;
        }
    }



}

