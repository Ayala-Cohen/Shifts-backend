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
        public static  Dictionary<int, Dictionary<string, IGrouping<string, Rating>>> dic_shift_rating = new Dictionary<int, Dictionary<string, IGrouping<string, Rating>>>();//מילון המכיל כמפתח קוד משמרת וכערך מילון שמכיל את רשימת הדירוגים מקובצת לפי דירוג

        //פונקציה לשליפת שיבוץ סופי
        public static List<AssigningEntity> GetAssigning(int business_id)
        {
            List<Assigning> l = ConnectDB.entity.Assigning.ToList();
            if (l != null)
                return (AssigningEntity.ConvertListDBToListEntity(l));
            return null;
        }
        //יצירת מילון שיכיל כמפתח קוד משמרת ולכל משמרת יישמר מילון של הדירוגים מקובץ לפי דירוג
        public static void CreateDicShifts()
        {
            var grouped_by_shift = ConnectDB.entity.Rating.GroupBy(x => x.Shift_In_Day).ToDictionary(x => x.Key);//רשימת הדירוגים מקובצת לפי משמרות
            //יצירת מילון שיכיל כמפתח קוד משמרת ולכל משמרת יישמר מילון של הדירוגים מקובץ לפי דירוג
            foreach (var item in grouped_by_shift)
            {
                var grouped_by_rating = item.Value.GroupBy(x => x.Rating1).ToDictionary(x => x.Key);
                dic_shift_rating.Add(item.Key, grouped_by_rating);
            }
        }
        //פונקציית שיבוץ
        public static List<AssigningEntity> AssigningActivity(int business_id)
        {
            dic_of_satisfaction = EmployeesBL.CreateDictionaryOfSatisfaction(business_id);
            CreateDicShifts();
            //var e = EmployeesBL.GetOptimalEmployee(7, 1033, 2);
            var list_departments = DepartmentsBL.GetAllDepartments(business_id);//רשימת המחלקות בעסק
            var list_employees_of_business = EmployeesBL.GetAllEmployees(business_id);//רשימת העובדים בעסק
            foreach (var dep in list_departments)//מעבר על רשימת המחלקות
            {
                foreach (var shift in DepartmentsBL.GetShiftForDepartment(dep.id))//מעבר על רשימת משמרות למחלקה
                {
                    var list_roles_for_shift = Shifts_EmployeesBL.GetRolesForSpecificShift(business_id, shift.id, dep.id);//שליפת התפקידים במשמרת זו
                    foreach (var item in list_roles_for_shift)
                    {
                        var l_employees = EmployeesBL.GetOptimalEmployee(shift.id, item.role_id, item.number_of_shift_employees);
                        if(l_employees.Count==0)//אם אין עובד שיכול לעבוד במשמרת הספציפית הזו
                        {
                            foreach (var c in dic_shift_rating[shift.id].Where(y=>y.Key=="יכול"||y.Key=="מעדיף"))
                            {

                            }
                        }
                        foreach (var e in l_employees)
                        {
                            currentAssigning.Add(new AssigningEntity { department_id = dep.id, employee_id = e.id, shift_in_day_id = shift.id });
                            Rating r = ConnectDB.entity.Rating.FirstOrDefault(x => x.Employee_ID == e.id && shift.id == x.Shift_In_Day);
                            //עדכון לטבלת שביעות רצון ע"מ שלא נתחשב פעמים באותו עובד
                            r.Shift_Approved = true;
                            ConnectDB.entity.SaveChanges();
                        }
                    }
                }

            }
            return GetAssigning(business_id);
        }
    }
}
