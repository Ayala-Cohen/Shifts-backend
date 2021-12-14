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
            return Employee_RolesEntity.ConvertDBToEntity(Employees_RolesDal.GetEmployeeRoleById(id));
        }
        //פונקציה לשליפת רשימת תפקידי עובדים
        public static List<Employee_RolesEntity> GetAllEmployeesRoles(int business_id)
        {
            return Employee_RolesEntity.ConvertListDBToListEntity(Employees_RolesDal.GetAllEmployeesRoles(business_id));
        }

        //פונקציה למחיקת תפקיד
        public static List<Employee_RolesEntity> DeleteEmployeeRole(int id)
        {
            return Employee_RolesEntity.ConvertListDBToListEntity(Employees_RolesDal.DeleteEmployeeRole(id));
        }
        //פונקציה לעדכון תפקיד
        public static List<Employee_RolesEntity> UpdateEmployeeRole(Employee_RolesEntity e)
        {
            var e_db = Employee_RolesEntity.ConvertEntityToDB(e);
            return Employee_RolesEntity.ConvertListDBToListEntity(Employees_RolesDal.UpdateEmployeeRole(e_db));
        }


        //פונקציה להוספת תפקיד
        public static List<Employee_RolesEntity> AddEmployeeRole(Employee_RolesEntity e)
        {
            var e_db = Employee_RolesEntity.ConvertEntityToDB(e);
            return Employee_RolesEntity.ConvertListDBToListEntity(Employees_RolesDal.AddEmployeeRole(e_db));
        }
    }
}
