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
    [RoutePrefix("api/ShiftsEmployees")]
    public class Shifts_EmployeesController : ApiController
    {
        //פונקציה להחזרת רשימת עובדים במשמרות
        [Route("GetAllShiftsEmployees/{business_id}")]
        [HttpGet]
        public List<Shift_EmployeesEntity> GetAllShiftsEmployees(int business_id)
        {
            return Shifts_EmployeesBL.GetAllEmployeesShifts(business_id);
        }

        //פונקציה להחזרת עובד במשמרת ע"י קוד
        [Route("GetShiftEmployeeById/{s_id}/{r_id}/{day}/{dep_id}")]
        [HttpGet]
        public Shift_EmployeesEntity GetShiftEmployeeById(int s_id, int r_id, string day, int dep_id)
        {
            return Shifts_EmployeesBL.GetEmployeesShiftById(s_id, r_id, day, dep_id);
        }
        //פונקציה למחיקת עובד במשמרת
        [Route("DeleteShiftEmployee/{s_id}/{r_id}/{day}/{dep_id}")]
        [HttpDelete]
        public List<Shift_EmployeesEntity> DeleteShiftEmployee(int s_id, int r_id, string day, int dep_id)
        {
            return Shifts_EmployeesBL.DeleteEmployeeShift(s_id, r_id, day, dep_id);
        }
        //פונקציה לעדכון עובד במשמרת
        [Route("UpdateShiftEmployee")]
        [HttpPost]
        public List<Shift_EmployeesEntity> UpdateShiftEmployee([FromBody] Shift_EmployeesEntity s)
        {
            return Shifts_EmployeesBL.UpdateEmployeeShift(s);
        }
        //פונקציה להוספת עובד במשמרת
        [Route("AddShiftEmployee")]
        [HttpPut]
        public List<Shift_EmployeesEntity> AddShiftEmployee([FromBody] Shift_EmployeesEntity s)
        {
            return Shifts_EmployeesBL.AddEmployeeShift(s);
        }
    }
}
