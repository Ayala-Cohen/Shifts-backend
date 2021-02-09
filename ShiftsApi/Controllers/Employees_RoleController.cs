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
    [RoutePrefix("api/Roles")]
    public class Employees_RoleController : ApiController
    {
        //פונקציה להחזרת רשימת תפקידים
        [Route("GetAllRoles")]
        [HttpGet]
        public List<Employee_RolesEntity> GetAllRoles()
        {
            return Employees_RoleBL.GetAllEmployeesRoles();
        }

        //פונקציה להחזרת תפקיד ע"י קוד
        [Route("GetRoleById/{id}")]
        [HttpGet]
        public Employee_RolesEntity GetRoleById(int id)
        {
            return Employees_RoleBL.GetEmployeeRoleById(id);
        }
        //פונקציה למחיקת תפקיד
        [Route("DeleteRole/{id}")]
        [HttpDelete]
        public List<Employee_RolesEntity> DeleteRole(int id)
        {
            return Employees_RoleBL.DeleteEmployeeRole(id);
        }
        //פונקציה לעדכון תפקיד
        [Route("UpdateRole")]
        [HttpPost]
        public List<Employee_RolesEntity> UpdateRole([FromBody] Employee_RolesEntity r)
        {
            return Employees_RoleBL.UpdateEmployeeRole(r);
        }
        //פונקציה להוספת תפקיד
        [Route("AddRole")]
        [HttpPut]
        public List<Employee_RolesEntity> AddRole([FromBody] Employee_RolesEntity r)
        {
            return Employees_RoleBL.AddEmployeeRole(r);
        }
    }
}
