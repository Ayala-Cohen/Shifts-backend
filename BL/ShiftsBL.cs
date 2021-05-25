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
            try
            {
                Shifts s = ConnectDB.entity.Shifts.First(x => x.ID == id);
                return ShiftsEntity.ConvertDBToEntity(s);
            }
            catch
            {
                return null;
            }
        }
        //פונקציה לשליפת משמרת ליום
        public static int GetShiftInDayId(int shift_id, string day)
        {
            try
            {
                var shift_in_day = ConnectDB.entity.Shifts_In_Days.FirstOrDefault(x => x.Shift_ID == shift_id && x.Day == day);
                if (shift_in_day != null)
                    return shift_in_day.ID;
            }
            catch (Exception)
            {
            }
            return 0;
        }

        //פונקציה לשליפת רשימת משמרות    
        public static List<ShiftsEntity> GetAllShifts(int business_id)
        {
            try
            {
                List<Shifts> l_shifts = ConnectDB.entity.Shifts.Where(x => x.Business_Id == business_id).ToList();
                return ShiftsEntity.ConvertListDBToListEntity(l_shifts);
            }
            catch (Exception)
            {
                return null;

            }

        }
        //פונקציה להחזרת רשימת משמרות ליום
        public static List<Shift_In_DayEntity> GetAllShiftsForDay(int business_id)
        {
            try
            {
                List<Shifts_In_Days> l = ConnectDB.entity.Shifts_In_Days.ToList();
                List<ShiftsEntity> l_shifts = GetAllShifts(business_id);

                l = l.Where(x => l_shifts.Any(y => y.id == x.Shift_ID) == true).ToList();
                return Shift_In_DayEntity.ConvertListDBToListEntity(l);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        public static void DeleteShiftInDay(int shift_in_day_id)
        {
            foreach (var item in ConnectDB.entity.Rating.Where(x=>x.Shift_In_Day == shift_in_day_id))
            {
                RatingBL.DeleteRating(item.Employee_ID, item.Shift_In_Day);
            }
            foreach (var item in ConnectDB.entity.Assigning.Where(x=>x.Shift_In_Day_ID == shift_in_day_id))
            {
                ConnectDB.entity.Assigning.Remove(item);
            }
            ConnectDB.entity.SaveChanges();
        }
        //פונקציה למחיקת משמרת
        public static List<ShiftsEntity> DeleteShift(int id)
        {
            try
            {
                //מחיקה של כל הנתונים המקושרים לשדה זה 
                foreach (var item in ConnectDB.entity.Shifts_In_Days)
                {
                    DeleteShiftInDay(item.ID);
                }
                foreach (var item in ConnectDB.entity.Constraints.Where(x => x.Shift_ID == id))
                {
                    ConstraintsBL.DeleteConstraint(item.Shift_ID, item.Day, item.Employee_Id);
                }
                foreach (var item in ConnectDB.entity.Shift_Employees.Where(x => x.Shift_ID == id))
                {
                    Shifts_EmployeesBL.DeleteEmployeeShift(item.Shift_ID, item.Role_Id, item.Day, item.Departments_Id);
                }
                foreach (var item in ConnectDB.entity.Rating.Where(x => x.Shift_Id == id))
                {
                    RatingBL.DeleteRating(item.Employee_ID, item.Shift_In_Day);
                }
                Shifts shift_for_deleting = ConnectDB.entity.Shifts.First(x => x.ID == id);
                int business_id = shift_for_deleting.Business_Id;
                ConnectDB.entity.Shifts.Remove(shift_for_deleting);
                ConnectDB.entity.SaveChanges();
                return ShiftsEntity.ConvertListDBToListEntity(ConnectDB.entity.Shifts.Where(x => x.Business_Id == business_id).ToList());
            }
            catch
            {
                return null;
            }
        }
        //פונקציה לעדכון משמרת
        public static List<ShiftsEntity> UpdateShift(ShiftsEntity s)
        {
            try
            {
                Shifts shift_for_updating = ConnectDB.entity.Shifts.First(x => x.ID == s.id);
                shift_for_updating.Name = s.name;
                ConnectDB.entity.SaveChanges();
                return ShiftsEntity.ConvertListDBToListEntity(ConnectDB.entity.Shifts.Where(x => x.Business_Id == s.business_id).ToList());
            }
            catch
            {
                return null;
            }

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
                return ShiftsEntity.ConvertListDBToListEntity(ConnectDB.entity.Shifts.Where(x => x.Business_Id == s.business_id).ToList());
            }
            catch
            {
                return null;
            }

        }

    }
}
