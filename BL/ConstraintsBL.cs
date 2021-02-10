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
            ConstraintsEntity c = ConstraintsEntity.ConvertDBToEntity(ConnectDB.entity.Constraints.First(x => x.Shift_Id == s_id && x.Employee_Id == e_id));
            return c;
        }
        //פונקציה לשליפת רשימת אילוצים
        public static List<ConstraintsEntity> GetAllConstraint()
        {
            List<ConstraintsEntity> l_constraints = ConstraintsEntity.ConvertListDBToListEntity(ConnectDB.entity.Constraints.ToList());
            return l_constraints;
        }

        //פונקציה למחיקת אילוץ
        public static List<ConstraintsEntity> DeleteConstraint(int s_id, string e_id)
        {
            //מחיקה של כל הנתונים המקושרים לשדה זה קודם
            Constraints c_for_deleting = ConnectDB.entity.Constraints.First(x => x.Shift_Id == s_id && x.Employee_Id == e_id);
            ConnectDB.entity.Constraints.Remove(c_for_deleting);
            return ConstraintsEntity.ConvertListDBToListEntity(ConnectDB.entity.Constraints.ToList());
        }
        //פונקציה לעדכון אילוץ
        public static List<ConstraintsEntity> UpdateConstraint(ConstraintsEntity c)
        {
            Constraints c_for_updating = ConnectDB.entity.Constraints.First(x => x.Shift_Id == c.shift_id && x.Employee_Id == c.employee_id);
            c_for_updating.Day = c.day;
            ConnectDB.entity.SaveChanges();
            return ConstraintsEntity.ConvertListDBToListEntity(ConnectDB.entity.Constraints.ToList());
        }

        //פונקציה להוספת אילוץ
        public static List<ConstraintsEntity> AddConstraint(ConstraintsEntity c)
        {
            ConnectDB.entity.Constraints.Add(ConstraintsEntity.ConvertEntityToDB(c));
            ConnectDB.entity.SaveChanges();
            return ConstraintsEntity.ConvertListDBToListEntity(ConnectDB.entity.Constraints.ToList());
        }




    }
}
