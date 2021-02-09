using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
namespace Entity
{
    class Shift_EmployeesEntity
    {
        public int Business_Id { get; set; }
        public int Departments_Id { get; set; }
        public int Role_Id { get; set; }
        public int Shift_ID { get; set; }
        public int Number_Of_Shift_Employees { get; set; }

        //המרת קטגוריה בודדת מסוג המחלקה לסוג המסד
        public static Shift_EmployeesEntity  ConvertDBToEntity(Shift_Employees s)
        {
            return new Shift_EmployeesEntity() {Business_Id =s.Business_Id , Departments_Id =s.Departments_Id , Role_Id =s.Role_Id , Shift_ID =s.Shift_ID , Number_Of_Shift_Employees =s.Number_Of_Shift_Employees };
        }

        //המרת קטגוריה בודדת מסוג המחלקה לסוג המסד
        public static Shift_Employees ConvertEntityToDB(Shift_EmployeesEntity s)
        {
            return new Shift_Employees() { Business_Id = s.Business_Id, Departments_Id = s.Departments_Id, Role_Id = s.Role_Id, Shift_ID = s.Shift_ID, Number_Of_Shift_Employees = s.Number_Of_Shift_Employees };
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

        //המרת רשימה מסוג המסד לרשימה מסוג המחלקה
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
