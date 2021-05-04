using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Entity;

namespace BL
{
    public class AssigningBL
    {

        public static Dictionary<string, Dictionary<int, int>> dic_of_satisfaction;
        public static List<AssigningEntity> currentAssigning = new List<AssigningEntity>();
        public static Dictionary<int, Dictionary<string, IGrouping<string, Rating>>> dic_shift_rating = new Dictionary<int, Dictionary<string, IGrouping<string, Rating>>>();//מילון המכיל כמפתח קוד משמרת וכערך מילון שמכיל את רשימת הדירוגים מקובצת לפי דירוג


        //פונקציה לשליפת שיבוץ סופי
        public static List<AssigningEntity> GetAssigning(int business_id)
        {
            try
            {
                List<Departments> departments = ConnectDB.entity.Departments.Where(x => x.Business_Id == business_id).ToList();
                List<Employees> employees = ConnectDB.entity.Employees.Where(x => x.Business_Id == business_id).ToList();
                List<Shifts> shifts = ConnectDB.entity.Shifts.Where(x => x.Business_Id == business_id).ToList();
                List<Shifts_In_Days> shifts_In_Days = ConnectDB.entity.Shifts_In_Days.ToList();
                shifts_In_Days = shifts_In_Days.Where(x => shifts.Any(y => y.ID == x.Shift_ID)).ToList();
                List<Assigning> l = ConnectDB.entity.Assigning.ToList();
                l = l.Where(x => departments.Any(y => y.ID == x.Department_ID) && employees.Any(y => y.ID == x.Employee_ID)
                && shifts_In_Days.Any(y => y.ID == x.Shift_In_Day_ID)).ToList();
                return (AssigningEntity.ConvertListDBToListEntity(l));
            }
            catch (Exception)
            {
                return null;
            }

        }
        //יצירת מילון שיכיל כמפתח קוד משמרת ולכל משמרת יישמר מילון של הדירוגים מקובץ לפי דירוג
        public static void CreateDicShifts(int business_id)
        {
            var shifts_in_day = ShiftsBL.GetAllShiftsForDay(business_id);
            //רשימת הדירוגים מקובצת לפי משמרות
            var grouped_by_shift = ConnectDB.entity.Rating.ToList().Where
            (x => shifts_in_day.Any(y => y.id == x.Shift_In_Day)).GroupBy(x => x.Shift_In_Day).ToDictionary(x => x.Key);
            //יצירת מילון שיכיל כמפתח קוד משמרת ולכל משמרת יישמר מילון של הדירוג
            foreach (var item in grouped_by_shift)
            {
                var grouped_by_rating = item.Value.GroupBy(x => x.Rating1).ToDictionary(x => x.Key);
                dic_shift_rating.Add(item.Key, grouped_by_rating);
            }
        }
        //פונקציה לעדכון האם משמרת התקבלה או לא
        public static void UpdateShiftApproved(bool status, string employee_id, int shift_id)
        {
            var r = ConnectDB.entity.Rating.FirstOrDefault(x => x.Employee_ID == employee_id && shift_id == x.Shift_In_Day);
            r.Shift_Approved = status;
            ConnectDB.entity.SaveChanges();
        }
        //פונקציית שיבוץ
        public static List<AssigningEntity> AssigningActivity(int business_id)
        {
            //יצירת מילון שביעות רצון לכל עובד
            dic_of_satisfaction = EmployeesBL.CreateDictionaryOfSatisfaction(business_id);
            //משתנים שמכילים את מספר המשמרות המינימלי ההכרחי לביצוע וכן את אורך הרשימה של העובדים האופטימליים לשיבוץ
            int min_for_performing, len_of_optimal;
            //משתנה שמסמל האם נהיה מוכרחים לשבץ גם עובדים שאינם יכולים או מעדיפים שלא
            bool is_last_option = false;
            //משתנה שיכיל בדיקה האם שיבצנו רק עובדים שיכלו
            bool is_found_with_only_can_employees = false;
            //משתנה שיכיל נתוני שיבוץ שאותו ננסה להחליף
            AssigningEntity earlier_assigning;
            //רשימה שתכיל את השיבוצים של העובדים שניסינו להחליף אותם אך לא הצלחנו
            List<AssigningEntity> local_assigning = new List<AssigningEntity>();
            //רשימת העובדים בעסק
            List<EmployeesEntity> l_employees_of_business = EmployeesBL.GetAllEmployees(business_id);
            //רשימה שתכיל את העובדים להחלפה במשמרת כלשהי
            List<EmployeesEntity> l_for_replace = new List<EmployeesEntity>();
            //בתחילת שיבוץ, אם עובד לא דירג את כל המשמרות נשלים לו אותן בדירוג יכול
            foreach (var item in l_employees_of_business)
            {
                EmployeesBL.CompleteRatingOfAllShifts(item.id);
            }
            //יצירת מילון שמכיל כמפתח קוד משמרת ותחתיו את הדירוגים לאותה המשמרת
            CreateDicShifts(business_id);
            //רשימת המחלקות בעסק
            var list_departments = DepartmentsBL.GetAllDepartments(business_id);
            //מעבר על רשימת המחלקות
            foreach (var dep in list_departments)
            {
                var l_shifts_to_department = DepartmentsBL.GetShiftForDepartment(dep.id);
                //מעבר על רשימת משמרות למחלקה
                foreach (var shift_in_day in l_shifts_to_department)
                {
                    //שליפת התפקידים במשמרת זו
                    var list_roles_for_shift = Shifts_EmployeesBL.GetRolesForSpecificShift(business_id, shift_in_day.id, dep.id);
                    //מעבר על רשימת התפקידים
                    foreach (var role in list_roles_for_shift)
                    {
                        //בתחילת כל שיבוץ קודם כל נשבץ את אלו שיכולים ולכן נאתחל משתנה זה כלא מתקיים
                        is_last_option = false;
                        //מספר העובדים מהתפקיד הנדרשים לכל משמרת
                        min_for_performing = role.number_of_shift_employees;
                        //שליפת רשימת העובדים האופטימליים לתפקיד זה
                        var l_employees = EmployeesBL.GetOptimalEmployee(shift_in_day.id, role.role_id, min_for_performing, is_last_option);
                        //אורך רשימת האופטימליים על מנת לבדוק שאכן עמדנו במספר הנדרש
                        len_of_optimal = l_employees.Count;
                        //שליפת הדירוגים מעדיף ויכול למשמרת זו
                        var dic_rating_can_prefere = dic_shift_rating[shift_in_day.id].Where(y => y.Key == "יכול" || y.Key == "מעדיף");
                        //כל עוד לא שובצו עובדים כמספר המינימלי הנדרש
                        while (len_of_optimal != min_for_performing)
                        {
                            //אם בפעם הראשונה של השיבוץ לא נמצאו עובדים כמכסה המינימלית
                            if (currentAssigning.Count == 0)
                            {
                                //עדכון משתנה "אין ברירה" לחיובי
                                is_last_option = true;
                                //שליחה שוב למציאת עובדים אופטימליים כאשר כרגע יאפשר לשבץ גם עובדים שאינם יכולים 
                                l_employees = EmployeesBL.GetOptimalEmployee(shift_in_day.id, 
                                    role.role_id, min_for_performing, is_last_option);
                                len_of_optimal = l_employees.Count;
                            }
                            else//לא מדובר בפעם הראשונה ולא עמדנו במספר הנדרש
                            {
                                //מעבר על השיבוץ המקומי על מנת לבדוק האם 
                                //ישנו עובד שאם נזיז אותו למשמרת אחרת נצליח לשבץ את מספר העובדים הנדרש
                                foreach (var a in currentAssigning.ToList())
                                {
                                    //שליפת הדירוגים של העובד שעליו אנו מבצעים את 
                                    //הבדיקה בתנאי שהוא לא מועמד מלכתחילה לשיבוץ במשמרת זו
                                    var l_assigned_with_high_rating = dic_rating_can_prefere.Where
                                      (x => x.Value.Any(y => y.Employee_ID == a.employee_id)
                                      && !l_employees.Any(y => y.id == a.employee_id));
                                    //הגבלה לשורה הקודמת - רק אם העובד מאותו התפקיד שאנו מנסים לשבץ כרגע
                                    l_assigned_with_high_rating = l_assigned_with_high_rating.
                                    Where(x => EmployeesBL.GetEmployeeById(a.employee_id).role_id == role.role_id);
                                    // בדיקה שאכן לעובד הנוכחי היה דירוג גבוה למשמרת זו
                                    if (l_assigned_with_high_rating.Count() != 0)
                                    {
                                        //שליחה לפונקציה על מנת לקבל רשימת עובדים להחלפה במשמרת של העובד שנרצה להחליפו
                                        l_for_replace = EmployeesBL.GetOptimalEmployee(a.shift_in_day_id, role.role_id,
                                            role.number_of_shift_employees, is_last_option);
                                        //בדיקה האם במשמרת של העובד שאותו נזיז יש מישהו להחלפה
                                        if (l_for_replace.Count() != 0)
                                        {
                                            earlier_assigning = a;//שמירת הערך מרשימת השיבוץ המקומי למשתנה
                                            currentAssigning.Remove(a);//הסרתו מהרשימה
                                            //עדכון שהמשמרת לא התקבלה בסופו של דבר
                                            UpdateShiftApproved(false, earlier_assigning.employee_id, shift_in_day.id);
                                            var employee_for_replace = l_for_replace.First();
                                            //הוספה לשיבוץ המקומי
                                            currentAssigning.Add(new AssigningEntity() { shift_in_day_id = a.shift_in_day_id,
                                                employee_id = employee_for_replace.id, department_id = a.department_id });
                                            UpdateShiftApproved(true, employee_for_replace.id, a.shift_in_day_id);
                                            //חיפוש לאחר השינוי
                                            l_employees = EmployeesBL.GetOptimalEmployee(shift_in_day.id,
                                                role.role_id, min_for_performing, is_last_option);
                                            len_of_optimal = l_employees.Count;
                                            if (len_of_optimal == min_for_performing)//ההחלפה הועילה
                                            {
                                                //הצלחנו למצוא עובדים בלי לשבץ כאלה שאינם יכולים
                                                is_found_with_only_can_employees = true;
                                                break;//יציאה מלולאת החיפוש על מנת לעבור לתפקיד הבא
                                            }
                                            else//לא הועילה ההחלפה
                                            {
                                                //נוסיף משתנה זה לרשימה מקומית כדי שלא
                                                // תווצר לולאה אינסופית מכיון שרשימת השיבוץ מתעדכנת כל הזמן
                                                local_assigning.Add(earlier_assigning);
                                                UpdateShiftApproved(true, earlier_assigning.employee_id, earlier_assigning.shift_in_day_id);
                                                // הסרת העובד החדש ששבצנו מרשימת השיבוץ
                                                currentAssigning.Remove(currentAssigning.First(x => x.shift_in_day_id == 
                                                    earlier_assigning.shift_in_day_id && x.employee_id == employee_for_replace.id));
                                                UpdateShiftApproved(false, employee_for_replace.id, earlier_assigning.shift_in_day_id);
                                            }
                                        }
                                    }
                                    //אם החלפה לא הועילה או שאין אפשרות להחליף
                                    if (a == currentAssigning.Last() || l_for_replace.Count() == 0)
                                    {
                                        is_last_option = true;//עדכון משתנה "אין ברירה" לחיובי
                                        //אם הפונקציה מגיעה לכאן הווי אומר שאי אפשר להתחשב בשום
                                        // צורה בעובדים שדרגו משמרת כמעדיף שלא ולא יכול ולכן נאלץ לשבץ 
                                        //(אותם על אף שהם אינם יכולים (כמובן שנשבץ את מי שהיה הכי מרוצה מביניהם
                                        l_employees = EmployeesBL.GetOptimalEmployee(shift_in_day.id, role.role_id,
                                            min_for_performing, is_last_option);
                                        len_of_optimal = l_employees.Count;
                                    }
                                }
                                if (!is_found_with_only_can_employees)//נאלצנו לשבץ עובדים שאינם יכולים
                                    //החזרת המצב לקדמותו - רק אם ההחלפה לא הועילה
                                    currentAssigning.AddRange(local_assigning);
                            }
                        }
                        //עדכון בשיבוץ המקומי כאשר נמצאו עובדים לשיבוץ 
                        foreach (var e in l_employees)
                        {
                            currentAssigning.Add(new AssigningEntity { department_id = dep.id, employee_id = e.id, 
                                shift_in_day_id = shift_in_day.id });
                            UpdateShiftApproved(true, e.id, shift_in_day.id);
                            //עדכון לטבלת שביעות רצון ע"מ שלא נתחשב פעמים באותו עובד
                            //הוספה מקומית למילון - הגדלה של רמת השביעות רצון 
                            //בסוף השיבוץ נכניס גם לדאטה בייס
                            dic_of_satisfaction[e.id][1]++;
                        }
                    }
                }
            }
            foreach (var item in l_employees_of_business)
            {
                //לנסות לנתח אם יכול לקרות
                if (!EmployeesBL.CheckIfAssignedInAllShifts(item.id))
                    break;//לטפל בצורה אחרת
            }
            //בסיום השיבוץ - הכנסת הנתונים לדאטה בייס
            foreach (var item in currentAssigning)
            {
                //עדכון טבלת שיבוץ סופי
                ConnectDB.entity.Assigning.Add(AssigningEntity.ConvertEntityToDB(item));
                ConnectDB.entity.Business.First(b => b.ID == business_id).LastAssigningDate = DateTime.Today;
                //ConnectDB.entity.SaveChanges();
                //עדכון טבלת שביעות רצון
                RatingBL.updateStatus();
            }
            ConnectDB.entity.SaveChanges();
            return GetAssigning(business_id);
        }
        //פונקציה לשליפת רשימת העובדים שיכולים או
        // מעדיפים משמרת על מנת שהמנהל יוכל להחליף בצורה ידנית
        public static List<EmployeesEntity> GetEmployeesWithHighRating(int business_id,
            int shift_id, int role_id)
        {
            List<EmployeesEntity> l_employees_of_business = EmployeesBL.GetAllEmployees(business_id);
            List<EmployeesEntity> l_suitable = new List<EmployeesEntity>();
            //שליפת הדירוגים מעדיף ויכול למשמרת זו
            var dic_rating_can_prefere = dic_shift_rating[shift_id].Where(y => y.Key == "יכול" || y.Key == "מעדיף");
            foreach (var item in l_employees_of_business)
            {
                //בדיקה שהעובד אכן רוצה משמרת זו וכן שתפקידו זהה לתפקיד הנדרש
                if (item.role_id == role_id && dic_rating_can_prefere.Where(x =>
                    x.Value.Any(y => y.Employee_ID == item.id)) != null) 
                {
                    //בדיקה שלא משובץ כבר במשמרת אחרת
                    if (!EmployeesBL.CheckIfAssignedInShift(shift_id, item.id))
                        l_suitable.Add(item);
                }
            }
            return l_suitable;
        }
    }
}
