using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
namespace Entity
{
    public class EmployeesEntity
    {
        public string id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public int role_id { get; set; }
        public string email { get; set; }
        public int business_id { get; set; }
        public string phone { get; set; }



        //המרת עובד מסוג המסד לסוג המחלקה
        public static EmployeesEntity ConvertDBToEntity(Employees e)
        {
            return new EmployeesEntity () { id = e.ID,  name=e.Name , password=e.Password , role_id=e.Role_Id , email=e.Email ,business_id = e.Business_Id , phone = e.Phone };
        }

        //המרת עובד מסוג המחלקה לסוג המסד
        public static Employees  ConvertEntityToDB(EmployeesEntity  e)
        {
            return new Employees () { ID = e.id, Name = e.name, Password = e.password, Role_Id = e.role_id, Email = e.email, Business_Id = e.business_id, Phone = e.phone };
        }

        //המרת רשימה מסוג המסד לרשימה מסוג המחלקה
        public static List<EmployeesEntity> ConvertListDBToListEntity(List<Employees > l)
        {
            List<EmployeesEntity> e_Employee = new List<EmployeesEntity>();
            foreach (var item in l)
            {
                e_Employee .Add(ConvertDBToEntity(item));
            }
            return e_Employee;
        }

        //המרת רשימה מסוג המחלקה לרשימה מסוג המסד 
        public static List<Employees > ConvertListEntityToListDB(List<EmployeesEntity> l)
        {
            List<Employees > e_Employee = new List<Employees >();
            foreach (var item in l)
            {
                e_Employee.Add(ConvertEntityToDB(item));
            }
            return e_Employee;
        }
    }
}
