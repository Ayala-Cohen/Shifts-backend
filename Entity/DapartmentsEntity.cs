using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
namespace Entity
{
    class DepartmentsEntity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Business_Id { get; set; }
        public System.DateTime Diary_Opening_Day { get; set; }
        public System.DateTime Diary_Closing_Day { get; set; }


        //המרת קטגוריה בודדת מסוג המחלקה לסוג המסד
        public static DepartmentsEntity ConvertDBToEntity(Departments d)
        {
            return new DepartmentsEntity() { ID = d.ID, Name = d.Name, Business_Id = d.Business_Id, Diary_Opening_Day = d.Diary_Opening_Day, Diary_Closing_Day = d.Diary_Closing_Day };
        }

        //המרת קטגוריה בודדת מסוג המחלקה לסוג המסד
        public static Departments ConvertEntityToDB (DepartmentsEntity d)
        {
            return new  Departments () { ID = d.ID, Name = d.Name, Business_Id = d.Business_Id, Diary_Opening_Day = d.Diary_Opening_Day, Diary_Closing_Day = d.Diary_Closing_Day };
        }


        //המרת רשימה מסוג המסד לרשימה מסוג המחלקה
        public static List<DepartmentsEntity > ConvertListDBToListEntity(List<Departments > l)
        {
            List<DepartmentsEntity > d_departments = new List<DepartmentsEntity>();
            foreach (var item in l)
            {
                d_departments .Add(ConvertDBToEntity(item));
            }
            return d_departments ;
        }

        //המרת רשימה מסוג המסד לרשימה מסוג המחלקה
        public static List<Departments > ConvertListEntityToListDB(List<DepartmentsEntity> l)
        {
            List<Departments> d_departments = new List<Departments >();
            foreach (var item in l)
            {
                d_departments.Add(ConvertEntityToDB(item));
            }
            return d_departments;
        }
    }


}

