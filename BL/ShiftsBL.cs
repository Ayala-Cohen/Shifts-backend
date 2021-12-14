using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            return ShiftsEntity.ConvertDBToEntity(ShiftsDal.GetShiftById(id));
        }
        //פונקציה לשליפת משמרת ליום
        public static int GetShiftInDayId(int shift_id, string day)
        {
            return ShiftsDal.GetShiftInDayId(shift_id, day);
        }

        //פונקציה לשליפת רשימת משמרות    
        public static List<ShiftsEntity> GetAllShifts(int business_id)
        {
            return ShiftsEntity.ConvertListDBToListEntity(ShiftsDal.GetAllShifts(business_id));
        }
        //פונקציה להחזרת רשימת משמרות ליום
        public static List<Shift_In_DayEntity> GetAllShiftsForDay(int business_id)
        {
            return Shift_In_DayEntity.ConvertListDBToListEntity(ShiftsDal.GetAllShiftsForDay(business_id));
        }

        //פונקציה למחיקת משמרת ליום
        public static void DeleteShiftInDay(int shift_in_day_id)
        {
            ShiftsDal.DeleteShiftInDay(shift_in_day_id);
        }
        //פונקציה למחיקת משמרת
        public static List<ShiftsEntity> DeleteShift(int id)
        {
            return ShiftsEntity.ConvertListDBToListEntity(ShiftsDal.DeleteShift(id));
        }
        //פונקציה לעדכון משמרת
        public static List<ShiftsEntity> UpdateShift(ShiftsEntity s)
        {
            var s_db = ShiftsEntity.ConvertEntityToDB(s);
            return ShiftsEntity.ConvertListDBToListEntity(ShiftsDal.UpdateShift(s_db));
        }


        //פונקציה להוספת משמרת
        public static List<ShiftsEntity> AddShift(ShiftsEntity s)
        {
            var s_db = ShiftsEntity.ConvertEntityToDB(s);
            return ShiftsEntity.ConvertListDBToListEntity(ShiftsDal.AddShift(s_db));
        }
    }
}
