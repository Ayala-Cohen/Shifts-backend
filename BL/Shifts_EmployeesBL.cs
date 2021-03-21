using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Entity;

namespace BL
{
    public class Shifts_EmployeesBL
    {
        //פונקציה לשליפת עובד במשמרת בודד על פי קוד
        public static Shift_EmployeesEntity GetEmployeesShiftById(int s_id, int r_id, string day)
        {

            Shift_Employees s = ConnectDB.entity.Shift_Employees.FirstOrDefault(x => x.Shift_ID == s_id && x.Role_Id == r_id && x.Day == day);
            if(s!=null)
                return Shift_EmployeesEntity.ConvertDBToEntity(s);
            return null;
        }
        //פונקציה לשליפת רשימת עובדים במשמרות
        public static List<Shift_EmployeesEntity> GetAllEmployeesShifts(int business_id)
        {
            List<Shift_EmployeesEntity> l_employees_in_shifts = Shift_EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Shift_Employees.Where(x=>x.Business_Id == business_id).ToList());
            return l_employees_in_shifts;
        }

        //פונקציה למחיקת עובד במשמרת
        public static List<Shift_EmployeesEntity> DeleteEmployeeShift(int s_id, int r_id, string day)
        {
            Shift_Employees employee_shift_for_deleting = ConnectDB.entity.Shift_Employees.First(x => x.Shift_ID == s_id && x.Role_Id == r_id && x.Day == day);
            int business_id = employee_shift_for_deleting.Business_Id;
            ConnectDB.entity.Shift_Employees.Remove(employee_shift_for_deleting);
            return Shift_EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Shift_Employees.Where(x=>x.Business_Id == business_id).ToList());
        }
        //פונקציה לעדכון עובד במשמרת
        public static List<Shift_EmployeesEntity> UpdateEmployeeShift(Shift_EmployeesEntity s)
        {
            Shift_Employees employee_in_shift_for_updating = ConnectDB.entity.Shift_Employees.First(x => x.Shift_ID == s.shift_id);
            employee_in_shift_for_updating.Number_Of_Shift_Employees = s.number_of_shift_employees;
            ConnectDB.entity.SaveChanges();
            return Shift_EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Shift_Employees.Where(x=>x.Business_Id == s.business_id).ToList());
        }


        //פונקציה להוספת עובד במשמרת
        public static List<Shift_EmployeesEntity> AddEmployeeShift(Shift_EmployeesEntity s)
        {
            ConnectDB.entity.Shift_Employees.Add(Shift_EmployeesEntity.ConvertEntityToDB(s));
            ConnectDB.entity.SaveChanges();
            return Shift_EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Shift_Employees.Where(x=>x.Business_Id == s.business_id).ToList());
        }

        //פונקציה לשליפת תפקידי עובדים במשמרת בודדת
        public static List<Shift_EmployeesEntity> GetRolesForSpecificShift(int business_id, int shift_in_day_id, int department_id)
        {
            var day = ConnectDB.entity.Shifts_In_Days.FirstOrDefault(x => x.ID == shift_in_day_id).Day;
            var shift = ConnectDB.entity.Shifts_In_Days.FirstOrDefault(x => x.ID == shift_in_day_id).Shift_ID;
            var l = GetAllEmployeesShifts(business_id).Where(x => x.department_id == department_id && day == x.day && x.shift_id == shift).ToList();
            return l;
        }
    }
}
