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
        public static List<Employee_RolesEntity> GetAllEmployeesRoles(int business_id)
        {
            List<Employee_RolesEntity> l_employees_role = Employee_RolesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employee_Roles.Where(x => x.Business_Id == business_id).ToList());
            return l_employees_role;
        }

        //פונקציה למחיקת תפקיד
        public static List<Employee_RolesEntity> DeleteEmployeeRole(int id)
        {
            //מחיקה של כל הנתונים המקושרים לשדה זה 
            foreach (var item in ConnectDB.entity.Employees.Where(x=>x.Role_Id==id))
            {
                ConnectDB.entity.Employees.Remove(item);
            }
            foreach (var item in ConnectDB.entity.Shift_Employees.Where(x=>x.Role_Id==id))
            {
                ConnectDB.entity.Shift_Employees.Remove(item);
            }
            Employee_Roles role_for_deleting = ConnectDB.entity.Employee_Roles.First(x => x.ID == id);
            int business_id = role_for_deleting.Business_Id;
            ConnectDB.entity.Employee_Roles.Remove(role_for_deleting);
            return Employee_RolesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employee_Roles.Where(x => x.Business_Id == business_id).ToList());
        }
        //פונקציה לעדכון תפקיד
        public static List<Employee_RolesEntity> UpdateEmployeeRole(Employee_RolesEntity e)
        {
            Employee_Roles role_for_updating = ConnectDB.entity.Employee_Roles.First(x => x.ID == e.id);
            role_for_updating.Business_Id = e.business_id;
            role_for_updating.Min_Of_Shift = e.min_of_shift;
            role_for_updating.Role = e.role;
            ConnectDB.entity.SaveChanges();
            return Employee_RolesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employee_Roles.Where(x => x.Business_Id == e.business_id).ToList());
        }


        //פונקציה להוספת תפקיד
        public static List<Employee_RolesEntity> AddEmployeeRole(Employee_RolesEntity e)
        {
            try
            {
                ConnectDB.entity.Employee_Roles.Add(Employee_RolesEntity.ConvertEntityToDB(e));
                ConnectDB.entity.SaveChanges();
            }
            catch { }

            return Employee_RolesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employee_Roles.Where(x => x.Business_Id == e.business_id).ToList());
        }
    }
}
