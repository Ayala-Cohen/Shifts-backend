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
        //פונקציה להכנסת נתוני עובדים מקובץ אקסל
        public static List<EmployeesEntity> ImportFromExcel(int business_id, string filePath)
        {
            EmployeesEntity e = new EmployeesEntity();
            Excel.Application xlapp = new Excel.Application();
            Excel.Workbook xlworkbook = xlapp.Workbooks.Open(filePath);
            Excel._Worksheet xlworksheet = xlworkbook.Sheets[1];
            Excel.Range xlrange = xlworksheet.UsedRange;
            try
            {

                for (int i = 2; i <= xlrange.Rows.Count; i++)
                {
                    for (int j = 1; j <= xlrange.Columns.Count; j++)
                    {
                        var contentObj = xlrange.Cells[i, j];
                        var value = xlrange.Cells[i, j].value2;
                        if (contentObj != null && value != null)
                        {
                            string header = xlrange.Cells[1, j].value2.ToString();
                            header = header.Replace("\"", "");
                            string currentCell = xlrange.Cells[i, j].value2.ToString();
                            switch (header)
                            {
                                case "מספר זהות":
                                    e.id = currentCell;
                                    break;
                                case "שם":
                                    e.name = currentCell;
                                    break;
                                case "תפקיד":
                                    Employee_Roles e_role = ConnectDB.entity.Employee_Roles.FirstOrDefault(x => x.Business_Id == business_id && x.Role == currentCell);
                                    int role_id;
                                    if (e_role != null)//תפקיד נמצא
                                        e.role_id = e_role.ID;
                                    else
                                    {
                                        ConnectDB.entity.Employee_Roles.Add(Employee_RolesEntity.ConvertEntityToDB(new Employee_RolesEntity { business_id = business_id, role = currentCell, min_of_shift = 5 }));
                                        ConnectDB.entity.SaveChanges();
                                        role_id = ConnectDB.entity.Employee_Roles.ToList().Last().ID;
                                        e.role_id = role_id;
                                    }
                                    break;
                                case "כתובת דואל":
                                    e.email = currentCell;
                                    break;
                                case "טלפון":
                                    e.phone = currentCell;
                                    break;
                            }
                        }
                    }
                    e.business_id = business_id;
                    e.password = "123456";
                    if (ConnectDB.entity.Employees.FirstOrDefault(x => x.ID == e.id) == null)
                    {
                        ConnectDB.entity.Employees.Add(EmployeesEntity.ConvertEntityToDB(e));
                        ConnectDB.entity.SaveChanges();
                    }
                }
            }
            catch { }
            finally
            {
                xlworkbook.Close();
            }
            return EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employees.Where(x => x.Business_Id == business_id).ToList());
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
        public static Dictionary<string, int> GetEmployeesToAssigning()
        {
            //לשנות בהתאם לאוסף המקומי
            List<Assigning> assignings = ConnectDB.entity.Assigning.ToList();

            Dictionary<string, int> d = new Dictionary<string, int>();
            foreach (var item in ConnectDB.entity.Employees)
            {
                int num = item.Employee_Roles.Min_Of_Shift - assignings.FindAll(x => x.Employee_ID == item.ID).Count();
                d.Add(item.ID, num);
            }
            return d;
        }

        //פונקציה להחזרת העובד האופטימלי לשיבוץ
        public static EmployeesEntity GetOptimalEmployee(int shift_in_day_id)
        {
            Dictionary<string, Dictionary<int, int>> d = new Dictionary<string, Dictionary<int, int>>();
            Dictionary<int, int> dic;
            //יצירת מילון המכיל כמפתח קוד עובד ועבור כל עובד את השכיחות של כל דירוג סטטיסטי
            var grouped_list = ConnectDB.entity.Satisfaction_Status.GroupBy(x => x.Employee_ID);//קיבוץ לפי קוד עובד
            foreach (var item in grouped_list)//מעבר על הרשימה המקובצת
            {
                dic = new Dictionary<int, int>();
                var grouped_by_status = item.GroupBy(x => x.Satisfaction_Status1);//קיבוץ של הרשימה המקובצת לפי דירוג סטטיסטי
                foreach (var status in grouped_by_status.OrderBy(x => x.Key))//מעבר על הרשימה המקובצת לפי דירוג סטטיסטי
                {
                    dic.Add(status.Key, status.Count());//הוספת ערכים למילון השכיחויות  
                }
                d.Add(item.Key, dic); //הוספת ערכים למילון הראשי
            }

            int max, index;
            bool is_found;
            var grouped_by_shift = ConnectDB.entity.Rating.GroupBy(x => x.Shift_In_Day).ToDictionary(x => x.Key);
            Dictionary<int, Dictionary<string, IGrouping<string, Rating>>> dic_shift_rating = new Dictionary<int, Dictionary<string, IGrouping<string, Rating>>>();
            foreach (var item in grouped_by_shift)
            {
                var grouped_by_rating = item.Value.GroupBy(x => x.Rating1).ToDictionary(x => x.Key);
                dic_shift_rating.Add(item.Key, grouped_by_rating);
            }
            foreach (var item in dic_shift_rating[shift_in_day_id])//בדיקה לגבי המשמרת הספציפית
            {
                is_found = false;
                if (item.Value.Count() != 0) //רשימה לא ריקה
                {
                    if (item.Key == "מעדיף" || item.Key == "יכול")
                        index = 3;
                    else
                        index = 0;
                    while (!is_found)
                    {
                        //שליפת העובדים שמופיעים תחת דירוג מסוים מתוך המילון
                        var specific = d.Where(x => item.Value.Any(y => y.Employee_ID == x.Key));
                        //מציאת הדירוג הסטטיסטי הנמוך או הגבוה - תלוי לפי המיקום שנקבע למעלה
                        max = specific.Max(x => x.Value[index]);
                        //בדיקה שלעובד אין שיבוץ קבוע במשמרת מסוימת
                        string id = d.FirstOrDefault(x => x.Value[index] == max).Key;
                        string day = ConnectDB.entity.Shifts_In_Days.First(x => x.ID == shift_in_day_id).Day;
                        int shift_id = ConnectDB.entity.Shifts_In_Days.First(x => x.ID == shift_in_day_id).Shift_ID;
                        bool is_has_constaint = ConnectDB.entity.Constraints.FirstOrDefault(x => x.Shift_ID == shift_id && day == x.Day && x.Employee_Id == id) != null;

                        if (GetEmployeesToAssigning()[id] != 0 && !is_has_constaint) //עדיין לא שובץ בכל המשמרות שעליו לבצע ואין לו אילוץ קבוע במשמרת זו
                            return EmployeesEntity.ConvertDBToEntity(ConnectDB.entity.Employees.FirstOrDefault(x => x.ID == id));
                    }
                }
            }
            return null;
        }
        //פונקציה לבדיקה האם עובד משובץ כבר במשמרת מסוימת
        public static bool checkIfAssignedInShift(int shift_in_day_id, string employee_id)
        {
            //לשנות בהתאם לאוסף המקומי
            return ConnectDB.entity.Assigning.Any(x => x.Employee_ID == employee_id && x.Shift_In_Day_ID == shift_in_day_id);
        }
    }
}
