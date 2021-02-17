using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Entity;
using BL;

namespace ShiftsApi.Controllers
{
    [RoutePrefix("api/Employees")]
    public class EmployeesController : ApiController
    {
        //פונקציה להחזרת רשימת תפקידים
        [Route("GetAllEmployees/{business_id}")]
        [HttpGet]
        public List<EmployeesEntity> GetAllEmployees(int business_id)
        {
            return EmployeesBL.GetAllEmployees(business_id);
        }

        //פונקציה להחזרת תפקיד ע"י קוד
        [Route("GetEmployeeById/{id}")]
        [HttpGet]
        public EmployeesEntity GetEmployeeById(string id)
        {
            return EmployeesBL.GetEmployeeById(id);
        }
        //פונקציה למחיקת תפקיד
        [Route("DeleteEmployee/{id}")]
        [HttpDelete]
        public List<EmployeesEntity> DeleteEmployee(string id)
        {
            return EmployeesBL.DeleteEmployee(id);
        }
        //פונקציה לעדכון תפקיד
        [Route("UpdateEmployee")]
        [HttpPost]
        public List<EmployeesEntity> UpdateEmployee([FromBody] EmployeesEntity e)
        {
            return EmployeesBL.UpdateEmployee(e);
        }
        //פונקציה להוספת תפקיד
        [Route("AddEmployee")]
        [HttpPut]
        public List<EmployeesEntity> AddEmployee([FromBody] EmployeesEntity e)
        {
            return EmployeesBL.AddEmployee(e);
        }
        [Route("CheckEmployee/{email}/{password}")]
        [HttpGet]
        //פונקציה לבדיקת פרטי עובד ע"י שם משתמש וסיסמה
        public EmployeesEntity CheckEmployee(string email, string password)
        {
            return EmployeesBL.CheckEmployee(email, password);
        }
    }
}
