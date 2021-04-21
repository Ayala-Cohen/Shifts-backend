﻿using System;
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
            try
            {
                DepartmentsEntity d = DepartmentsEntity.ConvertDBToEntity(ConnectDB.entity.Departments.First(x => x.ID == id));
                return d;
            }
            catch (Exception)
            {
                return null;
            }

        }
        //פונקציה לשליפת רשימת מחלקות
        public static List<DepartmentsEntity> GetAllDepartments(int business_id)
        {
            try
            {
                var l = ConnectDB.entity.Departments.Where(x => x.Business_Id == business_id).ToList();
                List<DepartmentsEntity> l_departments = DepartmentsEntity.ConvertListDBToListEntity(l);
                return l_departments;
            }
            catch (Exception)
            {
            }
            return null;
        }

        //פונקציה למחיקת מחלקה
        public static List<DepartmentsEntity> DeleteDepartment(int id)
        {
            try
            {
                //מחיקה של כל הנתונים המקושרים לשדה זה 
                foreach (var item in ConnectDB.entity.Shift_Employees.Where(x => x.Departments_Id == id))
                {
                    Shifts_EmployeesBL.DeleteEmployeeShift(item.Shift_ID, item.Role_Id, item.Day, item.Departments_Id);
                }
                foreach (var item in ConnectDB.entity.Assigning.Where(x => x.Department_ID == id))
                {
                    ConnectDB.entity.Assigning.Remove(item);
                }
                foreach (var item in ConnectDB.entity.Employees.Where(x => x.Departments.Any(y => y.ID == id)))
                {
                    item.Departments.Clear();
                }
                Departments d_for_deleting = ConnectDB.entity.Departments.First(x => x.ID == id);
                int business_id = d_for_deleting.Business_Id;
                ConnectDB.entity.Departments.Remove(d_for_deleting);
                ConnectDB.entity.SaveChanges();
                return DepartmentsEntity.ConvertListDBToListEntity(ConnectDB.entity.Departments.Where(x => x.Business_Id == business_id).ToList());
            }
            catch (Exception)
            {
                return null;
            }

        }
        //פונקציה לעדכון מחלקה
        public static List<DepartmentsEntity> UpdateDepartment(DepartmentsEntity d)
        {
            try
            {
                Departments d_for_updating = ConnectDB.entity.Departments.First(x => x.ID == d.id);
                d_for_updating.Business_Id = d.business_id;
                d_for_updating.Diary_Closing_Day = d.diary_closing_day;
                d_for_updating.Diary_Opening_Day = d.diary_opening_day;
                d_for_updating.Name = d.name;
                ConnectDB.entity.SaveChanges();
                return DepartmentsEntity.ConvertListDBToListEntity(ConnectDB.entity.Departments.Where(x => x.Business_Id == d.business_id).ToList());
            }
            catch (Exception)
            {
                return null;
            }

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
            try
            {
                //שליפת משמרות לפי מחלקה
                List<Shift_Employees> l = ConnectDB.entity.Shift_Employees.Where(x => x.Departments_Id == d_id).ToList();
                List<Shift_In_DayEntity> l_shifts = new List<Shift_In_DayEntity>();
                var l_shift_day = l.Select(x => new { shift_id = x.Shift_ID, day = x.Day }).ToList();
                //המרה לסוג של טבלת משמרות ליום
                foreach (var item in l_shift_day)
                {
                    l_shifts.Add(Shift_In_DayEntity.ConvertDBToEntity(ConnectDB.entity.Shifts_In_Days.FirstOrDefault(x => x.Shift_ID == item.shift_id && x.Day == item.day)));
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
