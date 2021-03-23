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
        //פונקציה לשליפת שיבוץ סופי
        public static List<AssigningEntity> GetAssigning(int business_id)
        {
            List<Assigning> l = ConnectDB.entity.Assigning.ToList();
            if (l != null)
                return (AssigningEntity.ConvertListDBToListEntity(l));
            return null;
        }

        //פונקציית שיבוץ
        public static List<AssigningEntity> AssigningActivity(int business_id)
        {
            dic_of_satisfaction = EmployeesBL.CreateDictionaryOfSatisfaction(business_id);
            
            var e = EmployeesBL.GetOptimalEmployee(7, 1033, 2);
            var list_departments = DepartmentsBL.GetAllDepartments(business_id);//רשימת המחלקות בעסק
            var list_employees_of_business = EmployeesBL.GetAllEmployees(business_id);//רשימת העובדים בעסק
            foreach (var dep in list_departments)//מעבר על רשימת המחלקות
            {
                foreach (var shift in DepartmentsBL.GetShiftForDepartment(dep.id))//מעבר על רשימת משמרות למחלקה
                {
                    var list_roles_for_shift = Shifts_EmployeesBL.GetRolesForSpecificShift(business_id, shift.id, dep.id);//שליפת התפקידים במשמרת זו
                    foreach (var item in list_roles_for_shift)
                    {
                        //var e = EmployeesBL.GetOptimalEmployee(shift.id, item.role_id, item.number_of_shift_employees);
                        //currentAssigning.Add(new AssigningEntity { department_id = dep.id, employee_id = e.id, shift_in_day_id = shift.id });
                        //Rating r = ConnectDB.entity.Rating.FirstOrDefault(x => x.Employee_ID == e.id && shift.id == x.Shift_In_Day);
                        //r.Shift_Approved = true;
                        //ConnectDB.entity.SaveChanges();
                    }
                }
            }
            return GetAssigning(business_id);
        }
    }
}
