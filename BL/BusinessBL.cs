using System;
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
            try
            {
                BusinessEntity b = BusinessEntity.ConvertDBToEntity(ConnectDB.entity.Business.First(x => x.ID == id));
                return b;
            }
            catch (Exception)
            {
            }
            return null;
        }
        //פונקציה לשליפת רשימת עסקים
        public static List<BusinessEntity> GetAllBusinesses()
        {
            try
            {
                List<BusinessEntity> l_business = BusinessEntity.ConvertListDBToListEntity(ConnectDB.entity.Business.ToList());
                return l_business;
            }
            catch (Exception)
            {
            }
            return null;
        }

        //פונקציה למחיקת עסק
        public static List<BusinessEntity> DeleteBusiness(int id)
        {
            try
            {
                //מחיקה של כל הנתונים המקושרים לשדה זה 
                foreach (var item in ConnectDB.entity.Shift_Employees.Where(x => x.Business_Id == id))
                {
                    Shifts_EmployeesBL.DeleteEmployeeShift(item.Shift_ID, item.Role_Id, item.Day, item.Departments_Id);
                }
                foreach (var item in ConnectDB.entity.Employees.Where(x => x.Business_Id == id))
                {
                    EmployeesBL.DeleteEmployee(item.ID);
                }
                foreach (var item in ConnectDB.entity.Departments.Where(x => x.Business_Id == id))
                {
                    DepartmentsBL.DeleteDepartment(item.ID);
                }
                foreach (var item in ConnectDB.entity.Shifts.Where(x => x.Business_Id == id))
                {
                    ShiftsBL.DeleteShift(item.ID);
                }
                foreach (var item in ConnectDB.entity.Employee_Roles.Where(x => x.Business_Id == id))
                {
                    Employees_RoleBL.DeleteEmployeeRole(item.ID);
                }
                Business business_for_deleting = ConnectDB.entity.Business.First(x => x.ID == id);
                ConnectDB.entity.Business.Remove(business_for_deleting);
                ConnectDB.entity.SaveChanges();
                return BusinessEntity.ConvertListDBToListEntity(ConnectDB.entity.Business.ToList());
            }
            catch (Exception)
            {
                return null;
            }

        }
        //פונקציה לעדכון עסק
        public static List<BusinessEntity> UpdateBusiness(BusinessEntity b)
        {
            try
            {
                Business business_for_updating = ConnectDB.entity.Business.First(x => x.ID == b.id);
                business_for_updating.Full_Name = b.full_name;
                business_for_updating.Logo = b.logo;
                business_for_updating.Name = b.name;
                business_for_updating.Number = b.number;
                business_for_updating.Password = b.password;
                business_for_updating.User_Name = b.user_name;
                ConnectDB.entity.SaveChanges();
                return BusinessEntity.ConvertListDBToListEntity(ConnectDB.entity.Business.ToList());
            }
            catch (Exception)
            {
                return null;
            }

        }


        //פונקציה להוספת עסק
        public static List<BusinessEntity> AddBusiness(BusinessEntity b)
        {
            try
            {
                ConnectDB.entity.Business.Add(BusinessEntity.ConvertEntityToDB(b));
                ConnectDB.entity.SaveChanges();
            }
            catch { }

            return BusinessEntity.ConvertListDBToListEntity(ConnectDB.entity.Business.ToList());
        }

        //פונקציה לשליפת פרטי עסק על ידי פרטי מנהל
        public static BusinessEntity GetBusinessBydirectorDetails(string email, string password)
        {
            try
            {
                Business b = ConnectDB.entity.Business.FirstOrDefault(x => x.User_Name == email && x.Password == password);
                if(b != null)
                    return BusinessEntity.ConvertDBToEntity(b);
                return null;
            }
            catch (Exception)
            {
                return null;

            }
        }
    }
}
