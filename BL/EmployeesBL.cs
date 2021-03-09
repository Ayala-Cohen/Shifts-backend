using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Entity;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Net.Mail;
using System.Net;

namespace BL
{
    public class EmployeesBL
    {
        //פונקציה לשליפת עובד בודד על פי קוד
        public static EmployeesEntity GetEmployeeById(string id)
        {
            EmployeesEntity e = EmployeesEntity.ConvertDBToEntity(ConnectDB.entity.Employees.First(x => x.ID == id));
            return e;
        }
        //פונקציה לשליפת רשימת עובדים
        public static List<EmployeesEntity> GetAllEmployees(int business_id)
        {
            List<EmployeesEntity> l_employees = EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employees.Where(x => x.Business_Id == business_id).ToList());
            return l_employees;
        }

        //פונקציה למחיקת עובד
        public static List<EmployeesEntity> DeleteEmployee(string id)
        {
            //מחיקה של כל הנתונים המקושרים לשדה זה 
            foreach (var item in ConnectDB.entity.Rating.Where(x => x.Employee_ID == id))
            {
                ConnectDB.entity.Rating.Remove(item);
            }

            foreach (var item in ConnectDB.entity.Constraints.Where(x => x.Employee_Id == id))
            {
                ConnectDB.entity.Constraints.Remove(item);
            }
            Employees employee_for_deleting = ConnectDB.entity.Employees.First(x => x.ID == id);
            int business_id = employee_for_deleting.Business_Id;
            ConnectDB.entity.Employees.Remove(employee_for_deleting);
            return EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employees.Where(x => x.Business_Id == business_id).ToList());
        }
        //פונקציה לעדכון עובד
        public static List<EmployeesEntity> UpdateEmployee(EmployeesEntity e)
        {
            Employees employee_for_updating = ConnectDB.entity.Employees.First(x => x.ID == e.id);
            employee_for_updating.Email = e.email;
            employee_for_updating.Name = e.name;
            employee_for_updating.Password = e.password;
            employee_for_updating.Role_Id = e.role_id;
            employee_for_updating.Business_Id = e.business_id;
            employee_for_updating.Phone = e.phone;
            ConnectDB.entity.SaveChanges();
            return EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employees.Where(x => x.Business_Id == e.business_id).ToList());
        }


        //פונקציה להוספת עובד
        public static List<EmployeesEntity> AddEmployee(EmployeesEntity e)
        {
            try
            {
                ConnectDB.entity.Employees.Add(EmployeesEntity.ConvertEntityToDB(e));
                ConnectDB.entity.SaveChanges();
            }
            catch
            { }
            return EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employees.Where(x => x.Business_Id == e.business_id).ToList());
        }
        //פונקציה לבדיקת פרטי עובד ע"י שם משתמש וסיסמה
        public static EmployeesEntity CheckEmployee(string email, string password)
        {
            Employees e = ConnectDB.entity.Employees.FirstOrDefault(x => x.Email == email && x.Password == password);
            if (e != null)
                return EmployeesEntity.ConvertDBToEntity(e);
            return null;
        }

        public static void ImportFromExcel(int business_id, Excel.Application application)
        {
            Excel.Application xlapp = new Excel.Application();
            //xlapp.GetOpenFilename(list_employees);
            if (!File.Exists(application.Path))
                File.Copy(application.Path, application.Name);
            Excel.Workbook xlworkbook = xlapp.Workbooks.Open(application.Name);
            Excel._Worksheet xlworksheet = xlworkbook.Sheets[1];
            Excel.Range xlrange = xlworksheet.UsedRange;
            for (int i = 1; i <= xlrange.Rows.Count; i++)
            {
                for (int j = 1; j <= xlrange.Columns.Count; j++)
                {
                    //if (j == 1)

                    //if (xlrange.Cells[i, j] != null && xlrange.Cells[i, j].value2 != null)
                    //    console.writeline(xlrange.Cells[i, j].value2.toString());
                }
            }
            //return EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employees.Where(x => x.Business_Id == business_id).ToList());    
        }

        //פונקציה לשליפת עובד ע"פ כתובת הדוא"ל שלו 
        public static EmployeesEntity GetEmployeeByEmail(string email)
        {
            Employees e = ConnectDB.entity.Employees.FirstOrDefault(x => x.Email == email);
            if (e != null)
                return EmployeesEntity.ConvertDBToEntity(e);
            return null;
        }

        //פונקציה להחזרת עובדים שאינם משובצים בכל המשמרות שעליהם לבצע
        public static Dictionary<string, int> GetEmployeesToAssigning(List<Assigning> assignings)
        {
            //לשנות בהתאם לאוסף המקומי
            Dictionary<string, int> d = new Dictionary<string, int>();
            foreach (var item in ConnectDB.entity.Employees)
            {
                int num = item.Employee_Roles.Min_Of_Shift - assignings.FindAll(x => x.Employee_ID == item.ID).Count();
                d.Add(item.ID, num);
            }
            return d;
        }

        //פונקציה להחזרת העובד האופטימלי לשיבוץ
        //public static EmployeesEntity GetOptimalEmployee()
        //{
        //    //לבדוק מקרה קצה של החזרת רשימה ריקה
        //    List<string> e = ConnectDB.entity.Rating.Where(x => x.Rating1 == "מעדיף").Select(y=>y.Employee_ID).ToList();
        //    Dictionary<string, Dictionary<int, int>> d = new Dictionary<string, Dictionary<int, int>>();
        //    foreach (var item in ConnectDB.entity.Satisfaction_Status)
        //    {
        //        Dictionary<int, int> dic = new Dictionary<int, int>();
        //        ConnectDB.entity.Satisfaction_Status.Select(x => new { stat_rating = x.Satisfaction_Status1, num = ConnectDB.entity.Satisfaction_Status.Count(y => y.Satisfaction_Status1 == item.Satisfaction_Status1) });
        //        //d.Add(item.Employee_ID,);
        //    }
        //}

    }
}
