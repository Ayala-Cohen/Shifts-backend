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
    public class BusinessBL
    {
        //פונקציה לשליפת עסק בודד על פי קוד
        public static BusinessEntity GetBusinessById(int id)
        {
            return BusinessEntity.ConvertDBToEntity(BusinessDal.GetBusinessById(id));
        }
        //פונקציה לשליפת רשימת עסקים
        public static List<BusinessEntity> GetAllBusinesses()
        {
            return BusinessEntity.ConvertListDBToListEntity(BusinessDal.GetAllBusinesses());
        }

        //פונקציה למחיקת עסק
        public static List<BusinessEntity> DeleteBusiness(int id)
        {
            return BusinessEntity.ConvertListDBToListEntity(BusinessDal.DeleteBusiness(id));
        }
        //פונקציה לעדכון עסק
        public static List<BusinessEntity> UpdateBusiness(BusinessEntity b)
        {
            var b_db = BusinessEntity.ConvertEntityToDB(b);
            return BusinessEntity.ConvertListDBToListEntity(BusinessDal.UpdateBusiness(b_db));
        }


        //פונקציה להוספת עסק
        public static List<BusinessEntity> AddBusiness(BusinessEntity b)
        {
            var b_db = BusinessEntity.ConvertEntityToDB(b);
            return BusinessEntity.ConvertListDBToListEntity(BusinessDal.UpdateBusiness(b_db));
        }

        //פונקציה לשליפת פרטי עסק על ידי פרטי מנהל
        public static BusinessEntity GetBusinessBydirectorDetails(string email, string password)
        {
            return BusinessEntity.ConvertDBToEntity(BusinessDal.GetBusinessByDirectorDetails(email, password));
        }
    }
}
