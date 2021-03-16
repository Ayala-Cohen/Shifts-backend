using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace Entity
{
    public class Satisfaction_StatusEntity
    {
        public string employee_id { get; set; }
        public int satisfaction_status { get; set; }
        public int id { get; set; }

        //המרה מסוג המסד לסוג המחלקה

        public static Satisfaction_StatusEntity ConvertDBToEntity(Satisfaction_Status s)
        {
            return new Satisfaction_StatusEntity() { employee_id = s.Employee_ID, satisfaction_status = s.Satisfaction_Status1};
        }


        //המרה מסוג המחלקה לסוג המסד
        public static Satisfaction_Status ConvertEntityToDB(Satisfaction_StatusEntity s)
        {
            return new Satisfaction_Status() { Employee_ID = s.employee_id,  Satisfaction_Status1 = s.satisfaction_status };
        }

        //המרת רשימה מסוג המסד לרשימה מסוג המחלקה
        public static List<Satisfaction_StatusEntity> ConvertListDBToListEntity(List<Satisfaction_Status> l)
        {
            List<Satisfaction_StatusEntity> l_statisfation = new List<Satisfaction_StatusEntity>();
            foreach (var item in l)
            {
                l_statisfation.Add(ConvertDBToEntity(item));
            }
            return l_statisfation;
        }

        //המרת רשימה מסוג המחלקה לרשימה מסוג המסד
        public static List<Satisfaction_Status> ConvertListEntityToListDB(List<Satisfaction_StatusEntity> l)
        {
            List<Satisfaction_Status> l_statisfation = new List<Satisfaction_Status>();
            foreach (var item in l)
            {
                l_statisfation.Add(ConvertEntityToDB(item));
            }
            return l_statisfation;
        }
    }
}
