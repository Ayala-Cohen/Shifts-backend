﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Entity;
using BL;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace ShiftsApi.Controllers
{
    [RoutePrefix("api/Employees")]
    public class EmployeesController : ApiController
    {
        //פונקציה להחזרת רשימת עובדים
        [Route("GetAllEmployees/{business_id}")]
        [HttpGet]
        public List<EmployeesEntity> GetAllEmployees(int business_id)
        {
            return EmployeesBL.GetAllEmployees(business_id);
        }

        //פונקציה להחזרת עובד ע"י מספר זהות
        [Route("GetEmployeeById/{id}")]
        [HttpGet]
        public EmployeesEntity GetEmployeeById(string id)
        {
            return EmployeesBL.GetEmployeeById(id);
        }
        //פונקציה למחיקת עובד
        [Route("DeleteEmployee/{id}")]
        [HttpDelete]
        public List<EmployeesEntity> DeleteEmployee(string id)
        {
            return EmployeesBL.DeleteEmployee(id);
        }
        //פונקציה לעדכון עובד
        [Route("UpdateEmployee")]
        [HttpPost]
        public List<EmployeesEntity> UpdateEmployee([FromBody] EmployeesEntity e)
        {
            return EmployeesBL.UpdateEmployee(e);
        }
        //פונקציה להוספת עובד
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
        //פונקציה לטעינת רשימת עובדים מקובץ אקסל
        [Route("ImportFromExcel/{business_id}")]
        [HttpPost]
        public void ImportFromExcel(int business_id, [FromBody] Excel.Application application)
        {
            EmployeesBL.ImportFromExcel(business_id, application);
        }


        //פונקציה לשליפת עובד ע"פ כתובת הדוא"ל שלו 
        [Route("GetEmployeeByEmail")]
        [HttpPost]
        public EmployeesEntity GetEmployeeByEmail([FromBody] string email)
        {
            return EmployeesBL.GetEmployeeByEmail(email);
        }

    }
}
