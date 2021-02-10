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
        public static List<ShiftsEntity> GetAllShifts()
        {
            List<ShiftsEntity> l_shifts = ShiftsEntity.ConvertListDBToListEntity(ConnectDB.entity.Shifts.ToList());
            return l_shifts;
        }

        //פונקציה למחיקת משמרת
        public static List<ShiftsEntity> DeleteShift(int id)
        {
            //מחיקה של כל הנתונים המקושרים לשדה זה קודם
            Shifts shift_for_deleting = ConnectDB.entity.Shifts.First(x => x.ID == id);
            ConnectDB.entity.Shifts.Remove(shift_for_deleting);
            return ShiftsEntity.ConvertListDBToListEntity(ConnectDB.entity.Shifts.ToList());
        }
        //פונקציה לעדכון משמרת
        public static List<ShiftsEntity> UpdateShift(ShiftsEntity s)
        {
            Shifts shift_for_updating = ConnectDB.entity.Shifts.First(x => x.ID == s.id);
            shift_for_updating.Name = s.name;
            ConnectDB.entity.SaveChanges();
            return ShiftsEntity.ConvertListDBToListEntity(ConnectDB.entity.Shifts.ToList());
        }


        //פונקציה להוספת משמרת
        public static List<ShiftsEntity> AddShift(ShiftsEntity s)
        {
            ConnectDB.entity.Shifts.Add(ShiftsEntity.ConvertEntityToDB(s));
            ConnectDB.entity.SaveChanges();
            return ShiftsEntity.ConvertListDBToListEntity(ConnectDB.entity.Shifts.ToList());
        }

    }
}
