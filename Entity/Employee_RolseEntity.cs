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
        public int ID { get; set; }
        public int Business_Id { get; set; }
        public string Role { get; set; }
        public int Min_Of_Shift { get; set; }

        //המרת תפקיד מסוג המסד לסוג המחלקה
        public static Employee_RolesEntity  ConvertDBToEntity(Employee_Roles e)
        {
            return new Employee_RolesEntity() { ID = e.ID, Business_Id = e.Business_Id, Role = e.Role , Min_Of_Shift =e.Min_Of_Shift  };
        }

        //המרת תפקיד מסוג המחלקה לסוג המסד
        public static Employee_Roles ConvertEntityToDB(Employee_RolesEntity e)
        {
            return new Employee_Roles() { ID = e.ID, Business_Id = e.Business_Id, Role = e.Role, Min_Of_Shift = e.Min_Of_Shift };
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
