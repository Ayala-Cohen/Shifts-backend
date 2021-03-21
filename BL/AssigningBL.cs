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
            var e = EmployeesBL.GetOptimalEmployee(4);
            var list_departments = DepartmentsBL.GetAllDepartments(business_id);//רשימת המחלקות בעסק
            var list_employees_of_business = EmployeesBL.GetAllEmployees(business_id);//רשימת העובדים בעסק
            foreach (var dep in list_departments)//מעבר על רשימת המחלקות
            {
                foreach (var shift in DepartmentsBL.GetShiftForDepartment(dep.id))//מעבר על רשימת משמרות למחלקה
                {
                    var list_roles_for_shift = Shifts_EmployeesBL.GetRolesForSpecificShift(business_id, shift.id, dep.id);//שליפת התפקידים במשמרת זו

                }
            }
            return GetAssigning(business_id);
        }
    }
}
