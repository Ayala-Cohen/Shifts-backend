using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Shifts_EmployeesDal
    {
        static ShiftEntities db = new ShiftEntities(); 
        //פונקציה לשליפת עובד במשמרת בודד על פי קוד
        public static Shift_Employees GetEmployeesShiftById(int s_id, int r_id, string day, int dep_id)
        {
            try
            {
                Shift_Employees s = db.Shift_Employees.FirstOrDefault(x => x.Shift_ID == s_id && x.Role_Id == r_id && x.Day == day && x.Departments_Id == dep_id);
                return s;
            }
            catch
            {
                return null;
            }

        }
        //פונקציה לשליפת רשימת עובדים במשמרות
        public static List<Shift_Employees> GetAllEmployeesShifts(int business_id)
        {
            try
            {
                List<Shift_Employees> l_employees_in_shifts = new List<Shift_Employees>();
                var l_whole = db.Shift_Employees.ToList();
                var l = l_whole.Where(x => x.Business_Id == business_id).ToList();
                if (l != null)
                    l_employees_in_shifts = l;
                return l_employees_in_shifts;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }

        }

        //פונקציה למחיקת עובד במשמרת
        public static List<Shift_Employees> DeleteEmployeeShift(int s_id, int r_id, string day, int dep_id)
        {
            try
            {
                Shift_Employees employee_shift_for_deleting = GetEmployeesShiftById(s_id, r_id, day, dep_id);
                int business_id = employee_shift_for_deleting.Business_Id;
                db.Shift_Employees.Remove(employee_shift_for_deleting);
                db.SaveChanges();
                return GetAllEmployeesShifts(business_id);
            }
            catch
            {
                return null;
            }
        }
        //פונקציה לעדכון עובד במשמרת
        public static List<Shift_Employees> UpdateEmployeeShift(Shift_Employees s)
        {
            try
            {
                Shift_Employees employee_in_shift_for_updating = GetEmployeesShiftById(s.Shift_ID, s.Role_Id, s.Day, s.Departments_Id);
                if (s.Number_Of_Shift_Employees != 0)
                    employee_in_shift_for_updating.Number_Of_Shift_Employees = s.Number_Of_Shift_Employees;
                else
                    DeleteEmployeeShift(s.Shift_ID, s.Role_Id, s.Day, s.Departments_Id);
                db.SaveChanges();
                return GetAllEmployeesShifts(s.Business_Id);
            }
            catch
            {
                return null;
            }
        }


        //פונקציה להוספת עובד במשמרת
        public static List<Shift_Employees> AddEmployeeShift(Shift_Employees s)
        {
            try
            {
                db.Shift_Employees.Add(s);
                db.SaveChanges();
                return GetAllEmployeesShifts(s.Business_Id);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }
    }
}
