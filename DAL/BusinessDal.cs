using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class BusinessDal
    {
        static ShiftEntities db = new ShiftEntities();
        //פונקציה לשליפת עסק בודד על פי קוד
        public static Business GetBusinessById(int id)
        {
            try
            {
                Business b = db.Business.First(x => x.ID == id);
                return b;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //פונקציה לשליפת רשימת עסקים
        public static List<Business> GetAllBusinesses()
        {
            try
            {
                List<Business> l_business = db.Business.ToList();
                return l_business;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return null;
        }

        //פונקציה למחיקת עסק
        public static List<Business> DeleteBusiness(int id)
        {
            try
            {
                //מחיקה של כל הנתונים המקושרים לשדה זה 
                foreach (var item in db.Shift_Employees.Where(x => x.Business_Id == id))
                {
                    Shifts_EmployeesDal.DeleteEmployeeShift(item.Shift_ID, item.Role_Id, item.Day, item.Departments_Id);
                }
                foreach (var item in db.Employees.Where(x => x.Business_Id == id))
                {
                    EmployeeDal.DeleteEmployee(item.ID);
                }
                foreach (var item in db.Departments.Where(x => x.Business_Id == id))
                {
                    DepartmentsDal.DeleteDepartment(item.ID);
                }
                foreach (var item in db.Shifts.Where(x => x.Business_Id == id))
                {
                    ShiftsDal.DeleteShift(item.ID);
                }
                foreach (var item in db.Employee_Roles.Where(x => x.Business_Id == id))
                {
                    Employees_RolesDal.DeleteEmployeeRole(item.ID);
                }
                Business business_for_deleting = db.Business.First(x => x.ID == id);
                db.Business.Remove(business_for_deleting);
                db.SaveChanges();
                return GetAllBusinesses();
            }
            catch (Exception)
            {
                return null;
            }

        }
        //פונקציה לעדכון עסק
        public static List<Business> UpdateBusiness(Business b)
        {
            try
            {
                Business business_for_updating = db.Business.First(x => x.ID == b.ID);
                business_for_updating.Full_Name = b.Full_Name;
                business_for_updating.Logo = b.Logo;
                business_for_updating.Name = b.Name;
                business_for_updating.Number = b.Number;
                business_for_updating.Password = b.Password;
                business_for_updating.User_Name = b.User_Name;
                db.SaveChanges();
                return GetAllBusinesses();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }

        }


        //פונקציה להוספת עסק
        public static List<Business> AddBusiness(Business b)
        {
            try
            {
                db.Business.Add(b);
                db.SaveChanges();
                return GetAllBusinesses();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        //פונקציה לשליפת פרטי עסק על ידי פרטי מנהל
        public static Business GetBusinessByDirectorDetails(string email, string password)
        {
            try
            {
                Business b = db.Business.FirstOrDefault(x => x.User_Name == email && x.Password == password);
                return b;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

