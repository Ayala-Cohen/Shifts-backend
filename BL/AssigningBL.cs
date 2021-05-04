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
                l = l.Where(x => departments.Any(y => y.ID == x.Department_ID) && employees.Any(y => y.ID == x.Employee_ID) && shifts_In_Days.Any(y => y.ID == x.Shift_In_Day_ID)).ToList();
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
            var grouped_by_shift = ConnectDB.entity.Rating.ToList().Where(x => shifts_in_day.Any(y => y.id == x.Shift_In_Day)).GroupBy(x => x.Shift_In_Day).ToDictionary(x => x.Key);//רשימת הדירוגים מקובצת לפי משמרות
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
            dic_of_satisfaction = EmployeesBL.CreateDictionaryOfSatisfaction(business_id);//יצירת מילון שביעות רצון לכל עובד
            int min_for_performing, len_of_optimal;//משתנים שמכילים את מספר המשמרות המינימלי ההכרחי לביצוע וכן את אורך הרשימה של העובדים האופטימליים לשיבוץ
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
                        min_for_performing = role.number_of_shift_employees;//מספר העובדים מהתפקיד הנדרשים לכל משמרת
                        var l_employees = EmployeesBL.GetOptimalEmployee(shift_in_day.id, role.role_id, min_for_performing, is_last_option);//שליפת רשימת העובדים האופטימליים לתפקיד זה
                        len_of_optimal = l_employees.Count;//אורך רשימת האופטימליים על מנת לבדוק שאכן עמדנו במספר הנדרש
                        while (len_of_optimal != min_for_performing)//כל עוד לא שובצו עובדים כמספר המינימלי הנדרש
                        {
                            if (currentAssigning.Count == 0)//אם בפעם הראשונה של השיבוץ לא נמצאו עובדים כמכסה המינימלית
                            {
                                is_last_option = true;//עדכון משתנה "אין ברירה" לחיובי
                                l_employees = EmployeesBL.GetOptimalEmployee(shift_in_day.id, role.role_id, min_for_performing, is_last_option);//שליחה שוב למציאת עובדים אופטימליים כאשר כרגע יאפשר לשבץ גם עובדים שאינם יכולים 
                                len_of_optimal = l_employees.Count;
                            }
                            else//לא מדובר בפעם הראשונה ולא עמדנו במספר הנדרש
                            {
                                foreach (var a in currentAssigning.ToList())//מעבר על השיבוץ המקומי על מנת לבדוק האם ישנו עובד שאם נזיז אותו למשמרת אחרת נצליח לשבץ את מספר העובדים הנדרש
                                {
                                    var dic_rating_can_prefere = dic_shift_rating[shift_in_day.id].Where(y => y.Key == "יכול" || y.Key == "מעדיף");//שליפת הדירוגים מעדיף ויכול למשמרת זו
                                    var l_assigned_with_high_rating = dic_rating_can_prefere.Where(x => x.Value.Any(y => y.Employee_ID == a.employee_id) && !l_employees.Any(y => y.id == a.employee_id));//שליפת הדירוגים של העובד שעליו אנו מבצעים את הבדיקה בתנאי שהוא לא מועמד מלכתחילה לשיבוץ במשמרת זו
                                    l_assigned_with_high_rating = l_assigned_with_high_rating.Where(x => EmployeesBL.GetEmployeeById(a.employee_id).role_id == role.role_id);//הגבלה לשורה הקודמת - רק אם העובד מאותו התפקיד שאנו מנסים לשבץ כרגע
                                    if (l_assigned_with_high_rating.Count() != 0)// בדיקה שאכן לעובד הנוכחי היה דירוג גבוה למשמרת זו
                                    {
                                        l_for_replace = EmployeesBL.GetOptimalEmployee(a.shift_in_day_id, role.role_id, role.number_of_shift_employees, is_last_option);//שליחה לפונקציה על מנת לקבל רשימת עובדים להחלפה במשמרת של העובד שנרצה להחליפו
                                        if (l_for_replace.Count() != 0)//בדיקה האם במשמרת של העובד שאותו נזיז יש מישהו להחלפה
                                        {
                                            earlier_assigning = a;//שמירת הערך מרשימת השיבוץ המקומי למשתנה
                                            currentAssigning.Remove(a);//הסרתו מהרשימה
                                            UpdateShiftApproved(false, earlier_assigning.employee_id, shift_in_day.id);//עדכון שהמשמרת לא התקבלה בסופו של דבר
                                            RatingBL.UpdateStatusLocal(shift_in_day.id);
                                            var employee_for_replace = l_for_replace.First();
                                            //הוספה לשיבוץ המקומי
                                            currentAssigning.Add(new AssigningEntity() { shift_in_day_id = a.shift_in_day_id, employee_id = employee_for_replace.id, department_id = a.department_id });
                                            UpdateShiftApproved(true, employee_for_replace.id, a.shift_in_day_id);
                                            RatingBL.UpdateStatusLocal(shift_in_day.id);
                                            l_employees = EmployeesBL.GetOptimalEmployee(shift_in_day.id, role.role_id, min_for_performing, is_last_option);//חיפוש לאחר השינוי
                                            len_of_optimal = l_employees.Count;
                                            if (len_of_optimal == min_for_performing)//ההחלפה הועילה
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
                                                currentAssigning.Remove(currentAssigning.First(x => x.shift_in_day_id == earlier_assigning.shift_in_day_id && x.employee_id == employee_for_replace.id));
                                                UpdateShiftApproved(false, employee_for_replace.id, earlier_assigning.shift_in_day_id);
                                                RatingBL.UpdateStatusLocal(shift_in_day.id);
                                            }
                                        }
                                    }
                                    if (a == currentAssigning.Last() || l_for_replace.Count() == 0)//אם החלפה לא הועילה או שאין אפשרות להחליף
                                    {
                                        is_last_option = true;//עדכון משתנה "אין ברירה" לחיובי
                                        //אם הפונקציה מגיעה לכאן הווי אומר שאי אפשר להתחשב בשום צורה בעובדים שדרגו משמרת כמעדיף שלא ולא יכול
                                        //(ולכן נאלץ לשבץ אותם על אף שהם אינם יכולים (כמובן שנשבץ את מי שהיה הכי מרוצה מביניהם
                                        l_employees = EmployeesBL.GetOptimalEmployee(shift_in_day.id, role.role_id, min_for_performing, is_last_option);
                                        len_of_optimal = l_employees.Count;
                                    }
                                }
                                if (!is_found_with_only_can_employees)//נאלצנו לשבץ עובדים שאינם יכולים
                                    currentAssigning.AddRange(local_assigning);//החזרת המצב לקדמותו - רק אם ההחלפה לא הועילה
                            }
                        }
                        foreach (var e in l_employees)//עדכון בשיבוץ המקומי כאשר נמצאו עובדים לשיבוץ 
                        {
                            currentAssigning.Add(new AssigningEntity { department_id = dep.id, employee_id = e.id, shift_in_day_id = shift_in_day.id });
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
                if (!EmployeesBL.CheckIfAssignedInAllShifts(item.id))//לנסות לנתח אם יכול לקרות
                    break;//לטפל בצורה אחרת
            }
            //בסיום השיבוץ - הכנסת הנתונים לדאטה בייס
            foreach (var item in currentAssigning)
            {
                ConnectDB.entity.Assigning.Add(AssigningEntity.ConvertEntityToDB(item));//עדכון טבלת שיבוץ סופי
                RatingBL.SetStatus();//עדכון טבלת שביעות רצון
            }
            ConnectDB.entity.SaveChanges();
            return GetAssigning(business_id);
        }

        //פונקציה לשליפת רשימת העובדים שיכולים או מעדיפים משמרת על מנת שהמנהל יוכל להחליף בצורה ידנית
        public static Dictionary<int, List<EmployeesEntity>> GetEmployeesWithHighRating(int business_id, int shift_id)
        {
            List<EmployeesEntity> l_employees_of_business = EmployeesBL.GetAllEmployees(business_id);
            List<EmployeesEntity> l_suitable = new List<EmployeesEntity>();
            List<Employee_RolesEntity> l_roles = Employees_RoleBL.GetAllEmployeesRoles(business_id);
            Dictionary<int, List<EmployeesEntity>> dic_suitable = new Dictionary<int, List<EmployeesEntity>>();
            var dic_rating_can_prefere = dic_shift_rating[shift_id].Where(y => y.Key == "יכול" || y.Key == "מעדיף");//שליפת הדירוגים מעדיף ויכול למשמרת זו
            foreach (var role in l_roles)
            {
                foreach (var item in l_employees_of_business)
                {
                    if (item.role_id == role.id && dic_rating_can_prefere.Where(x => x.Value.Any(y => y.Employee_ID == item.id)) != null) //בדיקה שהעובד אכן רוצה משמרת זו וכן שתפקידו זהה לתפקיד הנדרש
                    {
                        if (!EmployeesBL.CheckIfAssignedInShift(shift_id, item.id))//בדיקה שלא משובץ כבר במשמרת אחרת
                            l_suitable.Add(item);
                    }
                }
                dic_suitable.Add(role.id, l_suitable);
            }

            return dic_suitable;
        }
    }
}
