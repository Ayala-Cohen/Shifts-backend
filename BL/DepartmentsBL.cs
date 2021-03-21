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
        public static List<DepartmentsEntity> GetAllDepartments(int business_id)
        {
            var l = ConnectDB.entity.Departments.Where(x => x.Business_Id == business_id).ToList();
            List<DepartmentsEntity> l_departments = DepartmentsEntity.ConvertListDBToListEntity(l);
            return l_departments;
        }

        //פונקציה למחיקת מחלקה
        public static List<DepartmentsEntity> DeleteDepartment(int id)
        {
            //מחיקה של כל הנתונים המקושרים לשדה זה 
            foreach (var item in ConnectDB.entity.Shift_Employees.Where(x=>x.Departments_Id == id))
            {
                ConnectDB.entity.Shift_Employees.Remove(item);
            }
            Departments d_for_deleting = ConnectDB.entity.Departments.First(x => x.ID == id);
            int business_id = d_for_deleting.Business_Id;
            ConnectDB.entity.Departments.Remove(d_for_deleting);
            return DepartmentsEntity.ConvertListDBToListEntity(ConnectDB.entity.Departments.Where(x => x.Business_Id == business_id).ToList());
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
            return DepartmentsEntity.ConvertListDBToListEntity(ConnectDB.entity.Departments.Where(x => x.Business_Id == d.business_id).ToList());
        }

        //פונקציה להוספת מחלקה
        public static List<DepartmentsEntity> AddDepartment(DepartmentsEntity d)
        {
            try
            {
                ConnectDB.entity.Departments.Add(DepartmentsEntity.ConvertEntityToDB(d));
                ConnectDB.entity.SaveChanges();
            }
            catch { }

            return DepartmentsEntity.ConvertListDBToListEntity(ConnectDB.entity.Departments.Where(x => x.Business_Id == d.business_id).ToList());
        }

        //פונקציה לשליפת רשימת משמרות למחלקה
        public static List<Shift_In_DayEntity> GetShiftForDepartment(int d_id)
        {
            //שליפת משמרות לפי מחלקה
            List<Shift_Employees> l = ConnectDB.entity.Shift_Employees.Where(x => x.Departments_Id == d_id).ToList();
            List<Shift_In_DayEntity> l_shifts = new List<Shift_In_DayEntity>();
            //המרה לסוג של טבלת משמרות ליום
            foreach (var item in l.Select(x => new { shift_id = x.Shift_ID, day = x.Day }).ToList())
            {
                l_shifts.Add(Shift_In_DayEntity.ConvertDBToEntity(ConnectDB.entity.Shifts_In_Days.FirstOrDefault(x => x.Shift_ID == item.shift_id)));
            }
            return l_shifts;

        }
    }
}
