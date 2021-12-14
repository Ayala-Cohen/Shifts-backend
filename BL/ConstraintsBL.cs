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
    public class ConstraintsBL
    {
        //פונקציה לשליפת אילוץ בודד על פי קוד
        public static ConstraintsEntity GetConstraintById(int s_id, string e_id)
        {
            return ConstraintsEntity.ConvertDBToEntity(ConstraintsDal.GetConstraintById(s_id, e_id));
        }
        //פונקציה לשליפת רשימת אילוצים של עובד מסוים
        public static List<ConstraintsEntity> GetAllConstraint(string employee_id)
        {
            return ConstraintsEntity.ConvertListDBToListEntity(ConstraintsDal.GetAllConstraint(employee_id));
        }


        //פונקציה למחיקת אילוץ
        public static List<ConstraintsEntity> DeleteConstraint(int s_id, string day, string e_id)
        {
            return ConstraintsEntity.ConvertListDBToListEntity(ConstraintsDal.DeleteConstraint(s_id, day, e_id));
        }
        //פונקציה לעדכון אילוץ
        public static List<ConstraintsEntity> UpdateConstraint(ConstraintsEntity c)
        {
            var c_db = ConstraintsEntity.ConvertEntityToDB(c);
            return ConstraintsEntity.ConvertListDBToListEntity(ConstraintsDal.UpdateConstraint(c_db));
        }

        //פונקציה להוספת אילוץ
        public static List<ConstraintsEntity> AddConstraint(ConstraintsEntity c)
        {
            var c_db = ConstraintsEntity.ConvertEntityToDB(c);
            return ConstraintsEntity.ConvertListDBToListEntity(ConstraintsDal.AddConstraint(c_db));
        }

        //חישוב המספר להגבלת אילוצים קבועים על מנת שלא יווצר מצב שכל העובדים חסמו את אותה המשמרת באילוץ קבוע
        //החישוב יתבצע בצורה כזו
        //נבדוק קודם כל כמה עובדים יש מהתפקיד של העובד המחובר , אחר כך נבדוק כמה מתוך מספר זה עובדים ביותר ממחלקה אחת
        //בסופו של דבר נבדוק איזה מספר מקסימלי של עובדים במשמרת אחת מתפקיד זה
        //המספר להגבלה יהיה מספר העובדים מהתפקיד פחות כל שאר המשתנים שחישבנו
        public static int GetLimitForConstraint(int business_id, int role_id)
        {
            try
            {
                var list_employees = EmployeesBL.GetAllEmployees(business_id).Where(x => x.role_id == role_id);
                var num_employees_with_same_role = list_employees.Count();
                var employees_with_same_role_work_more_than_ward = 0;
                var employees_departments = EmployeesBL.GetAllEmployeesDepartments(business_id);
                foreach (var employee in list_employees)
                {
                    if (employees_departments[employee.id].Count() > 1)
                        employees_with_same_role_work_more_than_ward++;
                }
                var employees_in_shift_current_role = Shifts_EmployeesBL.GetAllEmployeesShifts(business_id).Where(x => x.role_id == role_id);
                var number_of_high_from_current_role = employees_in_shift_current_role.OrderBy(x => x.number_of_shift_employees).Last().number_of_shift_employees;
                var limit = num_employees_with_same_role - employees_with_same_role_work_more_than_ward - number_of_high_from_current_role;
                if (limit < 0)
                    limit = 0;
                return limit;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return -1;
            }
        }

        public static Dictionary<int, Dictionary<int, int>> GetNumberOfConstraintsPerShift(int business_id)
        {
            Dictionary<int, int> dic_shifts;
            Dictionary<int, Dictionary<int, int>> dic_roles = new Dictionary<int, Dictionary<int, int>>();
            try
            {
                var roles = Employees_RoleBL.GetAllEmployeesRoles(business_id);
                var constraints_db = ConnectDB.entity.Constraints.Where(x => ConnectDB.entity.Shifts.Any(y => y.Business_Id == business_id));
                foreach (var role in roles)
                {
                    dic_shifts = new Dictionary<int, int>();
                    var filtered = constraints_db.Where(x => ConnectDB.entity.Employees.FirstOrDefault(y => y.ID == x.Employee_Id).Role_Id == role.id);//רשימת האילוצים של העובדים בתפקיד הנוכחי
                    var constraints_db_grouped = filtered.GroupBy(x => new { x.Shift_ID, x.Day });
                    foreach (var item in constraints_db_grouped)
                    {
                        var shift_in_day_id = ShiftsBL.GetShiftInDayId(item.Key.Shift_ID, item.Key.Day);
                        dic_shifts.Add(shift_in_day_id, item.Count());
                    }
                    dic_roles.Add(role.id, dic_shifts);
                }
                return dic_roles;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

    }
}
