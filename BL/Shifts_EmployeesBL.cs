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
        public static Shift_EmployeesEntity GetEmployeesShiftById(int id)
        {
            Shift_EmployeesEntity s = Shift_EmployeesEntity.ConvertDBToEntity(ConnectDB.entity.Shift_Employees.First(x => x.Shift_ID == id));
            return s;
        }
        //פונקציה לשליפת רשימת עובדים במשמרות
        public static List<Shift_EmployeesEntity> GetAllEmployeesShifts()
        {
            List<Shift_EmployeesEntity> l_employees_in_shifts = Shift_EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Shift_Employees.ToList());
            return l_employees_in_shifts;
        }

        //פונקציה למחיקת עובד במשמרת
        public static List<Shift_EmployeesEntity> DeleteEmployeeShift(int id)
        {
            //מחיקה של כל הנתונים המקושרים לשדה זה קודם
            Shift_Employees employee_shift_for_deleting = ConnectDB.entity.Shift_Employees.First(x => x.Shift_ID == id);
            ConnectDB.entity.Shift_Employees.Remove(employee_shift_for_deleting);
            return Shift_EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Shift_Employees.ToList());
        }
        //פונקציה לעדכון עובד במשמרת
        public static List<Shift_EmployeesEntity> UpdateEmployeeShift(Shift_EmployeesEntity s)
        {
            Shift_Employees employee_in_shift_for_updating = ConnectDB.entity.Shift_Employees.First(x => x.Shift_ID == s.shift_id);
            employee_in_shift_for_updating.Number_Of_Shift_Employees = s.number_of_shift_employees;
            employee_in_shift_for_updating.Role_Id = s.role_id;
            employee_in_shift_for_updating.Business_Id = s.business_id;
            employee_in_shift_for_updating.Departments_Id = s.department_id;
            ConnectDB.entity.SaveChanges();
            return Shift_EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Shift_Employees.ToList());
        }


        //פונקציה להוספת עובד במשמרת
        public static List<Shift_EmployeesEntity> AddEmployeeShift(Shift_EmployeesEntity s)
        {
            ConnectDB.entity.Shift_Employees.Add(Shift_EmployeesEntity.ConvertEntityToDB(s));
            ConnectDB.entity.SaveChanges();
            return Shift_EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Shift_Employees.ToList());
        }
    }
}
