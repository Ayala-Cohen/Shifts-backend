using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
namespace Entity
{
    public class ConstraintsEntity
    {
        public string employee_id { get; set; }
        public int shift_id { get; set; }
        public string day { get; set; }


        //המרת אילוץ מסוג המסד לסוג המחלקה

        public static ConstraintsEntity ConvertDBToEntity(Constraints c)
        {
            return new ConstraintsEntity() { employee_id =c.Employee_Id , shift_id =c.Shift_ID , day =c.Day  };
        }


        //המרת אילוץ מסוג המחלקה לסוג המסד
        public static Constraints  ConvertEntityToDB(ConstraintsEntity c)
        {
            return new Constraints () { Employee_Id = c.employee_id, Shift_ID = c.shift_id, Day = c.day }; 
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

