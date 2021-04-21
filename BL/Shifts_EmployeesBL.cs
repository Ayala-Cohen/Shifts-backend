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
            try
            {
                Shift_Employees s = ConnectDB.entity.Shift_Employees.FirstOrDefault(x => x.Shift_ID == s_id && x.Role_Id == r_id && x.Day == day && x.Departments_Id == dep_id);
                return Shift_EmployeesEntity.ConvertDBToEntity(s);
            }
            catch
            {
                return null;
            }

        }
        //פונקציה לשליפת רשימת עובדים במשמרות
        public static List<Shift_EmployeesEntity> GetAllEmployeesShifts(int business_id)
        {
            try
            {
                List<Shift_EmployeesEntity> l_employees_in_shifts = new List<Shift_EmployeesEntity>();
                var l_db_whole = ConnectDB.entity.Shift_Employees.ToList();
                var l_db = l_db_whole.Where(x => x.Business_Id == business_id).ToList();
                if (l_db != null)
                    l_employees_in_shifts = Shift_EmployeesEntity.ConvertListDBToListEntity(l_db);
                return l_employees_in_shifts;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }

        }

        //פונקציה למחיקת עובד במשמרת
        public static List<Shift_EmployeesEntity> DeleteEmployeeShift(int s_id, int r_id, string day, int dep_id)
        {
            try
            {
                Shift_Employees employee_shift_for_deleting = ConnectDB.entity.Shift_Employees.First(x => x.Shift_ID == s_id && x.Role_Id == r_id && x.Day == day && x.Departments_Id == dep_id);
                int business_id = employee_shift_for_deleting.Business_Id;
                ConnectDB.entity.Shift_Employees.Remove(employee_shift_for_deleting);
                ConnectDB.entity.SaveChanges();
                return Shift_EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Shift_Employees.Where(x => x.Business_Id == business_id).ToList());
            }
            catch
            {
                return null;
            }
        }
        //פונקציה לעדכון עובד במשמרת
        public static List<Shift_EmployeesEntity> UpdateEmployeeShift(Shift_EmployeesEntity s)
        {
            try
            {
                Shift_Employees employee_in_shift_for_updating = ConnectDB.entity.Shift_Employees.FirstOrDefault(x => x.Shift_ID == s.shift_id && x.Departments_Id == s.department_id && x.Role_Id == s.role_id && x.Day == s.day);
                if (s.number_of_shift_employees != 0)
                    employee_in_shift_for_updating.Number_Of_Shift_Employees = s.number_of_shift_employees;
                else
                    DeleteEmployeeShift(s.shift_id, s.role_id, s.day, s.department_id);
                ConnectDB.entity.SaveChanges();
                return Shift_EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Shift_Employees.Where(x => x.Business_Id == s.business_id).ToList());
            }
            catch
            {
                return null;
            }
        }


        //פונקציה להוספת עובד במשמרת
        public static List<Shift_EmployeesEntity> AddEmployeeShift(Shift_EmployeesEntity s)
        {
            try
            {
                var s_db = Shift_EmployeesEntity.ConvertEntityToDB(s);
                ConnectDB.entity.Shift_Employees.Add(s_db);
                ConnectDB.entity.SaveChanges();
                return Shift_EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Shift_Employees.Where(x => x.Business_Id == s.business_id).ToList());

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        //פונקציה לשליפת תפקידי עובדים במשמרת בודדת
        public static List<Shift_EmployeesEntity> GetRolesForSpecificShift(int business_id, int shift_in_day_id, int department_id)
        {
            try
            {
                var day = ConnectDB.entity.Shifts_In_Days.FirstOrDefault(x => x.ID == shift_in_day_id).Day;
                var shift = ConnectDB.entity.Shifts_In_Days.FirstOrDefault(x => x.ID == shift_in_day_id).Shift_ID;
                var l = GetAllEmployeesShifts(business_id).Where(x => x.department_id == department_id && day == x.day && x.shift_id == shift).ToList();
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
