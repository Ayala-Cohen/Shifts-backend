using System;
using System.Collections.Generic;
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
            ConstraintsEntity c = ConstraintsEntity.ConvertDBToEntity(ConnectDB.entity.Constraints.First(x => x.Shift_ID == s_id && x.Employee_Id == e_id));
            return c;
        }
        //פונקציה לשליפת רשימת אילוצים של עובד מסוים
        public static List<ConstraintsEntity> GetAllConstraint(string employee_id)
        {
            try
            {
                List<ConstraintsEntity> l_constraints = ConstraintsEntity.ConvertListDBToListEntity(ConnectDB.entity.Constraints.Where(x => x.Employee_Id == employee_id).ToList());
                return l_constraints;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        //פונקציה למחיקת אילוץ
        public static List<ConstraintsEntity> DeleteConstraint(int s_id, string day, string e_id)
        {
            try
            {
                Constraints c_for_deleting = ConnectDB.entity.Constraints.First(x => x.Shift_ID == s_id && x.Employee_Id == e_id && x.Day == day);
                ConnectDB.entity.Constraints.Remove(c_for_deleting);
                ConnectDB.entity.SaveChanges();
                return ConstraintsEntity.ConvertListDBToListEntity(ConnectDB.entity.Constraints.ToList());
            }
            catch (Exception e)
            {
                return null;
            }

        }
        //פונקציה לעדכון אילוץ
        public static List<ConstraintsEntity> UpdateConstraint(ConstraintsEntity c)
        {
            Constraints c_for_updating = ConnectDB.entity.Constraints.First(x => x.Shift_ID == c.shift_id && x.Employee_Id == c.employee_id);
            c_for_updating.Day = c.day;
            ConnectDB.entity.SaveChanges();
            return ConstraintsEntity.ConvertListDBToListEntity(ConnectDB.entity.Constraints.ToList());
        }

        //פונקציה להוספת אילוץ
        public static List<ConstraintsEntity> AddConstraint(ConstraintsEntity c)
        {
            try
            {
                ConnectDB.entity.Constraints.Add(ConstraintsEntity.ConvertEntityToDB(c));
                ConnectDB.entity.SaveChanges();
            }
            catch
            { }
            return ConstraintsEntity.ConvertListDBToListEntity(ConnectDB.entity.Constraints.ToList());
        }




    }
}
