using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Entity;

namespace BL
{
    public class ShiftsBL
    {
        //פונקציה לשליפת משמרת בודדת על פי קוד
        public static ShiftsEntity GetShiftById(int id)
        {
            Shifts s = ConnectDB.entity.Shifts.First(x => x.ID == id);
            if (s!= null)
                return ShiftsEntity.ConvertDBToEntity(s);
            return null;
        }
        //פונקציה לשליפת רשימת משמרות    
        public static List<ShiftsEntity> GetAllShifts(int business_id)
        {
            List<Shifts> l_shifts = ConnectDB.entity.Shifts.Where(x => x.Business_Id == business_id).ToList();
            if(l_shifts != null)
                return ShiftsEntity.ConvertListDBToListEntity(l_shifts);
            return null;
        }

        //פונקציה למחיקת משמרת
        public static List<ShiftsEntity> DeleteShift(int id)
        {
            //מחיקה של כל הנתונים המקושרים לשדה זה 
            foreach (var item in ConnectDB.entity.Shifts_In_Days)
            {
                ConnectDB.entity.Shifts_In_Days.Remove(item);
            }
            foreach (var item in ConnectDB.entity.Constraints.Where(x=>x.Shift_Id == id))
            {
                ConnectDB.entity.Constraints.Remove(item);
            }
            foreach (var item in ConnectDB.entity.Shift_Employees.Where(x=>x.Shift_ID == id))
            {
                ConnectDB.entity.Shift_Employees.Remove(item);
            }
            foreach (var item in ConnectDB.entity.Rating.Where(x=>x.Shift_Id==id))
            {
                ConnectDB.entity.Rating.Remove(item);
            }
            Shifts shift_for_deleting = ConnectDB.entity.Shifts.First(x => x.ID == id);
            int business_id = shift_for_deleting.Business_Id;
            ConnectDB.entity.Shifts.Remove(shift_for_deleting);
            return ShiftsEntity.ConvertListDBToListEntity(ConnectDB.entity.Shifts.Where(x=>x.Business_Id == business_id).ToList());
        }
        //פונקציה לעדכון משמרת
        public static List<ShiftsEntity> UpdateShift(ShiftsEntity s)
        {
            Shifts shift_for_updating = ConnectDB.entity.Shifts.First(x => x.ID == s.id);
            shift_for_updating.Name = s.name;
            ConnectDB.entity.SaveChanges();
            return ShiftsEntity.ConvertListDBToListEntity(ConnectDB.entity.Shifts.Where(x=>x.Business_Id == s.business_id).ToList());
        }


        //פונקציה להוספת משמרת
        public static List<ShiftsEntity> AddShift(ShiftsEntity s)
        {
            try
            {
                //הוספה לטבלת משמרות וימים
                List<string> days = new List<string>() { "ראשון", "שני", "שלישי", "רביעי", "חמישי", "שישי", "שבת" };
                foreach (var item in days)
                {
                    ConnectDB.entity.Shifts_In_Days.Add(Shift_In_DayEntity.ConvertEntityToDB(new Shift_In_DayEntity() { shift_id = s.id, day = item }));
                }
                //הוספה לטבלת משמרות
                ConnectDB.entity.Shifts.Add(ShiftsEntity.ConvertEntityToDB(s));
                ConnectDB.entity.SaveChanges();
            }
            catch { }

            return ShiftsEntity.ConvertListDBToListEntity(ConnectDB.entity.Shifts.Where(x=>x.Business_Id==s.business_id).ToList());
        }

    }
}
