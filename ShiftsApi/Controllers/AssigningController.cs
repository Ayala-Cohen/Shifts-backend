using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BL;
using Entity;

namespace ShiftsApi.Controllers
{
    [RoutePrefix("api/Assigning")]
    public class AssigningController : ApiController
    {
        //פונקציה לשליפת שיבוץ סופי
        [Route("GetAssigning/{business_id}")]
        public IHttpActionResult GetAssigning(int business_id)
        {
            return Ok(AssigningBL.GetAssigning(business_id));
        }

        //פונקציה להחזרת העובדים שדירגו משמרת מסוימת בדירוג גבוה על מנת לערוך את השיבוץ
        [Route("GetEmployeesForReplacing/{business_id}/{shift_id}")]
        [HttpGet]
        public Dictionary<int, List<EmployeesEntity>> GetEmployeesForReplacing(int business_id, int shift_id)
        {
            return AssigningBL.GetEmployeesForReplacing(business_id, shift_id);
        }

        [Route("ActivateAssigning/{business_id}")]
        [HttpGet]
        //פונקציה להפעלת שיבוץ
        public List<AssigningEntity> ActivateAssigning(int business_id)
        {
            AssigningBL.AssigningActivity(business_id);
            return (List<AssigningEntity>)GetAssigning(business_id);
        }

        //פונקציה לעריכת שיבוץ
        [Route("EditAssigning/{employee_id_replacing}")]
        [HttpPost]
        public List<AssigningEntity> EditAssigning(string employee_id_replacing, [FromBody] AssigningEntity assinging_for_editing)
        {
            return AssigningBL.EditAssiging(assinging_for_editing, employee_id_replacing);
        }

        [Route("GetAssingingByRoles/{shift_in_day_id}/{department_id}")]
        [HttpGet]
        public Dictionary<int, List<EmployeesEntity>> GetAssingingByRoles(int shift_in_day_id, int department_id)
        {
            return AssigningBL.GetAssingingByRoles(shift_in_day_id, department_id);
        }

    }
}
