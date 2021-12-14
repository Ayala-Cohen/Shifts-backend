using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class EmployeeDal
    {
        static ShiftEntities db = new ShiftEntities();
        //פונקציה לשליפת עובד בודד על פי מספר זהות
        public static Employees GetEmployeeById(string id)
        {
            try
            {
                Employees e = db.Employees.First(x => x.ID == id);
                return e;
            }
            catch (Exception)
            {
                return null;
            }

        }
        //פונקציה לשליפת רשימת עובדים
        public static List<Employees> GetAllEmployees(int business_id)
        {
            try
            {
                List<Employees> l_employees = db.Employees.Where(x => x.Business_Id == business_id).ToList();
                return l_employees;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //פונקציה למחיקת עובד
        public static List<Employees> DeleteEmployee(string id)
        {
            try
            {
                //מחיקה של כל הנתונים המקושרים לשדה זה 
                foreach (var item in db.Rating.Where(x => x.Employee_ID == id).ToList())
                {
                    RatingDal.DeleteRating(id, item.Shift_In_Day);
                }

                foreach (var item in db.Constraints.Where(x => x.Employee_Id == id).ToList())
                {
                    ConstraintsDal.DeleteConstraint(item.Shift_ID, item.Day, id);
                }
                Employees employee_for_deleting = GetEmployeeById(id);
                int business_id = employee_for_deleting.Business_Id;
                db.Employees.Remove(employee_for_deleting);
                db.SaveChanges();
                return GetAllEmployees(business_id);
            }
            catch (Exception)
            {
                return null;
            }

        }
        //פונקציה לעדכון עובד
        public static List<Employees> UpdateEmployee(Employees e)
        {
            try
            {
                Employees employee_for_updating = GetEmployeeById(e.ID);
                employee_for_updating.Email = e.Email;
                employee_for_updating.Name = e.Name;
                employee_for_updating.Password = e.Password;
                employee_for_updating.Role_Id = e.Role_Id;
                employee_for_updating.Business_Id = e.Business_Id;
                employee_for_updating.Phone = e.Phone;
                db.SaveChanges();
                return GetAllEmployees(e.Business_Id);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"בעדכון עובד {ex}");
                return null;
            }

        }


        //פונקציה להוספת עובד
        public static List<Employees> AddEmployee(Employees e)
        {
            try
            {
                db.Employees.Add(e);
                db.SaveChanges();
                return GetAllEmployees(e.Business_Id);

            }
            catch
            {
                return null;
            }
        }
    }
}
