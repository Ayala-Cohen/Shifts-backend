using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
namespace Entity
{
    public class Shift_EmployeesEntity
    {
        public int business_id { get; set; }
        public int department_id { get; set; }
        public int role_id { get; set; }
        public int shift_id { get; set; }
        public int number_of_shift_employees { get; set; }
        public string day { get; set; }


        //המרת עובד במשמרת מסוג המסד לסוג המחלקה
        public static Shift_EmployeesEntity  ConvertDBToEntity(Shift_Employees s)
        {
            return new Shift_EmployeesEntity() {business_id =s.Business_Id , department_id =s.Departments_Id , role_id =s.Role_Id , shift_id =s.Shift_ID , number_of_shift_employees =s.Number_Of_Shift_Employees, day = s.Day };
        }

        //המרת עובד במשמרת מסוג המחלקה לסוג המסד
        public static Shift_Employees ConvertEntityToDB(Shift_EmployeesEntity s)
        {
            Shift_Employees s_db = new Shift_Employees() { Business_Id = s.business_id, Departments_Id = s.department_id, Role_Id = s.role_id, Shift_ID = s.shift_id, Number_Of_Shift_Employees = s.number_of_shift_employees, Day = s.day };
            return s_db;
        }

        //המרת רשימה מסוג המסד לרשימה מסוג המחלקה
        public static List<Shift_EmployeesEntity> ConvertListDBToListEntity(List<Shift_Employees> l)
        {
            List<Shift_EmployeesEntity> s_Shift_Employees = new List<Shift_EmployeesEntity>();
            foreach (var item in l)
            {
                s_Shift_Employees .Add(ConvertDBToEntity(item));
            }
            return s_Shift_Employees;
        }

        //המרת רשימה מסוג המחלקה לרשימה מסוג המסד 
        public static List<Shift_Employees> ConvertListEntityToListDB(List<Shift_EmployeesEntity> l)
        {
            List<Shift_Employees> s_Shift_Employees = new List<Shift_Employees>();
            foreach (var item in l)
            {
                s_Shift_Employees.Add(ConvertEntityToDB(item));
            }
            return s_Shift_Employees;
        }
    }
}
