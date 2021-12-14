using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ShiftsDal
    {
        static  ShiftEntities db = new ShiftEntities();

        //פונקציה לשליפת משמרת בודדת על פי קוד
        public static Shifts GetShiftById(int id)
        {
            try
            {
                Shifts s = db.Shifts.First(x => x.ID == id);
                return s;
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
                var shift_in_day = db.Shifts_In_Days.FirstOrDefault(x => x.Shift_ID == shift_id && x.Day == day);
                if (shift_in_day != null)
                    return shift_in_day.ID;
            }
            catch (Exception)
            {
            }
            return 0;
        }

        //פונקציה לשליפת רשימת משמרות    
        public static List<Shifts> GetAllShifts(int business_id)
        {
            try
            {
                List<Shifts> l_shifts = db.Shifts.Where(x => x.Business_Id == business_id).ToList();
                return l_shifts;
            }
            catch (Exception)
            {
                return null;

            }

        }
        //פונקציה להחזרת רשימת משמרות ליום
        public static List<Shifts_In_Days> GetAllShiftsForDay(int business_id)
        {
            try
            {
                List<Shifts_In_Days> l = db.Shifts_In_Days.ToList();
                List<Shifts> l_shifts = GetAllShifts(business_id);

                l = l.Where(x => l_shifts.Any(y => y.ID == x.Shift_ID) == true).ToList();
                return l;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        //פונקציה למחיקת משמרת ליום
        public static void DeleteShiftInDay(int shift_in_day_id)
        {
            foreach (var item in db.Rating.Where(x => x.Shift_In_Day == shift_in_day_id))
            {
                RatingDal.DeleteRating(item.Employee_ID, item.Shift_In_Day);
            }
            foreach (var item in db.Assigning.Where(x => x.Shift_In_Day_ID == shift_in_day_id))
            {
                db.Assigning.Remove(item);
            }
            db.SaveChanges();
        }
        //פונקציה למחיקת משמרת
        public static List<Shifts> DeleteShift(int id)
        {
            try
            {
                //מחיקה של כל הנתונים המקושרים לשדה זה 
                foreach (var item in db.Shifts_In_Days)
                {
                    DeleteShiftInDay(item.ID);
                }
                foreach (var item in db.Constraints.Where(x => x.Shift_ID == id))
                {
                    ConstraintsDal.DeleteConstraint(item.Shift_ID, item.Day, item.Employee_Id);
                }
                foreach (var item in db.Shift_Employees.Where(x => x.Shift_ID == id))
                {
                    Shifts_EmployeesDal.DeleteEmployeeShift(item.Shift_ID, item.Role_Id, item.Day, item.Departments_Id);
                }
                foreach (var item in db.Rating.Where(x => x.Shift_Id == id))
                {
                    RatingDal.DeleteRating(item.Employee_ID, item.Shift_In_Day);
                }
                Shifts shift_for_deleting = db.Shifts.First(x => x.ID == id);
                int business_id = shift_for_deleting.Business_Id;
                db.Shifts.Remove(shift_for_deleting);
                db.SaveChanges();
                return GetAllShifts(business_id);
            }
            catch
            {
                return null;
            }
        }
        //פונקציה לעדכון משמרת
        public static List<Shifts> UpdateShift(Shifts s)
        {
            try
            {
                Shifts shift_for_updating = db.Shifts.First(x => x.ID == s.ID);
                shift_for_updating.Name = s.Name;
                db.SaveChanges();
                return GetAllShifts(s.Business_Id);
            }
            catch
            {
                return null;
            }

        }


        //פונקציה להוספת משמרת
        public static List<Shifts> AddShift(Shifts s)
        {
            try
            {
                //הוספה לטבלת משמרות וימים
                List<string> days = new List<string>() { "ראשון", "שני", "שלישי", "רביעי", "חמישי", "שישי", "שבת" };
                foreach (var item in days)
                {
                    db.Shifts_In_Days.Add(new Shifts_In_Days() { Shift_ID = s.ID, Day = item });
                }
                //הוספה לטבלת משמרות
                db.Shifts.Add(s);
                db.SaveChanges();
                return GetAllShifts(s.Business_Id);
            }
            catch
            {
                return null;
            }

        }
    }
}
