using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AssigningDal
    {
        static ShiftEntities db = new ShiftEntities();
        //פונקציה לשליפת שיבוץ סופי
        public static List<Assigning> GetAssigning(int business_id)
        {
            try
            {
                List<Departments> departments = DepartmentsDal.GetAllDepartments(business_id);
                List<Employees> employees = EmployeeDal.GetAllEmployees(business_id);
                List<Shifts> shifts = ShiftsDal.GetAllShifts(business_id);
                List<Shifts_In_Days> shifts_In_Days = ShiftsDal.GetAllShiftsForDay(business_id);
                shifts_In_Days = shifts_In_Days.Where(x => shifts.Any(y => y.ID == x.Shift_ID)).ToList();
                List<Assigning> l = db.Assigning.ToList();
                l = l.Where(x => departments.Any(y => y.ID == x.Department_ID) && employees.Any(y => y.ID == x.Employee_ID) && shifts_In_Days.Any(y => y.ID == x.Shift_In_Day_ID)).ToList();
                return l;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }

        }
        public static void AddAssigning(Assigning assigning)
        {
            db.Assigning.Add(assigning);
            db.SaveChanges();
        }
    }
}
