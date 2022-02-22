using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static List<AssigningEntity> localAssigning = new List<AssigningEntity>();
        public static Dictionary<int, Dictionary<string, IGrouping<string, RatingEntity>>> dic_shift_rating = new Dictionary<int, Dictionary<string, IGrouping<string, RatingEntity>>>();//מילון המכיל כמפתח קוד משמרת וכערך מילון שמכיל את רשימת הדירוגים מקובצת לפי דירוג


        //פונקציה לשליפת שיבוץ סופי
        public static List<AssigningEntity> GetAssigning(int business_id)
        {
            return AssigningEntity.ConvertListDBToListEntity(AssigningDal.GetAssigning(business_id));
        }
        //יצירת מילון שיכיל כמפתח קוד משמרת ולכל משמרת יישמר מילון של הדירוגים מקובץ לפי דירוג
        public static void CreateDicShifts(int business_id)
        {
            try
            {
                if (dic_shift_rating.Count == 0)
                {
                    var shifts_in_day = ShiftsBL.GetAllShiftsForDay(business_id);
                    if (shifts_in_day != null)
                    {
                        var grouped_by_shift = RatingBL.GetAllRatings().Where(x => shifts_in_day.Any(y => y.id == x.shift_in_day)).GroupBy(x => x.shift_in_day).ToDictionary(x => x.Key);//רשימת הדירוגים מקובצת לפי משמרות
                                                                                                                                                                                         //יצירת מילון שיכיל כמפתח קוד משמרת ולכל משמרת יישמר מילון של הדירוג
                        foreach (var item in grouped_by_shift)
                        {
                            var grouped_by_rating = item.Value.GroupBy(x => x.rating).ToDictionary(x => x.Key);
                            dic_shift_rating.Add(item.Key, grouped_by_rating);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

        }
        //פונקציה לעדכון האם משמרת התקבלה או לא
        public static void UpdateShiftApproved(bool status, string employee_id, int shift_id)
        {
            var r = RatingBL.GetRatingById(employee_id, shift_id);
            r.shift_approved = status;
            RatingBL.UpdateRating(r);
        }
        public static List<EmployeesEntity> GetEmployeesListInNoChoice(int shift_in_day_id, int role_id, int min_for_assigning_in_shift)
        {
            var l_optimal_employees = EmployeesBL.GetOptimalEmployee(shift_in_day_id, role_id, min_for_assigning_in_shift, true);//שליחה שוב למציאת עובדים אופטימליים כאשר כרגע יאפשר לשבץ גם עובדים שאינם יכולים 
            return l_optimal_employees;
        }
        //פונקציית שיבוץ
        public static List<AssigningEntity> AssigningActivity(int business_id)
        {
            //מחיקת טבלת שיבוץ 
            ClearAssigning(business_id);
            dic_of_satisfaction = EmployeesBL.CreateDictionaryOfSatisfaction(business_id);//יצירת מילון שביעות רצון לכל עובד
            int min_for_assigning_in_shift, len_of_optimal;//משתנים שמכילים את מספר המשמרות המינימלי ההכרחי לביצוע וכן את אורך הרשימה של העובדים האופטימליים לשיבוץ
            bool is_last_option = false;//משתנה שמסמל האם נהיה מוכרחים לשבץ גם עובדים שאינם יכולים או מעדיפים שלא
            bool is_found_with_only_can_employees = false;//משתנה שיכיל בדיקה האם שיבצנו רק עובדים שיכלו
            AssigningEntity earlier_assigning;//משתנה שיכיל נתוני שיבוץ שאותו ננסה להחליף
            List<AssigningEntity> local_assigning = new List<AssigningEntity>();//רשימה שתכיל את השיבוצים של העובדים שניסינו להחליף אותם אך לא הצלחנו
            List<EmployeesEntity> l_employees_of_business = EmployeesBL.GetAllEmployees(business_id);//רשימת העובדים בעסק
            List<EmployeesEntity> l_for_replace = new List<EmployeesEntity>();//רשימה שתכיל את העובדים להחלפה במשמרת כלשהי
            foreach (var item in l_employees_of_business)//בתחילת שיבוץ, אם עובד לא דירג את כל המשמרות נשלים לו אותן בדירוג יכול
            {
                EmployeesBL.CompleteRatingOfAllShifts(item.id);
            }
            CreateDicShifts(business_id);//יצירת מילון שמכיל כמפתח קוד משמרת ותחתיו את הדירוגים לאותה המשמרת
            var list_departments = DepartmentsBL.GetAllDepartments(business_id);//רשימת המחלקות בעסק
            foreach (var dep in list_departments)//מעבר על רשימת המחלקות
            {
                var l_shifts_to_department = DepartmentsBL.GetShiftForDepartment(dep.id);
                foreach (var shift_in_day in l_shifts_to_department)//מעבר על רשימת משמרות למחלקה
                {
                    var list_roles_for_shift = Shifts_EmployeesBL.GetRolesForSpecificShift(business_id, shift_in_day.id, dep.id);//שליפת התפקידים במשמרת זו
                    foreach (var role in list_roles_for_shift)//מעבר על רשימת התפקידים
                    {
                        is_last_option = false;//בתחילת כל שיבוץ קודם כל נשבץ את אלו שיכולים ולכן נאתחל משתנה זה כלא מתקיים
                        min_for_assigning_in_shift = role.number_of_shift_employees;//מספר העובדים מהתפקיד הנדרשים לכל משמרת
                        var l_optimal_employees = EmployeesBL.GetOptimalEmployee(shift_in_day.id, role.role_id, min_for_assigning_in_shift, is_last_option);//שליפת רשימת העובדים האופטימליים לתפקיד זה
                        len_of_optimal = l_optimal_employees.Count;//אורך רשימת האופטימליים על מנת לבדוק שאכן עמדנו במספר הנדרש
                        while (len_of_optimal != min_for_assigning_in_shift)//כל עוד לא שובצו עובדים כמספר המינימלי הנדרש
                        {
                            if (localAssigning.Count == 0) //אם בפעם הראשונה של השיבוץ לא נמצאו עובדים כמכסה המינימלית
                                l_optimal_employees = GetEmployeesListInNoChoice(shift_in_day.id, role.role_id, min_for_assigning_in_shift);
                            else //לא מדובר בפעם הראשונה ולא עמדנו במספר הנדרש
                            {
                                var dic_rating_can_prefere = dic_shift_rating[shift_in_day.id].Where(y => y.Key == RatingBL.Ratings.Can.ToString() || y.Key == RatingBL.Ratings.Prefere.ToString());//שליפת הדירוגים מעדיף ויכול למשמרת זו
                                foreach (var assigning in localAssigning.ToList())//מעבר על השיבוץ המקומי על מנת לבדוק האם ישנו עובד שאם נזיז אותו למשמרת אחרת נצליח לשבץ את מספר העובדים הנדרש
                                {
                                    var l_assigned_with_high_rating = dic_rating_can_prefere.Where(x => x.Value.Any(y => y.employee_id == assigning.employee_id) && !l_optimal_employees.Any(y => y.id == assigning.employee_id));//שליפת הדירוגים של העובד שעליו אנו מבצעים את הבדיקה בתנאי שהוא לא מועמד מלכתחילה לשיבוץ במשמרת זו
                                    l_assigned_with_high_rating = l_assigned_with_high_rating.Where(x => EmployeesBL.GetEmployeeById(assigning.employee_id).role_id == role.role_id);//הגבלה לשורה הקודמת - רק אם העובד מאותו התפקיד שאנו מנסים לשבץ כרגע
                                    if (l_assigned_with_high_rating.Count() != 0)// בדיקה שאכן לעובד הנוכחי היה דירוג גבוה למשמרת זו
                                    {
                                        l_for_replace = EmployeesBL.GetOptimalEmployee(assigning.shift_in_day_id, role.role_id, role.number_of_shift_employees, is_last_option);//שליחה לפונקציה על מנת לקבל רשימת עובדים להחלפה במשמרת של העובד שנרצה להחליפו
                                        if (l_for_replace.Count() != 0)//בדיקה האם במשמרת של העובד שאותו נזיז יש מישהו להחלפה
                                        {
                                            earlier_assigning = assigning;//שמירת הערך מרשימת השיבוץ המקומי למשתנה
                                            localAssigning.Remove(assigning);//הסרתו מהרשימה
                                            UpdateShiftApproved(false, earlier_assigning.employee_id, shift_in_day.id);//עדכון שהמשמרת לא התקבלה בסופו של דבר
                                            RatingBL.UpdateStatusLocal(shift_in_day.id);
                                            var employee_for_replace = l_for_replace.First();
                                            //הוספה לשיבוץ המקומי
                                            localAssigning.Add(new AssigningEntity() { shift_in_day_id = assigning.shift_in_day_id, employee_id = employee_for_replace.id, department_id = assigning.department_id });
                                            UpdateShiftApproved(true, employee_for_replace.id, assigning.shift_in_day_id);
                                            RatingBL.UpdateStatusLocal(shift_in_day.id);
                                            l_optimal_employees = EmployeesBL.GetOptimalEmployee(shift_in_day.id, role.role_id, min_for_assigning_in_shift, is_last_option);//חיפוש לאחר השינוי
                                            len_of_optimal = l_optimal_employees.Count;
                                            if (len_of_optimal == min_for_assigning_in_shift)//ההחלפה הועילה
                                            {
                                                is_found_with_only_can_employees = true;//הצלחנו למצוא עובדים בלי לשבץ כאלה שאינם יכולים
                                                break;//יציאה מלולאת החיפוש על מנת לעבור לתפקיד הבא
                                            }
                                            else//לא הועילה ההחלפה
                                            {
                                                local_assigning.Add(earlier_assigning);//נוסיף משתנה זה לרשימה מקומית כדי שלא תווצר לולאה אינסופית מכיון שרשימת השיבוץ מתעדכנת כל הזמן
                                                UpdateShiftApproved(true, earlier_assigning.employee_id, earlier_assigning.shift_in_day_id);
                                                RatingBL.UpdateStatusLocal(shift_in_day.id);
                                                // הסרת העובד החדש ששבצנו מרשימת השיבוץ
                                                localAssigning.Remove(localAssigning.First(x => x.shift_in_day_id == earlier_assigning.shift_in_day_id && x.employee_id == employee_for_replace.id));
                                                UpdateShiftApproved(false, employee_for_replace.id, earlier_assigning.shift_in_day_id);
                                                RatingBL.UpdateStatusLocal(shift_in_day.id);
                                            }
                                        }
                                    }
                                    //אם החלפה לא הועילה או שאין אפשרות להחליף
                                    if (assigning == localAssigning.Last() || l_for_replace.Count() == 0)
                                    {
                                        is_last_option = true;//עדכון משתנה "אין ברירה" לחיובי
                                        //אם הפונקציה מגיעה לכאן הווי אומר שאי אפשר להתחשב בשום
                                        //צורה בעובדים שדרגו משמרת כמעדיף שלא ולא יכול
                                        //(ולכן נאלץ לשבץ אותם על אף שהם אינם יכולים (כמובן שנשבץ את מי שהיה הכי מרוצה מביניהם
                                        l_optimal_employees = GetEmployeesListInNoChoice(shift_in_day.id, role.role_id, min_for_assigning_in_shift);
                                        len_of_optimal = l_optimal_employees.Count;
                                        if (len_of_optimal == min_for_assigning_in_shift)
                                            break;
                                    }
                                }
                                if (!is_found_with_only_can_employees)//נאלצנו לשבץ עובדים שאינם יכולים
                                    localAssigning.AddRange(local_assigning);//החזרת המצב לקדמותו - רק אם ההחלפה לא הועילה
                            }
                        }
                        foreach (var e in l_optimal_employees)//עדכון בשיבוץ המקומי כאשר נמצאו עובדים לשיבוץ 
                        {
                            localAssigning.Add(new AssigningEntity { department_id = dep.id, employee_id = e.id, shift_in_day_id = shift_in_day.id });
                            UpdateShiftApproved(true, e.id, shift_in_day.id);
                        }
                    }
                    //עדכון לטבלת שביעות רצון ע"מ שלא נתחשב פעמים באותו עובד
                    //הוספה מקומית למילון - הגדלה של רמת השביעות רצון 
                    //בסוף השיבוץ נכניס גם לדאטה בייס
                    RatingBL.UpdateStatusLocal(shift_in_day.id);
                }
            }
            foreach (var item in l_employees_of_business)
            {
                if (!EmployeesBL.IsAssignedInAllShifts(item.id))//לנסות לנתח אם יכול לקרות
                    break;//לטפל בצורה אחרת
            }
            //בסיום השיבוץ - הכנסת הנתונים לדאטה בייס
            foreach (var item in localAssigning)
            {
                AssigningDal.AddAssigning(AssigningEntity.ConvertEntityToDB(item)); //עדכון טבלת שיבוץ סופי
            }
            RatingBL.SetStatus();//עדכון טבלת שביעות רצון

            //אתחול טבלת דרוגים
            ClearRating(business_id);
            //עדכון תאריכי פתיחת יומן וסגירת ימן
            UpdateDicDate(business_id);
            return GetAssigning(business_id);
        }

        //פונקציה לשליפת רשימת העובדים שאינם משובצים במשמרת על מנת שהמנהל יוכל להחליף בצורה ידנית
        public static Dictionary<int, List<EmployeesEntity>> GetEmployeesForReplacing(int business_id, int shift_in_day_id)
        {
            List<EmployeesEntity> l_employees_of_business = EmployeesBL.GetAllEmployees(business_id);
            List<EmployeesEntity> l_suitable = new List<EmployeesEntity>();
            List<Employee_RolesEntity> l_roles = Employees_RoleBL.GetAllEmployeesRoles(business_id);
            Dictionary<int, List<EmployeesEntity>> dic_suitable = new Dictionary<int, List<EmployeesEntity>>();
            try
            {
                foreach (var role in l_roles)
                {
                    l_suitable = new List<EmployeesEntity>();
                    foreach (var employee in l_employees_of_business.Where(x => x.role_id == role.id))
                    {
                        if (!EmployeesBL.CheckIfAssignedInShift(shift_in_day_id, employee.id) && ConnectDB.entity.Assigning.FirstOrDefault(x => x.Employee_ID == employee.id && x.Shift_In_Day_ID == shift_in_day_id) == null)//בדיקה שלא משובץ כבר במשמרת אחרת או במשמרת הנוכחית
                            l_suitable.Add(employee);
                    }
                    dic_suitable.Add(role.id, l_suitable);
                }
                return dic_suitable;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }

        }
        //עריכת שיבוץ
        public static List<AssigningEntity> EditAssiging(AssigningEntity assigning, string employee_id_replacing)
        {
            var assiging_for_editing = ConnectDB.entity.Assigning.First(x => x.Employee_ID == assigning.employee_id && x.Department_ID == assigning.department_id && x.Shift_In_Day_ID == assigning.shift_in_day_id);
            ConnectDB.entity.Assigning.Remove(assiging_for_editing);
            ConnectDB.entity.Assigning.Add(new Assigning { Department_ID = assigning.department_id, Employee_ID = employee_id_replacing, Shift_In_Day_ID = assigning.shift_in_day_id });
            UpdateShiftApproved(true, employee_id_replacing, assigning.shift_in_day_id);
            var s_status_for_delete = ConnectDB.entity.Satisfaction_Status.FirstOrDefault(x => x.Employee_ID == assigning.employee_id && x.Satisfaction_Status1 == 1);
            ConnectDB.entity.Satisfaction_Status.Remove(s_status_for_delete);
            ConnectDB.entity.SaveChanges();
            return GetAssigning(EmployeesBL.GetEmployeeById(assigning.employee_id).business_id);
        }

        //פונקציה להחזרת העובדים המשובצים במשמרת כשהם מחולקים לפי תפקידים
        public static Dictionary<int, List<EmployeesEntity>> GetAssingingByRoles(int shift_in_day_id, int department_id)
        {
            Dictionary<int, List<EmployeesEntity>> dic = new Dictionary<int, List<EmployeesEntity>>();
            int business_id;
            List<AssigningEntity> l_assigning;
            List<EmployeesEntity> l_employees;
            try
            {
                business_id = DepartmentsBL.GetDepartmentById(department_id).business_id;
                l_assigning = GetAssigning(business_id);
                l_employees = EmployeesBL.GetAllEmployees(business_id);
                var current_assigning = l_assigning.Where(x => x.department_id == department_id && x.shift_in_day_id == shift_in_day_id);
                foreach (var role in Employees_RoleBL.GetAllEmployeesRoles(business_id))
                {
                    var current_assigning_from_current_role = current_assigning.Where(x => l_employees.FirstOrDefault(y => y.id == x.employee_id).role_id == role.id);
                    var employees_in_shift = l_employees.Where(x => current_assigning_from_current_role.FirstOrDefault(y => y.employee_id == x.id) != null).ToList();
                    dic.Add(role.id, employees_in_shift);
                }
                return dic;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }

        }

        //אתחול/מחיקה טבלת דרוגים
        public static void ClearRating(int business_id)
        {
            var Employees_in_business = ConnectDB.entity.Employees.Where(e => e.Business_Id == business_id);
            var rating = ConnectDB.entity.Rating.Where(b => Employees_in_business.Contains(b.Employees));
            ConnectDB.entity.Rating.RemoveRange(rating);
            ConnectDB.entity.SaveChanges();
        }

        //אתחול/מחיקה טבלת שיבוץ
        public static void ClearAssigning(int business_id)
        {
            var Employees_in_business = ConnectDB.entity.Employees.Where(e => e.Business_Id == business_id).Select(s => s.ID);
            var assigning = ConnectDB.entity.Assigning.Where(a => Employees_in_business.Contains(a.Employee_ID));
            ConnectDB.entity.Assigning.RemoveRange(assigning);
            ConnectDB.entity.SaveChanges();
        }

        //עדכון פתיחת יומן וסגירת יומן
        public static void UpdateDicDate(int business_id)
        {
            var departments = ConnectDB.entity.Business.FirstOrDefault(b => b.ID == business_id).Departments;
            //הפרש הימים
            double days = (departments.First().Diary_Closing_Day - departments.First().Diary_Opening_Day).TotalDays;
            departments.ToList().ForEach(d => { d.Diary_Opening_Day = d.Diary_Opening_Day.AddDays(days); d.Diary_Closing_Day = d.Diary_Closing_Day.AddDays(days); });
            ConnectDB.entity.SaveChanges();

        }


    }
}