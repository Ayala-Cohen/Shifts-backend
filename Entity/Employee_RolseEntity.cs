using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
namespace Entity
{
    public class Employee_RolesEntity
    {
        public int id { get; set; }
        public int business_id { get; set; }
        public string role { get; set; }
        public int min_of_shift { get; set; }

        //המרת תפקיד מסוג המסד לסוג המחלקה
        public static Employee_RolesEntity  ConvertDBToEntity(Employee_Roles e)
        {
            return new Employee_RolesEntity() { id = e.ID, business_id = e.Business_Id, role = e.Role , min_of_shift =e.Min_Of_Shift  };
        }

        //המרת תפקיד מסוג המחלקה לסוג המסד
        public static Employee_Roles ConvertEntityToDB(Employee_RolesEntity e)
        {
            return new Employee_Roles() { ID = e.id, Business_Id = e.business_id, Role = e.role, Min_Of_Shift = e.min_of_shift };
        }



        //המרת רשימה מסוג המסד לרשימה מסוג המחלקה
        public static List<Employee_RolesEntity > ConvertListDBToListEntity(List<Employee_Roles > l)
        {
            List<Employee_RolesEntity> e_Employee_Roles = new List<Employee_RolesEntity>();
            foreach (var item in l)
            {
                e_Employee_Roles.Add(ConvertDBToEntity(item));
            }
            return e_Employee_Roles;
        }

        //המרת רשימה מסוג המחלקה לרשימה מסוג המסד
        public static List<Employee_Roles> ConvertListEntityToListDB(List<Employee_RolesEntity> l)
        {
            List<Employee_Roles> e_Employee_Roles = new List<Employee_Roles>();
            foreach (var item in l)
            {
                e_Employee_Roles.Add(ConvertEntityToDB(item));
            }
            return e_Employee_Roles;
        }
    }


}
