﻿using System;
using System.Collections.Generic;
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
            BusinessEntity b = BusinessEntity.ConvertDBToEntity(ConnectDB.entity.Business.First(x => x.ID == id));
            return b;
        }
        //פונקציה לשליפת רשימת עסקים
        public static List<BusinessEntity> GetAllBusinesses()
        {
            List<BusinessEntity> l_business = BusinessEntity.ConvertListDBToListEntity(ConnectDB.entity.Business.ToList());
            return l_business;
        }

        //פונקציה למחיקת עסק
        public static List<BusinessEntity> DeleteBusiness(int id)
        {
            //מחיקה של כל הנתונים המקושרים לשדה זה קודם
            Business business_for_deleting = ConnectDB.entity.Business.First(x => x.ID == id);
            ConnectDB.entity.Business.Remove(business_for_deleting);
            return BusinessEntity.ConvertListDBToListEntity(ConnectDB.entity.Business.ToList());
        }
        //פונקציה לעדכון עסק
        public static List<BusinessEntity> UpdateBusiness(BusinessEntity b)
        {
            Business business_for_updating = ConnectDB.entity.Business.First(x => x.ID == b.ID);
            business_for_updating.Full_Name = b.Full_Name;
            business_for_updating.Logo = b.Logo;
            business_for_updating.Name = b.Name;
            business_for_updating.Number = b.Number;
            business_for_updating.Password = b.Password;
            business_for_updating.User_Name = b.User_Name;
            ConnectDB.entity.SaveChanges();
            return BusinessEntity.ConvertListDBToListEntity(ConnectDB.entity.Business.ToList());
        }


        //פונקציה להוספת עסק
        public static List<BusinessEntity> AddBusiness(BusinessEntity b)
        {
            ConnectDB.entity.Business.Add(BusinessEntity.ConvertEntityToDB(b));
            ConnectDB.entity.SaveChanges();
            return BusinessEntity.ConvertListDBToListEntity(ConnectDB.entity.Business.ToList());
        }

    }
}