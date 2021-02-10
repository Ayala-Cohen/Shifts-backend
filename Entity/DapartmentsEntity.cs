using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
namespace Entity
{
    public class DepartmentsEntity
    {
        public int id { get; set; }
        public string name { get; set; }
        public int business_id { get; set; }
        public System.DateTime diary_opening_day { get; set; }
        public System.DateTime diary_closing_day { get; set; }


        //המרת מחלקה בודדת מסוג המסד לסוג המחלקה
        public static DepartmentsEntity ConvertDBToEntity(Departments d)
        {
            return new DepartmentsEntity() { id = d.ID, name = d.Name, business_id = d.Business_Id, diary_opening_day = d.Diary_Opening_Day, diary_closing_day = d.Diary_Closing_Day };
        }

        //המרת קטגוריה בודדת מסוג המחלקה לסוג המסד
        public static Departments ConvertEntityToDB (DepartmentsEntity d)
        {
            return new  Departments () { ID = d.id, Name = d.name, Business_Id = d.business_id, Diary_Opening_Day = d.diary_opening_day, Diary_Closing_Day = d.diary_closing_day };
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

        //המרת רשימה מסוג המחלקה לרשימה מסוג המסד
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

