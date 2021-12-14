using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Entity;
using System.Diagnostics;
namespace BL
{
    public class Shifts_EmployeesBL
    {
        //פונקציה לשליפת עובד במשמרת בודד על פי קוד
        public static Shift_EmployeesEntity GetEmployeesShiftById(int s_id, int r_id, string day, int dep_id)
        {
            return Shift_EmployeesEntity.ConvertDBToEntity(Shifts_EmployeesDal.GetEmployeesShiftById(s_id, r_id, day, dep_id));
        }
        //פונקציה לשליפת רשימת עובדים במשמרות
        public static List<Shift_EmployeesEntity> GetAllEmployeesShifts(int business_id)
        {
            return Shift_EmployeesEntity.ConvertListDBToListEntity(Shifts_EmployeesDal.GetAllEmployeesShifts(business_id));
        }

        //פונקציה למחיקת עובד במשמרת
        public static List<Shift_EmployeesEntity> DeleteEmployeeShift(int s_id, int r_id, string day, int dep_id)
        {
            return Shift_EmployeesEntity.ConvertListDBToListEntity(Shifts_EmployeesDal.DeleteEmployeeShift(s_id, r_id, day, dep_id));
        }
        //פונקציה לעדכון עובד במשמרת
        public static List<Shift_EmployeesEntity> UpdateEmployeeShift(Shift_EmployeesEntity s)
        {
            var s_db = Shift_EmployeesEntity.ConvertEntityToDB(s);
            return Shift_EmployeesEntity.ConvertListDBToListEntity(Shifts_EmployeesDal.UpdateEmployeeShift(s_db));
        }


        //פונקציה להוספת עובד במשמרת
        public static List<Shift_EmployeesEntity> AddEmployeeShift(Shift_EmployeesEntity s)
        {
            var s_db = Shift_EmployeesEntity.ConvertEntityToDB(s);
            return Shift_EmployeesEntity.ConvertListDBToListEntity(Shifts_EmployeesDal.AddEmployeeShift(s_db));
        }

        //פונקציה לשליפת תפקידי עובדים במשמרת בודדת
        public static List<Shift_EmployeesEntity> GetRolesForSpecificShift(int business_id, int shift_in_day_id, int department_id)
        {
            try
            {
                var day = ConnectDB.entity.Shifts_In_Days.FirstOrDefault(x => x.ID == shift_in_day_id).Day;
                var shift = ConnectDB.entity.Shifts_In_Days.FirstOrDefault(x => x.ID == shift_in_day_id).Shift_ID;
                var l_whole_shifts = GetAllEmployeesShifts(business_id);
                var l = l_whole_shifts.Where(x => x.department_id == department_id && day == x.day && x.shift_id == shift).ToList();
                return l;
            }
            catch
            {
                return null;
            }

        }

        //פונקציה לשליפת משמרות פעילות במחלקה
        public static List<Shift_In_DayEntity> GetActiveShifts(int business_id)
        {
            try
            {
                var l = GetAllEmployeesShifts(business_id).Select(x => new { shift_id = x.shift_id, day = x.day }).Distinct();
                List<Shifts_In_Days> l_shifts_db = ConnectDB.entity.Shifts_In_Days.ToList().Where(x => l.Any(y => y.day == x.Day && y.shift_id == x.Shift_ID)).ToList();
                List<Shift_In_DayEntity> l_shifts = Shift_In_DayEntity.ConvertListDBToListEntity(l_shifts_db);
                return l_shifts;
            }
            catch
            {
                return null;
            }

        }
    }
}
