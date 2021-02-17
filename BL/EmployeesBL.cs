using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Entity;

namespace BL
{
    public class EmployeesBL
    {
        //פונקציה לשליפת עובד בודד על פי קוד
        public static EmployeesEntity GetEmployeeById(string id)
        {
            EmployeesEntity e = EmployeesEntity.ConvertDBToEntity(ConnectDB.entity.Employees.First(x => x.ID == id));
            return e;
        }
        //פונקציה לשליפת רשימת עובדים
        public static List<EmployeesEntity> GetAllEmployees(int business_id)
        {
            List<EmployeesEntity> l_employees = EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employees.Where(x=>x.Business_Id == business_id).ToList());
            return l_employees;
        }

        //פונקציה למחיקת עובד
        public static List<EmployeesEntity> DeleteEmployee(string id)
        {
            //מחיקה של כל הנתונים המקושרים לשדה זה קודם
            Employees employee_for_deleting = ConnectDB.entity.Employees.First(x => x.ID == id);
            int business_id = employee_for_deleting.Business_Id;
            ConnectDB.entity.Employees.Remove(employee_for_deleting);
            return EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employees.Where(x=>x.Business_Id == business_id).ToList());
        }
        //פונקציה לעדכון עובד
        public static List<EmployeesEntity> UpdateEmployee(EmployeesEntity e)
        {
            Employees employee_for_updating = ConnectDB.entity.Employees.First(x => x.ID == e.id);
            employee_for_updating.Email = e.email;
            employee_for_updating.Name = e.name;
            employee_for_updating.Password = e.password;
            employee_for_updating.Role_Id = e.role_id;
            employee_for_updating.Business_Id = e.business_id;
            ConnectDB.entity.SaveChanges();
            return EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employees.Where(x=>x.Business_Id == e.business_id).ToList());
        }


        //פונקציה להוספת עובד
        public static List<EmployeesEntity> AddEmployee(EmployeesEntity e)
        {
            ConnectDB.entity.Employees.Add(EmployeesEntity.ConvertEntityToDB(e));
            ConnectDB.entity.SaveChanges();
            return EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employees.Where(x=>x.Business_Id == e.business_id).ToList());
        }
        //פונקציה לבדיקת פרטי עובד ע"י שם משתמש וסיסמה
        public static EmployeesEntity CheckEmployee(string email, string password)
        {
            EmployeesEntity e = EmployeesEntity.ConvertDBToEntity(ConnectDB.entity.Employees.FirstOrDefault(x=>x.Email == email && x.Password == password));
            return e;
        }
    }
}
