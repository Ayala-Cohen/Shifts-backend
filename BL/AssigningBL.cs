﻿using System;
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

        //פונקציה לבדיקה האם שיבוץ שייך לעסק מסוים
        public static bool CheckIfOfBusiness(int business_id, Assigning assigning)
        {
            if (ConnectDB.entity.Departments.FirstOrDefault(x => x.ID == assigning.Department_ID).Business_Id != business_id)
                return false;
            if (ConnectDB.entity.Employees.FirstOrDefault(x => x.ID == assigning.Employee_ID).Business_Id != business_id)
                return false;
            if (ConnectDB.entity.Shifts.FirstOrDefault(x => x.ID == ConnectDB.entity.Shifts_In_Days.FirstOrDefault(y => y.ID == assigning.Shift_In_Day_ID).Shift_ID).Business_Id != business_id)
                return false;
            return true;
        }

        //פונקציה לשליפת שיבוץ סופי
        public static List<AssigningEntity> GetAssigning(int business_id)
        {
            List<Assigning> l = ConnectDB.entity.Assigning.Where(x=>CheckIfOfBusiness(business_id, x)).ToList();
            if (l != null)
                return (AssigningEntity.ConvertListDBToListEntity(l));
            return null;
        }
        //יצירת מילון שיכיל כמפתח קוד משמרת ולכל משמרת יישמר מילון של הדירוגים מקובץ לפי דירוג
        public static void CreateDicShifts()
        {
            var grouped_by_shift = ConnectDB.entity.Rating.GroupBy(x => x.Shift_In_Day).ToDictionary(x => x.Key);//רשימת הדירוגים מקובצת לפי משמרות
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
            dic_of_satisfaction = EmployeesBL.CreateDictionaryOfSatisfaction(business_id);
            CreateDicShifts();
            int min_for_performing, len_of_optimal;
            bool is_last_option = false, is_found_with_only_can_employees = false;
            AssigningEntity earlier_assigning;
            List<AssigningEntity> local_assigning = new List<AssigningEntity>();
            List<EmployeesEntity> l_employees_of_business = EmployeesBL.GetAllEmployees(business_id);
            foreach (var item in l_employees_of_business)
            {
                EmployeesBL.CompleteRatingOfAllShifts(item.id);
            }
            var list_departments = DepartmentsBL.GetAllDepartments(business_id);//רשימת המחלקות בעסק
            foreach (var dep in list_departments)//מעבר על רשימת המחלקות
            {
                var l_shifts_to_department = DepartmentsBL.GetShiftForDepartment(dep.id);
                foreach (var shift in l_shifts_to_department)//מעבר על רשימת משמרות למחלקה
                {
                    var list_roles_for_shift = Shifts_EmployeesBL.GetRolesForSpecificShift(business_id, shift.id, dep.id);//שליפת התפקידים במשמרת זו
                    foreach (var role in list_roles_for_shift)//מעבר על רשימת התפקידים
                    {
                        is_last_option = false;//האם הגענו למצב "אין ברירה" ולכן נהיה מוכרחים לשבץ גם כאלה שאינם יכולים
                        min_for_performing = role.number_of_shift_employees;//מספר העובדים מהתפקיד הנדרשים לכל משמרת
                        var l_employees = EmployeesBL.GetOptimalEmployee(shift.id, role.role_id, min_for_performing, is_last_option);//שליפת רשימת העובדים האופטימליים לתפקיד זה
                        len_of_optimal = l_employees.Count;//אורך רשימת האופטימליים על מנת לבדוק שאכן עמדנו במספר הנדרש
                        var dic_rating_can_prefere = dic_shift_rating[shift.id].Where(y => y.Key == "יכול" || y.Key == "מעדיף");//שליפת הדירוגים מעדיף ויכול למשמרת זו
                        while (len_of_optimal != min_for_performing)//כל עוד לא שובצו עובדים כמספר המינימלי הנדרש
                        {
                            if (currentAssigning.Count == 0)//אם בפעם הראשונה של השיבוץ לא נמצאו עובדים כמכסה המינימלית
                            {
                                is_last_option = true;//עדכון משתנה "אין ברירה" לחיובי
                                l_employees = EmployeesBL.GetOptimalEmployee(shift.id, role.role_id,min_for_performing, is_last_option);//(שליחה שוב למציאת עובדים אופטימליים כאשר כרגע יאפשר לשבץ גם עובדים שאינם יכולים (הרע במיעוטו
                                len_of_optimal = l_employees.Count;
                            }
                            else//לא מדובר בפעם הראשונה ולא עמדנו במספר הנדרש
                            {
                                foreach (var a in currentAssigning.ToList())//מעבר על השיבוץ המקומי על מנת לבדוק האם ישנו עובד שאם נזיז אותו למשמרת אחרת נצליח לשבץ את מספר העובדים הנדרש
                                {
                                    var l_assigned_with_high_rating = dic_rating_can_prefere.Where(x => x.Value.Any(y => y.Employee_ID == a.employee_id) && !l_employees.Any(y=>y.id == a.employee_id));//שליפת הדירוגים של העובד שעליו אנו מבצעים את הבדיקה בתנאי שהוא לא מועמד מלכתחילה לשיבוץ במשמרת זו
                                    l_assigned_with_high_rating = l_assigned_with_high_rating.Where(x => EmployeesBL.GetEmployeeById(a.employee_id).role_id == role.role_id);//הגבלה לשורה הקודמת - רק אם העובד מאותו התפקיד שאנו מנסים לשבץ כרגע
                                    if (l_assigned_with_high_rating.Count() != 0)// בדיקה שאכן לעובד הנוכחי היה דירוג גבוה למשמרת זו
                                    {
                                        earlier_assigning = a;//שמירת הערך מרשימת השיבוץ המקומי למשתנה
                                        currentAssigning.Remove(a);//הסרתו מהרשימה
                                        //להוסיף החלפה - רק אם יש עובד שמתאים לעבוד במשמרת שהוצאה להחליף
                                        UpdateShiftApproved(false, earlier_assigning.employee_id, shift.id);//עדכון שהמשמרת לא התקבלה בסופו של דבר
                                        l_employees = EmployeesBL.GetOptimalEmployee(shift.id, role.role_id, min_for_performing, is_last_option);//חיפוש לאחר השינוי
                                        len_of_optimal = l_employees.Count;
                                        if (len_of_optimal == min_for_performing)//ההחלפה הועילה
                                        {
                                            is_found_with_only_can_employees = true;//הצלחנו למצוא עובדים בלי לשבץ כאלה שאינם יכולים
                                            break;
                                        }
                                        else//לא הועילה ההחלפה
                                            local_assigning.Add(earlier_assigning);//נוסיף משתנה זה לרשימה מקומית כדי שלא תווצר לולאה אינסופית מכיון שרשימת השיבוץ מתעדכנת כל הזמן
                                    }
                                    if (a == currentAssigning.Last())//אם החלפה לא הועילה
                                    {
                                        is_last_option = true;//עדכון משתנה "אין ברירה" לחיובי
                                        //אם הפונקציה מגיעה לכאן הווי אומר שאי אפשר להתחשב בשום צורה בעובדים שדרגו משמרת כמעדיף שלא ולא יכול
                                        //(ולכן נאלץ לשבץ אותם על אף שהם אינם יכולים (כמובן שנשבץ את מי שהיה הכי מרוצה מביניהם
                                        l_employees = EmployeesBL.GetOptimalEmployee(shift.id, role.role_id, min_for_performing, is_last_option);
                                        len_of_optimal = l_employees.Count;
                                    }
                                }
                                if (!is_found_with_only_can_employees)//נאלצנו לשבץ עובדים שאינם יכולים
                                    currentAssigning.AddRange(local_assigning);//החזרת המצב לקדמותו - רק אם ההחלפה לא הועילה
                            }
                        }
                        foreach (var e in l_employees)//עדכון בשיבוץ המקומי כאשר נמצאו עובדים לשיבוץ 
                        {
                            currentAssigning.Add(new AssigningEntity { department_id = dep.id, employee_id = e.id, shift_in_day_id = shift.id });
                            UpdateShiftApproved(true, e.id, shift.id);
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
                if (!EmployeesBL.CheckIfAssignedInAllShifts(item.id))//לנסות לנתח אם יכול לקרות
                    break;//לטפל בצורה אחרת
                //?מקרה קצה בו העובד אינו יכול לבצע את כל המשמרות שבהן מחויב - מה לעשות
                //לתת למנהל רשימה של עובדים שיכולים או מעדיפים על מנת שידע את מי להחליף
                //ולבדוק שלא משובצים במחלקה אחרת
            }
            //בסיום השיבוץ - הכנסת הנתונים לדאטה בייס
            foreach (var item in currentAssigning)
            {
                ConnectDB.entity.Assigning.Add(AssigningEntity.ConvertEntityToDB(item));//עדכון טבלת שיבוץ סופי
                RatingBL.updateStatus();//עדכון טבלת שביעות רצון
            }
            ConnectDB.entity.SaveChanges();
            return GetAssigning(business_id);
        }

        //פונקציה לשליפת רשימת העובדים שיכולים או מעדיפים משמרת על מנת שהמנהל יוכל להחליף בצורה ידנית
        public static List<EmployeesEntity> GetEmployeesWithHighRating(int business_id, int shift_id, int role_id)
        {
            List<EmployeesEntity> l_employees_of_business = EmployeesBL.GetAllEmployees(business_id);
            List<EmployeesEntity> l_suitable = new List<EmployeesEntity>();
            var dic_rating_can_prefere = dic_shift_rating[shift_id].Where(y => y.Key == "יכול" || y.Key == "מעדיף");//שליפת הדירוגים מעדיף ויכול למשמרת זו
            foreach (var item in l_employees_of_business)
            {
                if (item.role_id == role_id && dic_rating_can_prefere.Where(x => x.Value.Any(y => y.Employee_ID == item.id)) != null) //בדיקה שהעובד אכן רוצה משמרת זו וכן שתפקידו זהה לתפקיד הנדרש
                {
                    if(!EmployeesBL.CheckIfAssignedInShift(shift_id, item.id))//בדיקה שלא משובץ כבר במשמרת אחרת
                        l_suitable.Add(item);
                }
            }
            return l_suitable;
        }
    }
}
