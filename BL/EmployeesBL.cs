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
using System.Diagnostics;

namespace BL
{
    public class EmployeesBL
    {
        //פונקציה לשליפת עובד בודד על פי מספר זהות
        public static EmployeesEntity GetEmployeeById(string id)
        {
            return EmployeesEntity.ConvertDBToEntity(EmployeeDal.GetEmployeeById(id));
        }
        //פונקציה לשליפת רשימת עובדים
        public static List<EmployeesEntity> GetAllEmployees(int business_id)
        {
            return EmployeesEntity.ConvertListDBToListEntity(EmployeeDal.GetAllEmployees(business_id));
        }

        //פונקציה למחיקת עובד
        public static List<EmployeesEntity> DeleteEmployee(string id)
        {
            return EmployeesEntity.ConvertListDBToListEntity(EmployeeDal.DeleteEmployee(id));
        }
        //פונקציה לעדכון עובד
        public static List<EmployeesEntity> UpdateEmployee(EmployeesEntity e)
        {
            var e_db = EmployeesEntity.ConvertEntityToDB(e);
            return EmployeesEntity.ConvertListDBToListEntity(EmployeeDal.UpdateEmployee(e_db));
        }


        //פונקציה להוספת עובד
        public static List<EmployeesEntity> AddEmployee(EmployeesEntity e)
        {
            var e_db = EmployeesEntity.ConvertEntityToDB(e);
            return EmployeesEntity.ConvertListDBToListEntity(EmployeeDal.AddEmployee(e_db));
        }

        //פונקציה לשליפת כל הרשומות מטבלת עובדים במחלקות
        public static Dictionary<string, List<DepartmentsEntity>> GetAllEmployeesDepartments(int business_id)
        {
            try
            {
                Dictionary<string, List<DepartmentsEntity>> l = new Dictionary<string, List<DepartmentsEntity>>();
                var l_employees_db = ConnectDB.entity.Employees.Where(x => x.Business_Id == business_id).ToList();
                List<DepartmentsEntity> departments_to_employee;
                if (l_employees_db != null)
                {
                    var l_departments_employees = ConnectDB.entity.get_employee_in_department().ToList();
                    foreach (var employee in l_employees_db)
                    {
                        departments_to_employee = new List<DepartmentsEntity>();
                        foreach (var e_department in l_departments_employees.Where(x=>x.Employee_ID == employee.ID))
                        {
                            departments_to_employee.Add(DepartmentsBL.GetDepartmentById(e_department.Departments_ID));
                        }
                        if(!l.Keys.Contains(employee.ID))
                            l.Add(employee.ID, departments_to_employee);
                    }
                }
                return l;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }

        }

        //פונקציה לשליפת המחלקות בהן עובד כל עובד
        public static List<DepartmentsEntity> GetDepartmentsForEmployee(string employee_id)
        {
            try
            {
                List<DepartmentsEntity> l_departments = null;
                Employees e = ConnectDB.entity.Employees.FirstOrDefault(x => x.ID == employee_id);
                if (e != null)
                {
                    if (e.Departments.Count != 0)
                    {
                        l_departments = DepartmentsEntity.ConvertListDBToListEntity(e.Departments.ToList());
                    }
                }
                return l_departments;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }
        //הוספת מחלקות לעובד
        public static List<DepartmentsEntity> AddOrRemoveDepartmentsForEmployee(List<DepartmentsEntity> l, string id)
        {
            try
            {
                Employees e = ConnectDB.entity.Employees.FirstOrDefault(x => x.ID == id);
                List<Departments> l_departments = DepartmentsEntity.ConvertListEntityToListDB(l);
                if (l.Count() < e.Departments.Count)//העובד כבר לא עובד במחלקה זו
                {
                    var l_departments_for_deleting = e.Departments.Where(x => !l.Any(y => y.id == x.ID)).ToList();
                    foreach (var dep in l_departments_for_deleting)
                    {
                        e.Departments.Remove(dep);
                    }
                }
                else
                    foreach (var item in l_departments)
                    {
                        if (!e.Departments.Any(x => x.ID == item.ID))
                            ConnectDB.entity.add_employee_in_department(id, item.ID);
                    }
                ConnectDB.entity.SaveChanges();
                return DepartmentsEntity.ConvertListDBToListEntity(e.Departments.ToList());

            }
            catch
            {
                return null;
            }
        }
        //פונקציה לבדיקת פרטי עובד ע"י שם משתמש וסיסמה
        public static EmployeesEntity CheckEmployee(string email, string password)
        {
            try
            {
                Employees e = ConnectDB.entity.Employees.FirstOrDefault(x => x.Email == email && x.Password == password);
                if (e != null)
                    return EmployeesEntity.ConvertDBToEntity(e);
                return null;
            }
            catch
            {
                return null;
            }
        }
        public static bool CheckId(string id)
        {
            int sum = 0;
            if (id.Count() == 9)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (((i + 1) % 2) != 0)
                    {
                        sum += int.Parse(id[i].ToString());
                    }
                    else
                    {
                        int num = int.Parse(id[i].ToString()) * 2;
                        if (num >= 10)
                            sum += (num % 10 + (num - (num % 10)) / 10);
                        else
                            sum += num;
                    }
                }
                return sum % 10 == 0;

            }
            return false;
        }




        //פונקציה להכנסת נתוני עובדים מקובץ אקסל
        public static List<EmployeesEntity> ImportFromExcel(int business_id, string filePath)
        {
            EmployeesEntity e = new EmployeesEntity();
            Excel.Application xlapp = new Excel.Application();
            Excel.Workbook xlworkbook = xlapp.Workbooks.Open(filePath);
            Excel._Worksheet xlworksheet = xlworkbook.Sheets[1];
            Excel.Range xlrange = xlworksheet.UsedRange;
            List<string> deps;
            try
            {

                for (int i = 2; i <= xlrange.Rows.Count; i++)
                {
                    deps = new List<string>();
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
                                    if (CheckId(currentCell))
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
                                case "מחלקה":
                                    if (currentCell.Contains(","))//העובד עובד ביותר ממחלקה אחת
                                    {
                                        deps = currentCell.Split(',').ToList();
                                    }
                                    else
                                        deps.Add(currentCell);
                                    break;
                            }
                        }
                    }
                    e.business_id = business_id;
                    var b = BusinessBL.GetBusinessById(business_id);
                    e.password = $"Ab{e.id}";
                    if (ConnectDB.entity.Employees.FirstOrDefault(x => x.ID == e.id) == null)
                    {
                        ConnectDB.entity.Employees.Add(EmployeesEntity.ConvertEntityToDB(e));
                        ConnectDB.entity.SaveChanges();
                        //הוספת מחלקות לעובד החדש
                        foreach (var item in deps)
                        {
                            var dep_db = ConnectDB.entity.Departments.FirstOrDefault(x => x.Name == item);
                            if (dep_db != null)
                                ConnectDB.entity.add_employee_in_department(e.id, dep_db.ID);

                        }
                        ConnectDB.entity.SaveChanges();
                    }
                }
                return EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employees.Where(x => x.Business_Id == business_id).ToList());
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"{ex}בהעתקת נתונים מקובץ אקסל");
                return null;
            }
            finally
            {
                xlworkbook.Close();
            }
        }


        //פונקציה להחזרת עובדים שאינם משובצים בכל המשמרות שעליהם לבצע
        //public static Dictionary<string, int> GetEmployeesToAssigning()
        //{

        //    List<AssigningEntity> assignings = AssigningBL.currentAssigning;

        //    Dictionary<string, int> d = new Dictionary<string, int>();
        //    foreach (var item in ConnectDB.entity.Employees)
        //    {
        //        int num = item.Employee_Roles.Min_Of_Shift - assignings.FindAll(x => x.employee_id == item.ID).Count();
        //        d.Add(item.ID, num);
        //    }
        //    return d;
        //}

        //פונקציה לבדיקה האם עובד משובץ בכל המשמרות שעליו לבצע
        public static bool IsAssignedInAllShifts(string id)
        {
            EmployeesEntity e = GetEmployeeById(id);
            int num_for_performing = Employees_RoleBL.GetEmployeeRoleById(e.role_id).min_of_shift;
            int num_assigned = AssigningBL.localAssigning.Where(x => x.employee_id == id).Count();//שליפת מספר הפעמים בהם משובץ העובד
            return num_assigned == num_for_performing;
        }
        //פונקציית עזר - יצירת מילון עבור דירוג סטטיסטי
        public static Dictionary<string, Dictionary<int, int>> CreateDictionaryOfSatisfaction(int business_id)
        {
            try
            {
                Dictionary<string, Dictionary<int, int>> dicSatisfactionOfEmployee = new Dictionary<string, Dictionary<int, int>>();
                Dictionary<int, int> dic;
                //if (ConnectDB.entity.Satisfaction_Status.Count() != 0)
                //{
                    //פונקציה ליצירת מילון המכיל כמפתח קוד עובד ועבור כל עובד את השכיחות של כל דירוג סטטיסטי
                    //רשימת העובדים בעסק ספציפי
                    var employees_of_business = GetAllEmployees(business_id);
                    var satisfaction_status_of_specific_business = ConnectDB.entity.Satisfaction_Status.Where(x => ConnectDB.entity.Employees.FirstOrDefault(y => y.ID == x.Employee_ID).Business_Id == business_id).ToList();
                    var grouped_list = satisfaction_status_of_specific_business.GroupBy(x => x.Employee_ID).ToDictionary(x => x.Key);//קיבוץ לפי קוד עובד
                    foreach (var employee in employees_of_business)//מעבר על הרשימה המקובצת
                    {
                        dic = new Dictionary<int, int>() { { 1, 0 }, { 4, 0 } };

                        if (grouped_list.Any(x => x.Key == employee.id))//אם לעובד יש ערכים בטבלת שביעות רצון
                        {
                            //dic.Clear();
                            foreach (var status in ConnectDB.entity.Satisfaction_Status.GroupBy(x => x.Satisfaction_Status1))//מעבר על רשימת הדירוגים הסטטיסטיים המקובצת לפי דירוג סטטיסטי
                            {
                                dic[status.Key] = grouped_list[employee.id].Count(x => x.Satisfaction_Status1 == status.Key);//הוספת ערכים למילון השכיחויות  
                            }
                        }
                        dicSatisfactionOfEmployee.Add(employee.id, dic); //הוספת ערכים למילון הראשי
                    }
                //}
                return dicSatisfactionOfEmployee;
            }
            catch
            {
                return null;
            }

        }
        //:פונקציית עזר - מיון לפי הדירוג על מנת שיעבור על הדירוגים באופן הבא
        //קודם מעדיף אחר כך יכול אחר כך מעדיף שלא ובסוף לא יכול
        public static Dictionary<string, IGrouping<string, RatingEntity>> OrderByRating(Dictionary<string, IGrouping<string, RatingEntity>> dic)
        {
            try
            {
                Dictionary<string, IGrouping<string, RatingEntity>> ordered_dic = new Dictionary<string, IGrouping<string, RatingEntity>>();
                Dictionary<int, string> d = new Dictionary<int, string> { { 1, "מעדיף" }, { 2, "יכול" }, { 3, "מעדיף שלא" }, { 4, "לא יכול" } };
                foreach (var item in d)
                {
                    if (dic.Keys.Contains(item.Value))
                    {
                        var ratings = dic[item.Value];
                        ordered_dic.Add(ratings.Key, ratings);
                    }
                }
                return ordered_dic;
            }
            catch
            {
                return null;
            }

        }
        //פונקציה להחזרת העובד האופטימלי לשיבוץ
        public static List<EmployeesEntity> GetOptimalEmployee(int shift_in_day_id, int role_id, int min_for_perfoming, bool is_last_option)
        {
            #region listEmployees
            try
            {
                //(פונקציה זו אחראית להחזיר את רשימת העובדים האופטימליים לשיבוץ (עובדים מאותו תפקיד
                //:העובדים האופטימליים לשיבוץ הם 
                //בדירוג מעדיף ויכול נשלוף את מספר העובדים מאותו תפקיד הנצרכים שהיו הכי פחות מרוצים על מנת לשבץ אותם
                //בדירוג מעדיף שלא ולא יכול נשלוף את העובדים (כנ"ל) שהיו מרוצים רוב הפעמים על מנת לשבץ אותם
                int shift_id, key;
                bool is_has_constaint;
                List<EmployeesEntity> l = new List<EmployeesEntity>();
                Dictionary<string, KeyValuePair<string, Dictionary<int, int>>> l_suitable;//רשימת העובדים המתאימים לשיבוץ במשמרת זו
                string id, day;
                Dictionary<string, Dictionary<int, int>> d = AssigningBL.dic_of_satisfaction;//מילון שביעות רצון
                var grouped_by_shift = ConnectDB.entity.Rating.GroupBy(x => x.Shift_In_Day).ToDictionary(x => x.Key);//רשימת הדירוגים מקובצת לפי משמרות
                Dictionary<int, Dictionary<string, IGrouping<string, RatingEntity>>> dic_shift_rating = AssigningBL.dic_shift_rating;//מילון המכיל כמפתח קוד משמרת וכערך מילון שמכיל את רשימת הדירוגים מקובצת לפי דירוג
                var dic_of_shift = OrderByRating(dic_shift_rating[shift_in_day_id]);//מיון רשימת הדירוגים על מנת שישבץ קודם כל את אלו שמעדיפים, אחר כך את יכולים וכו
                foreach (var item in dic_of_shift)//בדיקה לגבי המשמרת הספציפית
                {
                    //שליפת העובדים שמופיעים תחת הדירוג הנוכחי מתוך המילון
                    var specific = d.Where(x => item.Value.Any(y => y.employee_id == x.Key) && GetEmployeeById(x.Key).role_id == role_id);

                    if (item.Key == "מעדיף" || item.Key == "יכול")
                        key = 4;
                    else//דירוג לא יכול או מעדיף שלא
                        key = 1;
                    if (specific.Count() != 0)//כאשר ישנם עובדים מאותו התפקיד תחת אותו הדירוג
                    {
                        if (key == 4 || is_last_option)//שיבוץ קודם כל של אלו שיכולים ומעדיפים ואם אין אופציה אחרת נשבץ גם את אלו בדירוגים הנמוכים יותר
                        {
                            l_suitable = d.Where(x => specific.Any(y => y.Key == x.Key)).OrderByDescending(x => x.Value[key]).ToDictionary(x => x.Key);
                            foreach (var employee in l_suitable)
                            {
                                id = employee.Key;//מספר הזהות של העובד האופטימלי לשיבוץ הנוכחי
                                day = ConnectDB.entity.Shifts_In_Days.First(x => x.ID == shift_in_day_id).Day;//היום לגביו מתבצעת הבדיקה
                                shift_id = ConnectDB.entity.Shifts_In_Days.First(x => x.ID == shift_in_day_id).Shift_ID;//המשמרת לגביה מתבצעת הבדיקה
                                                                                                                        //בדיקה האם לעובד זה יש אילוץ קבוע במשמרת זו
                                is_has_constaint = ConnectDB.entity.Constraints.FirstOrDefault(x => x.Shift_ID == shift_id && day == x.Day && x.Employee_Id == id) != null;
                                //עדיין לא שובץ בכל המשמרות שעליו לבצע ואין לו אילוץ קבוע במשמרת זו וכן שהוא לא משובץ כבר במשמרת זו 
                                if (!IsAssignedInAllShifts(id) && !is_has_constaint && !CheckIfAssignedInShift(shift_in_day_id, id))
                                {
                                    l.Add(EmployeesEntity.ConvertDBToEntity(ConnectDB.entity.Employees.FirstOrDefault(x => x.ID == id)));//הוספה לרשימה הסופית של העובדים האופטימליים
                                    if (l.Count() == min_for_perfoming)//כאשר נמצאו מספר עובדים מתאימים לפי מספר העובדים הנדרשים למשמרת זו מאותו התפקיד
                                        return l;
                                }
                            }
                        }
                    }
                }
                return l;
            }
            catch(Exception e)
            {
                Debug.WriteLine($"במציאת עובדים אופטימלים\n{e}");
                return null;
            }

            #endregion
        }
        //פונקציה לבדיקה האם עובד משובץ כבר במשמרת מסוימת
        public static bool CheckIfAssignedInShift(int shift_in_day_id, string employee_id)
        {
            return AssigningBL.localAssigning.Any(x => x.employee_id == employee_id && x.shift_in_day_id == shift_in_day_id);
        }
        //פונקציה להשלמת דירוגים אם לעובד אין דירוג על כל המשמרות הפעילות בעסק
        public static void CompleteRatingOfAllShifts(string employee_id)
        {
            try
            {
                var e = GetEmployeeById(employee_id);
                var l_rating = RatingBL.GetAllRatingOfEmployee(employee_id);
                var l_active_shifts = Shifts_EmployeesBL.GetActiveShifts(e.business_id);
                if (!l_active_shifts.All(x => l_rating.Any(y => y.shift_in_day == x.id)))
                    foreach (var shift_in_day in l_active_shifts)
                    {
                        bool is_has_constaint = ConnectDB.entity.Constraints.FirstOrDefault(x => x.Shift_ID == shift_in_day.shift_id && shift_in_day.day == x.Day && x.Employee_Id == employee_id) != null;

                        if (!l_rating.Any(x => x.shift_in_day == shift_in_day.id) && !is_has_constaint)
                        {
                            RatingEntity r = new RatingEntity() { employee_id = employee_id, rating = "יכול", shift_in_day = shift_in_day.id, shift_approved = false, shift_id = shift_in_day.shift_id };
                            RatingBL.AddRating(r, shift_in_day.day);
                            l_rating = RatingBL.GetAllRatingOfEmployee(employee_id);
                        }
                    }
            }
            catch
            {

            }
        }


        //פונקציה לשליחת אמייל , הפונקציה מקבלת את הנושא ואת גוף ההודעה לשליחה
        public static void SendEmailToEmployees(List<Employees> l, string subject, string message)
        {
            try
            {
                foreach (var item in l)
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress("shiftssystem98@gmail.com");
                        mail.To.Add(item.Email);
                        mail.Subject = subject;
                        mail.Body = message;
                        mail.IsBodyHtml = true;

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.Credentials = new NetworkCredential("shiftssystem98@gmail.com", "shs2021shs");
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
