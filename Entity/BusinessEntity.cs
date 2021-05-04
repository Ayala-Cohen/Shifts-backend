using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace Entity
{
    public class BusinessEntity
    {

        public int id { get; set; }
        public string name { get; set; }
        public byte[] logo { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public string full_name { get; set; }
        public int number { get; set; }

        public Nullable<DateTime> LastAssigningDate { get; set; }


        //המרת עסק בודד מסוג המסד לסוג המחלקה

        public static BusinessEntity ConvertDBToEntity(Business b)
        {
            return new BusinessEntity() { id = b.ID, name = b.Name, logo = b.Logo, user_name = b.User_Name, password = b.Password, full_name = b.Full_Name, number = b.Number ,LastAssigningDate=b.LastAssigningDate};
        }


        //המרת עסק בודד מסוג המחלקה לסוג המסד
        public static Business ConvertEntityToDB(BusinessEntity b)
        {
            return new Business () { ID = b.id, Name = b.name, Logo = b.logo, User_Name = b.user_name, Password = b.password, Full_Name = b.full_name, Number = b.number };
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

        //המרת רשימה מסוג המחלקה לרשימה מסוג המסד
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

