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
            ShiftsEntity s = ShiftsEntity.ConvertDBToEntity(ConnectDB.entity.Shifts.First(x => x.ID == id));
            return s;
        }
        //פונקציה לשליפת רשימת משמרות    
        public static List<ShiftsEntity> GetAllShifts(int business_id)
        {
            List<ShiftsEntity> l_shifts = ShiftsEntity.ConvertListDBToListEntity(ConnectDB.entity.Shifts.Where(x=>x.Business_Id == business_id).ToList());
            return l_shifts;
        }

        //פונקציה למחיקת משמרת
        public static List<ShiftsEntity> DeleteShift(int id)
        {
            //מחיקה של כל הנתונים המקושרים לשדה זה 
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
                ConnectDB.entity.Shifts.Add(ShiftsEntity.ConvertEntityToDB(s));
                ConnectDB.entity.SaveChanges();
            }
            catch { }

            return ShiftsEntity.ConvertListDBToListEntity(ConnectDB.entity.Shifts.Where(x=>x.Business_Id==s.business_id).ToList());
        }

    }
}
