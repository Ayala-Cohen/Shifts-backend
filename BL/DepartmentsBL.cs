using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Entity;

namespace BL
{
    public class DepartmentsBL
    {
        //פונקציה לשליפת מחלקה בודדת על פי קוד
        public static DepartmentsEntity GetDepartmentById(int id)
        {
            DepartmentsEntity d = DepartmentsEntity.ConvertDBToEntity(ConnectDB.entity.Departments.First(x => x.ID == id));
            return d;
        }
        //פונקציה לשליפת רשימת מחלקות
        public static List<DepartmentsEntity> GetAllDepartments()
        {
            List<DepartmentsEntity> l_departments = DepartmentsEntity.ConvertListDBToListEntity(ConnectDB.entity.Departments.ToList());
            return l_departments;
        }

        //פונקציה למחיקת מחלקה
        public static List<DepartmentsEntity> DeleteDepartment(int id)
        {
            //מחיקה של כל הנתונים המקושרים לשדה זה קודם
            Departments d_for_deleting = ConnectDB.entity.Departments.First(x => x.ID == id);
            ConnectDB.entity.Departments.Remove(d_for_deleting);
            return DepartmentsEntity.ConvertListDBToListEntity(ConnectDB.entity.Departments.ToList());
        }
        //פונקציה לעדכון מחלקה
        public static List<DepartmentsEntity> UpdateDepartment(DepartmentsEntity d)
        {
            Departments d_for_updating = ConnectDB.entity.Departments.First(x => x.ID == d.id);
            d_for_updating.Business_Id = d.business_id;
            d_for_updating.Diary_Closing_Day = d.diary_closing_day;
            d_for_updating.Diary_Opening_Day = d.diary_opening_day;
            d_for_updating.Name = d.name;
            ConnectDB.entity.SaveChanges();
            return DepartmentsEntity.ConvertListDBToListEntity(ConnectDB.entity.Departments.ToList());
        }

        //פונקציה להוספת מחלקה
        public static List<DepartmentsEntity> AddDepartment(DepartmentsEntity d)
        {
            ConnectDB.entity.Departments.Add(DepartmentsEntity.ConvertEntityToDB(d));
            ConnectDB.entity.SaveChanges();
            return DepartmentsEntity.ConvertListDBToListEntity(ConnectDB.entity.Departments.ToList());
        }
    }
}
