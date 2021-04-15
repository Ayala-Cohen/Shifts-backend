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
            try
            {
                List<EmployeesEntity> l_employees = EmployeesEntity.ConvertListDBToListEntity(ConnectDB.entity.Employees.Where(x => x.Business_Id == business_id).ToList());
                return l_employees;
            }
            catch (Exception)
            {
            }
            return null;
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
            try
            {
                Employees employee_for_updating = ConnectDB.entity.Employees.First(x => x.ID == e.id);
                employee_for_updating.Email = e.email;
                employee_for_updating.Name = e.name;
                employee_for_updating.Password = e.password;
                employee_for_updating.Role_Id = e.role_id;
                employee_for_updating.Business_Id = e.business_id;
                employee_for_updating.Phone = e.phone;
                ConnectDB.entity.SaveChanges();
            }
            catch (Exception)
            {
            }

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

        //פונקציה לשליפת המחלקות בהן עובד כל עובד
        public static List<DepartmentsEntity> GetDepartmentsForEmployee(string employee_id)
        {
            try
            {
                Employees e = ConnectDB.entity.Employees.FirstOrDefault(x => x.ID == employee_id);
                if (e != null)
                {
                    if (e.Departments.Count != 0)
                    {
                        List<DepartmentsEntity> l_departments = DepartmentsEntity.ConvertListDBToListEntity(e.Departments.ToList());
                        return l_departments;
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        //הוספת מחלקות לעובד
        public static List<DepartmentsEntity> AddOrRemoveDepartmentsForEmployee(List<DepartmentsEntity> l, string id)
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
            if(e.Departments.Count != 0)
                return DepartmentsEntity.ConvertListDBToListEntity(e.Departments.ToList());
            return null;
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


        //פונקציה להחזרת עובדים שאינם משובצים בכל המשמרות שעליהם לבצע
        public static Dictionary<string, int> GetEmployeesToAssigning()
        {
            List<AssigningEntity> assignings = AssigningBL.currentAssigning;

            Dictionary<string, int> d = new Dictionary<string, int>();
            foreach (var item in ConnectDB.entity.Employees)
            {
                int num = item.Employee_Roles.Min_Of_Shift - assignings.FindAll(x => x.employee_id == item.ID).Count();
                d.Add(item.ID, num);
            }
            return d;
        }
        //פונקציה לבדיקה האם עובד משובץ בכל המשמרות שעליו לבצע
        public static bool CheckIfAssignedInAllShifts(string id)
        {
            EmployeesEntity e = GetEmployeeById(id);
            int num_for_performing = Employees_RoleBL.GetEmployeeRoleById(e.role_id).min_of_shift;
            int num_assigned = AssigningBL.currentAssigning.Where(x => x.employee_id == id).Count();//שליפת מספר הפעמים בהם משובץ העובד
            return num_assigned == num_for_performing;
        }
        //פונקציית עזר - יצירת מילון עבור דירוג סטטיסטי
        public static Dictionary<string, Dictionary<int, int>> CreateDictionaryOfSatisfaction(int business_id)
        {
            //פונקציה ליצירת מילון המכיל כמפתח קוד עובד ועבור כל עובד את השכיחות של כל דירוג סטטיסטי
            Dictionary<string, Dictionary<int, int>> d = new Dictionary<string, Dictionary<int, int>>();
            Dictionary<int, int> dic;
            //רשימת העובדים בעסק ספציפי
            var employees_of_business = ConnectDB.entity.Satisfaction_Status.Where(x => ConnectDB.entity.Employees.FirstOrDefault(y => y.ID == x.Employee_ID).Business_Id == business_id);
            var grouped_list = employees_of_business.GroupBy(x => x.Employee_ID);//קיבוץ לפי קוד עובד
            foreach (var item in grouped_list)//מעבר על הרשימה המקובצת
            {
                dic = new Dictionary<int, int>();
                foreach (var status in ConnectDB.entity.Satisfaction_Status.GroupBy(x => x.Satisfaction_Status1))//מעבר על רשימת הדירוגים הסטטיסטיים המקובצת לפי דירוג סטטיסטי
                {
                    dic.Add(status.Key, item.Count(x => x.Satisfaction_Status1 == status.Key));//הוספת ערכים למילון השכיחויות  
                }
                d.Add(item.Key, dic); //הוספת ערכים למילון הראשי
            }
            return d;
        }
        //:פונקציית עזר - מיון לפי הדירוג על מנת שיעבור על הדירוגים באופן הבא
        //קודם מעדיף אחר כך יכול אחר כך מעדיף שלא ובסוף לא יכול
        public static Dictionary<string, IGrouping<string, Rating>> OrderByRating(Dictionary<string, IGrouping<string, Rating>> dic)
        {
            Dictionary<string, IGrouping<string, Rating>> ordered_dic = new Dictionary<string, IGrouping<string, Rating>>();
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
        //פונקציה להחזרת העובד האופטימלי לשיבוץ
        public static List<EmployeesEntity> GetOptimalEmployee(int shift_in_day_id, int role_id, int min_for_perfoming, bool is_last_option)
        {
            #region oneEmployee
            //:מציאת העובד האופטימלי לשיבוץ מתנהל בצורה כזו
            //בדירוג מעדיף ויכול בתחילה נחפש את מי שהדירוג הסטטיסטי הגרוע ביותר מופיע אצלו הכי הרבה פעמים
            //אם נראה שישנם כמה כאלה, נבדוק מי הוא העובד שהדירוג הסטטיסטי הגבוה ביותר מופיע אצלו הכי הרבה פעמים
            //(ואותו נרצה להחזיר (מכיוון שברוב הפעמים הוא היה מרוצה
            //במידה וישנם כמה עובדים שדירוג סטטיסטי זה שכיח אצלם במידה שווה נחזיר אחד מבינהם, לא משנה הסדר
            //בדירוג מעדיף שלא ולא יכול נעשה את אותו התהליך הפוך - קודם נבדוק מי היה מרוצה רוב הפעמים 
            //ואותו נרצה להחזיר. במידה וישנם כמה כאלה נחזיר את הראשון שהיה הכי פחות פעמים לא מרוצה
            //int min, shift_id, max, key;
            //bool is_has_constaint;
            //string id, day;
            //Dictionary<string, Dictionary<int, int>> d = AssigningBL.dic_of_satisfaction;
            //var grouped_by_shift = ConnectDB.entity.Rating.GroupBy(x => x.Shift_In_Day).ToDictionary(x => x.Key);
            //Dictionary<int, Dictionary<string, IGrouping<string, Rating>>> dic_shift_rating = new Dictionary<int, Dictionary<string, IGrouping<string, Rating>>>();
            //foreach (var item in grouped_by_shift)
            //{
            //    var grouped_by_rating = item.Value.GroupBy(x => x.Rating1).ToDictionary(x => x.Key);
            //    dic_shift_rating.Add(item.Key, grouped_by_rating);
            //}
            //foreach (var item in dic_shift_rating[shift_in_day_id])//בדיקה לגבי המשמרת הספציפית
            //{
            //    if (item.Value.Count() != 0) //רשימה לא ריקה
            //    {
            //        //שליפת העובדים שמופיעים תחת דירוג מסוים מתוך המילון
            //        var specific = d.Where(x => item.Value.Any(y => y.Employee_ID == x.Key));
            //        if (item.Key == "מעדיף" || item.Key == "יכול")
            //            key = 4;
            //        else//דירוג לא יכול או מעדיף שלא
            //            key = 1;

            //        max = specific.Max(x => x.Value[key]);//השכיחות הגבוהה ביותר של הדירוג הנמוך או הגבוה(לפי המפתח) ביותר
            //        var l_suitable = d.Where(x => x.Value[key] == max && specific.Any(y => y.Key == x.Key) && GetEmployeeById(x.Key).role_id == role_id);//רשימת העובדים עם הדירוג הנמוך או הגבוה ביותר ושבהם צריך להתחשב הכי הרבה
            //        if (l_suitable.Count() > 1)//אם מדובר בכמה עובדים
            //        {
            //            //חיפוש מתוך אלו ששווים למי שכיחות הדירוג הסטטיסטי הגבוה ביותר יותר נמוכה
            //            //או למי שכיחות הדירוג הנמוך ביותר יותר נמוכה בהתאמה
            //            min = l_suitable.Min(x => x.Value[5 - key]);
            //            l_suitable = d.Where(x => x.Value[5 - key] == min);
            //        }
            //        id = l_suitable.FirstOrDefault().Key;//מספר הזהות של העובד האופטימלי לשיבוץ
            //        //היום והמשמרת לגביהם מדובר השיבוץ
            //        day = ConnectDB.entity.Shifts_In_Days.First(x => x.ID == shift_in_day_id).Day;
            //        shift_id = ConnectDB.entity.Shifts_In_Days.First(x => x.ID == shift_in_day_id).Shift_ID;
            //        //בדיקה האם לעובד זה יש אילוץ קבוע במשמרת זו
            //        is_has_constaint = ConnectDB.entity.Constraints.FirstOrDefault(x => x.Shift_ID == shift_id && day == x.Day && x.Employee_Id == id) != null;
            //        if (GetEmployeesToAssigning()[id] != 0 && !is_has_constaint) //עדיין לא שובץ בכל המשמרות שעליו לבצע ואין לו אילוץ קבוע במשמרת זו
            //            return EmployeesEntity.ConvertDBToEntity(ConnectDB.entity.Employees.FirstOrDefault(x => x.ID == id));
            //    }
            //}
            //return null;
            #endregion
            #region listEmployees
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
            Dictionary<int, Dictionary<string, IGrouping<string, Rating>>> dic_shift_rating = AssigningBL.dic_shift_rating;//מילון המכיל כמפתח קוד משמרת וכערך מילון שמכיל את רשימת הדירוגים מקובצת לפי דירוג
            var dic_of_shift = OrderByRating(dic_shift_rating[shift_in_day_id]);
            foreach (var item in dic_of_shift)//בדיקה לגבי המשמרת הספציפית
            {
                //שליפת העובדים שמופיעים תחת הדירוג הנוכחי מתוך המילון
                var specific = d.Where(x => item.Value.Any(y => y.Employee_ID == x.Key) && GetEmployeeById(x.Key).role_id == role_id);

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
                            if (!CheckIfAssignedInAllShifts(id) && !is_has_constaint && !CheckIfAssignedInShift(shift_in_day_id, id))
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
            #endregion
        }
        //פונקציה לבדיקה האם עובד משובץ כבר במשמרת מסוימת
        public static bool CheckIfAssignedInShift(int shift_in_day_id, string employee_id)
        {
            return AssigningBL.currentAssigning.Any(x => x.employee_id == employee_id && x.shift_in_day_id == shift_in_day_id);
        }
        //פונקציה להשלמת דירוגים אם לעובד אין דירוג על כל המשמרות הפעילות בעסק
        public static void CompleteRatingOfAllShifts(string employee_id)
        {
            var e = GetEmployeeById(employee_id);
            var l_rating = RatingBL.GetAllRating(employee_id);
            var l_active_shifts = Shifts_EmployeesBL.GetActiveShifts(e.business_id);
            if (l_active_shifts.Count != l_rating.Count)
            {
                while (l_rating.Count < l_active_shifts.Count)
                {
                    foreach (var item in l_active_shifts)
                    {
                        if (!l_rating.Any(x => x.shift_in_day == item.id))
                        {
                            RatingEntity r = new RatingEntity() { employee_id = employee_id, rating = "יכול", shift_in_day = item.id, shift_approved = false, shift_id = item.shift_id };
                            RatingBL.AddRating(r, item.day);
                            ConnectDB.entity.SaveChanges();
                            l_rating = RatingBL.GetAllRating(employee_id);
                        }
                    }
                }
            }
        }

        //פונקציה לשליחת אמייל , הפונקציה מקבלת את הנושא ואת המסר לשליחה
        public static void SendEmailOfQuestion(List<EmployeesEntity> l, string subject, string message)
        {
            foreach (var item in l)
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("shiftssystem98@gmail.com");
                    mail.To.Add(item.email);
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
    }
}
