using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Entity;

namespace BL
{
    public class Employees_RoleBL
    {
        //פונקציה לשליפת תפקיד בודד על פי קוד
        public static Employee_RolesEntity GetEmployeeRoleById(int id)
        {
            Employee_RolesEntity e = Employee_RolesEntity.ConvertDBToEntity(ConnectDB.entity.Employee_Roles.First(x => x.ID == id));
            return e;
        }
        //פונקציה לשליפת רשימת תפקידי עובדים
        public static List<Employee_RolesEntity> GetAllEmployeesRoles()
        {
            List<Employee_RolesEntity> l_employees_role = Employee_RolesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employee_Roles.ToList());
            return l_employees_role;
        }

        //פונקציה למחיקת תפקיד
        public static List<Employee_RolesEntity> DeleteEmployeeRole(int id)
        {
            //מחיקה של כל הנתונים המקושרים לשדה זה קודם
            Employee_Roles role_for_deleting = ConnectDB.entity.Employee_Roles.First(x => x.ID == id);
            ConnectDB.entity.Employee_Roles.Remove(role_for_deleting);
            return Employee_RolesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employee_Roles.ToList());
        }
        //פונקציה לעדכון תפקיד
        public static List<Employee_RolesEntity> UpdateEmployeeRole(Employee_RolesEntity e)
        {
            Employee_Roles role_for_updating = ConnectDB.entity.Employee_Roles.First(x => x.ID == e.id);
            role_for_updating.Business_Id = e.business_id;
            role_for_updating.Min_Of_Shift = e.min_of_shift;
            role_for_updating.Role = e.role;
            ConnectDB.entity.SaveChanges();
            return Employee_RolesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employee_Roles.ToList());
        }


        //פונקציה להוספת תפקיד
        public static List<Employee_RolesEntity> AddEmployeeRole(Employee_RolesEntity e)
        {
            ConnectDB.entity.Employee_Roles.Add(Employee_RolesEntity.ConvertEntityToDB(e));
            ConnectDB.entity.SaveChanges();
            return Employee_RolesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employee_Roles.ToList());
        }
    }
}
