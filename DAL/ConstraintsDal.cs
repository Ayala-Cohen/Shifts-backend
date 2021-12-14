using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ConstraintsDal
    {
        static ShiftEntities db = new ShiftEntities();
        //פונקציה לשליפת אילוץ בודד על פי קוד
        public static Constraints GetConstraintById(int s_id, string e_id)
        {
            try
            {
                Constraints c = db.Constraints.First(x => x.Shift_ID == s_id && x.Employee_Id == e_id);
                return c;
            }
            catch (Exception)
            {
                return null;
            }

        }
        //פונקציה לשליפת רשימת אילוצים של עובד מסוים
        public static List<Constraints> GetAllConstraint(string employee_id)
        {
            try
            {
                List<Constraints> l_constraints = db.Constraints.Where(x => x.Employee_Id == employee_id).ToList();
                return l_constraints;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }

        }

        //פונקציה למחיקת אילוץ
        public static List<Constraints> DeleteConstraint(int s_id, string day, string e_id)
        {
            try
            {
                Constraints c_for_deleting = db.Constraints.First(x => x.Shift_ID == s_id && x.Employee_Id == e_id && x.Day == day);
                db.Constraints.Remove(c_for_deleting);
                db.SaveChanges();
                return GetAllConstraint(e_id);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }

        }
        //פונקציה לעדכון אילוץ
        public static List<Constraints> UpdateConstraint(Constraints c)
        {
            try
            {
                Constraints c_for_updating = db.Constraints.First(x => x.Shift_ID == c.Shift_ID && x.Employee_Id == c.Employee_Id);
                c_for_updating.Day = c.Day;
                db.SaveChanges();
                return GetAllConstraint(c.Employee_Id);
            }
            catch (Exception)
            {
                return null;
            }

        }

        //פונקציה להוספת אילוץ
        public static List<Constraints> AddConstraint(Constraints c)
        {
            try
            {
                db.Constraints.Add(c);
                db.SaveChanges();
            }
            catch
            { }
            return GetAllConstraint(c.Employee_Id);
        }
    }
}
