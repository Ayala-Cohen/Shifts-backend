using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Shift_In_DayEntity
    {
        public int shift_id { get; set; }
        public string day   { get; set; }
        public int id { get; set; }

        //המרת משמרת בודדת מסוג המסד לסוג המחלקה
        public static Shift_In_DayEntity ConvertDBToEntity(Shifts_In_Days s)
        {
            return new Shift_In_DayEntity() { id = s.ID, shift_id = s.Shift_ID, day = s.Day };
        }

        //המרת משמרת בודדת מסוג המחלקה לסוג המסד
        public static Shifts_In_Days ConvertEntityToDB(Shift_In_DayEntity s)
        {
            return new Shifts_In_Days() { ID = s.id, Shift_ID = s.shift_id, Day = s.day };
        }


        //המרת רשימה מסוג המסד לרשימה מסוג המחלקה
        public static List<Shift_In_DayEntity> ConvertListDBToListEntity(List<Shifts_In_Days> l)
        {
            List<Shift_In_DayEntity> s_Shifts = new List<Shift_In_DayEntity>();
            foreach (var item in l)
            {
                s_Shifts.Add(ConvertDBToEntity(item));
            }
            return s_Shifts;
        }

        //המרת רשימה מסוג המחלקה לרשימה מסוג המסד
        public static List<Shifts_In_Days> ConvertListEntityToListDB(List<Shift_In_DayEntity> l)
        {
            List<Shifts_In_Days> s_Shifts = new List<Shifts_In_Days>();
            foreach (var item in l)
            {
                s_Shifts.Add(ConvertEntityToDB(item));
            }
            return s_Shifts;
        }
    }
}
