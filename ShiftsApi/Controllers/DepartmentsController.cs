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
    [RoutePrefix("api/Departments")]
    public class DepartmentsController : ApiController
    {
        //פונקציה להחזרת רשימת המחלקות
        [Route("GetAllDepartments")]
        [HttpGet]
        public List<DepartmentsEntity> GetAllDepartments()
        {
            return DepartmentsBL.GetAllDepartments();
        }

        //פונקציה להחזרת מחלקה ע"י קוד
        [Route("GetDepartmentById/{id}")]
        [HttpGet]
        public DepartmentsEntity GetDepartmentById(int id)
        {
            return DepartmentsBL.GetDepartmentById(id);
        }
        //פונקציה למחיקת מחלקה
        [Route("DeleteDepartment/{id}")]
        [HttpDelete]
        public List<DepartmentsEntity> DeleteDepartment(int id)
        {
            return DepartmentsBL.DeleteDepartment(id);
        }
        //פונקציה לעדכון מחלקה
        [Route("UpdateDepartment")]
        [HttpPost]
        public List<DepartmentsEntity> UpdateDepartment([FromBody] DepartmentsEntity d)
        {
            return DepartmentsBL.UpdateDepartment(d);
        }
        //פונקציה להוספת מחלקה
        [Route("AddDepartment")]
        [HttpPut]
        public List<DepartmentsEntity> AddDepartment([FromBody] DepartmentsEntity d)
        {
            return DepartmentsBL.AddDepartment(d);
        }
    }
}
