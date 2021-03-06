using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
namespace Entity
{
    public class ShiftsEntity
    {
        public int id { get; set; }
        public int business_id { get; set; }
        public string name { get; set; }

        //המרת משמרת בודדת מסוג המסד לסוג המחלקה
        public static ShiftsEntity ConvertDBToEntity(Shifts s)
        {
            return new ShiftsEntity() { id =s.ID , business_id = s.Business_Id, name= s.Name };
        }

        //המרת משמרת בודדת מסוג המחלקה לסוג המסד
        public static Shifts ConvertEntityToDB(ShiftsEntity s)
        {
            return new Shifts() { ID = s.id, Business_Id = s.business_id, Name = s.name };
        }


        //המרת רשימה מסוג המסד לרשימה מסוג המחלקה
        public static List<ShiftsEntity> ConvertListDBToListEntity(List<Shifts> l)
        {
            List<ShiftsEntity> s_Shifts = new List<ShiftsEntity>();
            foreach (var item in l)
            {
                s_Shifts.Add(ConvertDBToEntity(item));
            }
            return s_Shifts;
        }

        //המרת רשימה מסוג המחלקה לרשימה מסוג המסד
        public static List<Shifts> ConvertListEntityToListDB(List<ShiftsEntity> l)
        {
            List<Shifts> s_Shifts = new List<Shifts>();
            foreach (var item in l)
            {
                s_Shifts.Add(ConvertEntityToDB(item));
            }
            return s_Shifts;
        }
    }
}
