using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace Entity
{
    class BusinessEntity
    {

        public int ID { get; set; }
        public string Name { get; set; }
        public byte[] Logo { get; set; }
        public string User_Name { get; set; }
        public int Password { get; set; }
        public string Full_Name { get; set; }
        public int Number { get; set; }


        //המרת קטגוריה בודדת מסוג המסד לסוג המחלקה

        public static BusinessEntity ConvertDBToEntity(Business b)
        {
            return new BusinessEntity() { ID = b.ID, Name = b.Name, Logo = b.Logo, User_Name = b.User_Name, Password = b.Password, Full_Name = b.Full_Name, Number = b.Number };
        }
         

        //המרת קטגוריה בודדת מסוג המחלקה לסוג המסד
        public static Business ConvertEntityToDB(BusinessEntity b)
        {
            return new Business () { ID = b.ID, Name = b.Name, Logo = b.Logo, User_Name = b.User_Name, Password = b.Password, Full_Name = b.Full_Name, Number = b.Number };
        }

        //המרת רשימה מסוג המסד לרשימה מסוג המחלקה
        public static List<BusinessEntity> ConvertListDBToListEntity(List<Business> l)
        {
            List<BusinessEntity>b_business = new List<BusinessEntity>();
            foreach (var item in l)
            {
                b_business.Add(ConvertDBToEntity(item));
            }
            return b_business;
        }

        //המרת רשימה מסוג המסד לרשימה מסוג המחלקה
        public static List<Business> ConvertListEntityToListDB(List<BusinessEntity> l)
        {
            List<Business > b_business = new List<Business >();
            foreach (var item in l)
            {
                b_business.Add(ConvertEntityToDB(item));
            }
            return b_business;
        }
    }

}

