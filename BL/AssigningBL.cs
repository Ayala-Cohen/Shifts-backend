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
        //פונקציה לשליפת שיבוץ סופי
        public static List<AssigningEntity> GetAssigning(int business_id)
        {
            List<Assigning> l = ConnectDB.entity.Assigning.ToList();
            if (l != null)
                return (AssigningEntity.ConvertListDBToListEntity(l));
            return null;
        }
    }
}
