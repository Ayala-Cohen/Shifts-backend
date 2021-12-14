using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DepartmentsDal
    {
        static ShiftEntities db = new ShiftEntities();
        //פונקציה לשליפת מחלקה בודדת על פי קוד
        public static Departments GetDepartmentById(int id)
        {
            try
            {
                Departments d = db.Departments.First(x => x.ID == id);
                return d;
            }
            catch (Exception)
            {
                return null;
            }

        }
        //פונקציה לשליפת רשימת מחלקות
        public static List<Departments> GetAllDepartments(int business_id)
        {
            try
            {
                var departments = db.Departments.Where(x => x.Business_Id == business_id).ToList();
                return departments;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //פונקציה למחיקת מחלקה
        public static List<Departments> DeleteDepartment(int id)
        {
            try
            {
                Departments d_for_deleting = db.Departments.First(x => x.ID == id);
                int business_id = d_for_deleting.Business_Id;
                //מחיקה של כל הנתונים המקושרים לשדה זה 
                foreach (var item in Shifts_EmployeesDal.GetAllEmployeesShifts(business_id).Where(x=>x.Departments_Id == id))
                {
                    Shifts_EmployeesDal.DeleteEmployeeShift(item.Shift_ID, item.Role_Id, item.Day, item.Departments_Id);
                }
                foreach (var item in db.Assigning.Where(x => x.Department_ID == id))
                {
                    db.Assigning.Remove(item);
                }
                foreach (var item in EmployeeDal.GetAllEmployees(business_id).Where(x => x.Departments.Any(y => y.ID == id)))
                {
                    item.Departments.Clear();
                }
                db.Departments.Remove(d_for_deleting);
                db.SaveChanges();
                return GetAllDepartments(business_id);
            }
            catch (Exception)
            {
                return null;
            }
        }
        //פונקציה לעדכון מחלקה
        public static List<Departments> UpdateDepartment(Departments d)
        {
            try
            {
                Departments d_for_updating = GetDepartmentById(d.ID);
                d_for_updating.Business_Id = d.Business_Id;
                d_for_updating.Diary_Closing_Day = d.Diary_Closing_Day;
                d_for_updating.Diary_Opening_Day = d.Diary_Opening_Day;
                d_for_updating.Name = d.Name;
                db.SaveChanges();
                return GetAllDepartments(d.Business_Id);
            }
            catch (Exception)
            {
                return null;
            }

        }

        //פונקציה להוספת מחלקה
        public static List<Departments> AddDepartment(Departments d)
        {
            try
            {
                db.Departments.Add(d);
                db.SaveChanges();
            }
            catch { }

            return GetAllDepartments(d.Business_Id);
        }
    }
}
