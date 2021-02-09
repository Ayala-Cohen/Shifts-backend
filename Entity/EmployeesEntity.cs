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
        public string ID { get; set; }
        public string Name { get; set; }
        public string password { get; set; }
        public int Role_id { get; set; }
        public string Email { get; set; }
        public int Business_Id { get; set; }


        //המרת עובד מסוג המסד לסוג המחלקה
        public static EmployeesEntity ConvertDBToEntity(Employees e)
        {
            return new EmployeesEntity () { ID = e.ID,  Name=e.Name , password=e.Password , Role_id=e.Role_Id , Email=e.Email ,Business_Id = e.Business_Id };
        }

        //המרת עובד מסוג המחלקה לסוג המסד
        public static Employees  ConvertEntityToDB(EmployeesEntity  e)
        {
            return new Employees () { ID = e.ID, Name = e.Name, Password = e.password, Role_Id = e.Role_id, Email = e.Email, Business_Id = e.Business_Id };
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
