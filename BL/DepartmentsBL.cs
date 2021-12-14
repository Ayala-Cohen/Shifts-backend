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
            return DepartmentsEntity.ConvertDBToEntity(DepartmentsDal.GetDepartmentById(id));
        }
        //פונקציה לשליפת רשימת מחלקות
        public static List<DepartmentsEntity> GetAllDepartments(int business_id)
        {
            return DepartmentsEntity.ConvertListDBToListEntity(DepartmentsDal.GetAllDepartments(business_id));
        }

        //פונקציה למחיקת מחלקה
        public static List<DepartmentsEntity> DeleteDepartment(int id)
        {
            return DepartmentsEntity.ConvertListDBToListEntity(DepartmentsDal.DeleteDepartment(id));
        }
        //פונקציה לעדכון מחלקה
        public static List<DepartmentsEntity> UpdateDepartment(DepartmentsEntity d)
        {
            var d_db = DepartmentsEntity.ConvertEntityToDB(d);
            return DepartmentsEntity.ConvertListDBToListEntity(DepartmentsDal.UpdateDepartment(d_db));
        }

        //פונקציה להוספת מחלקה
        public static List<DepartmentsEntity> AddDepartment(DepartmentsEntity d)
        {
            var d_db = DepartmentsEntity.ConvertEntityToDB(d);
            return DepartmentsEntity.ConvertListDBToListEntity(DepartmentsDal.AddDepartment(d_db));
        }

        //פונקציה לשליפת רשימת משמרות למחלקה
        public static List<Shift_In_DayEntity> GetShiftForDepartment(int d_id)
        {
            try
            {
                //שליפת משמרות לפי מחלקה
                List<Shift_Employees> l = ConnectDB.entity.Shift_Employees.Where(x => x.Departments_Id == d_id).ToList();
                List<Shift_In_DayEntity> l_shifts = new List<Shift_In_DayEntity>();
                var l_shift_day = l.Select(x => new { shift_id = x.Shift_ID, day = x.Day }).ToList();
                //המרה לסוג של טבלת משמרות ליום
                foreach (var item in l_shift_day)
                {
                    var shift_in_day = ConnectDB.entity.Shifts_In_Days.FirstOrDefault(x => x.Shift_ID == item.shift_id && x.Day == item.day);
                    var shift_in_day_entity = Shift_In_DayEntity.ConvertDBToEntity(shift_in_day);
                    if (l_shifts.FirstOrDefault(x=>x.id == shift_in_day_entity.id) == null)
                        l_shifts.Add(shift_in_day_entity);
                }
                return l_shifts;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
