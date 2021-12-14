using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Employees_RolesDal
    {
        static ShiftEntities db = new ShiftEntities();
        //פונקציה לשליפת תפקיד בודד על פי קוד
        public static Employee_Roles GetEmployeeRoleById(int id)
        {
            try
            {
                Employee_Roles e = db.Employee_Roles.First(x => x.ID == id);
                return e;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //פונקציה לשליפת רשימת תפקידי עובדים
        public static List<Employee_Roles> GetAllEmployeesRoles(int business_id)
        {
            try
            {
                List<Employee_Roles> l_employees_role = db.Employee_Roles.Where(x => x.Business_Id == business_id).ToList();
                return l_employees_role;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //פונקציה למחיקת תפקיד
        public static List<Employee_Roles> DeleteEmployeeRole(int id)
        {
            try
            {
                Employee_Roles role_for_deleting = GetEmployeeRoleById(id);

                //מחיקה של כל הנתונים המקושרים לשדה זה 
                foreach (var item in db.Employees.Where(x => x.Role_Id == id))
                {
                    EmployeeDal.DeleteEmployee(item.ID);
                }
                foreach (var item in db.Shift_Employees.Where(x => x.Role_Id == id))
                {
                    Shifts_EmployeesDal.DeleteEmployeeShift(item.Shift_ID, id, item.Day, item.Departments_Id);
                }
                int business_id = role_for_deleting.Business_Id;
                db.Employee_Roles.Remove(role_for_deleting);
                db.SaveChanges();
                return GetAllEmployeesRoles(business_id);
            }
            catch (Exception)
            {
                return null;
            }

        }
        //פונקציה לעדכון תפקיד
        public static List<Employee_Roles> UpdateEmployeeRole(Employee_Roles e)
        {
            try
            {
                Employee_Roles role_for_updating = GetEmployeeRoleById(e.ID);
                role_for_updating.Business_Id = e.Business_Id;
                role_for_updating.Min_Of_Shift = e.Min_Of_Shift;
                role_for_updating.Role = e.Role; 
                db.SaveChanges();
                return GetAllEmployeesRoles(e.Business_Id);
            }
            catch (Exception)
            {
                return null;
            }

        }


        //פונקציה להוספת תפקיד
        public static List<Employee_Roles> AddEmployeeRole(Employee_Roles e)
        {
            try
            {
                db.Employee_Roles.Add(e);
                db.SaveChanges();
            }
            catch { }

            return GetAllEmployeesRoles(e.Business_Id);
        }
    }
}
