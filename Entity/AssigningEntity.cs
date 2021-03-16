using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace Entity
{
    public class AssigningEntity
    {
        public int shift_in_day_id { get; set; }
        public string employee_id { get; set; }
        public int department_id { get; set; }

        //המרת שיבוץ  מסוג המסד לסוג המחלקה

        public static AssigningEntity ConvertDBToEntity(Assigning b)
        {
            return new AssigningEntity() { employee_id = b.Employee_ID, department_id = b.Department_ID, shift_in_day_id = b.Shift_In_Day_ID };
        }


        //המרת שיבוץ  מסוג המחלקה לסוג המסד
        public static Assigning ConvertEntityToDB(AssigningEntity b)
        {
            return new Assigning() { Employee_ID = b.employee_id, Shift_In_Day_ID = b.shift_in_day_id, Department_ID = b.department_id };
        }

        //המרת רשימה מסוג המסד לרשימה מסוג המחלקה
        public static List<AssigningEntity> ConvertListDBToListEntity(List<Assigning> l)
        {
            List<AssigningEntity> l_assigning = new List<AssigningEntity>();
            foreach (var item in l)
            {
                l_assigning.Add(ConvertDBToEntity(item));
            }
            return l_assigning;
        }

        //המרת רשימה מסוג המחלקה לרשימה מסוג המסד
        public static List<Assigning> ConvertListEntityToListDB(List<AssigningEntity> l)
        {
            List<Assigning> l_assignings = new List<Assigning>();
            foreach (var item in l)
            {
                l_assignings.Add(ConvertEntityToDB(item));
            }
            return l_assignings;
        }

    }
}
