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
    [RoutePrefix("api/Constraints")]
    public class ConstraintsController : ApiController
    {
        //פונקציה להחזרת רשימת האילוצים
        [Route("GetAllConstraints")]
        [HttpGet]
        public List<ConstraintsEntity> GetAllConstraints()
        {
            return ConstraintsBL.GetAllConstraint();
        }

        //פונקציה להחזרת אילוץ ע"י קוד
        [Route("GetConstraintById/{s_id}/{e_id}")]
        [HttpGet]
        public ConstraintsEntity GetConstaintById(int s_id, string e_id)
        {
            return ConstraintsBL.GetConstraintById(s_id, e_id);
        }
        //פונקציה למחיקת אילוץ
        [Route("DeleteConstraint/{s_id}/{e_id}")]
        //how to pass 2 arguments? 
        [HttpDelete]
        public List<ConstraintsEntity> DeleteConstraint(int s_id, string e_id)
        {
            return ConstraintsBL.DeleteConstraint(s_id, e_id);
        }
        //פונקציה לעדכון אילוץ
        [Route("UpdateConstraint")]
        [HttpPost]
        public List<ConstraintsEntity> UpdateConstraint([FromBody] ConstraintsEntity c)
        {
            return ConstraintsBL.UpdateConstraint(c);
        }
        //פונקציה להוספת אילוץ
        [Route("AddConstraint")]
        [HttpPut]
        public List<ConstraintsEntity> AddConstraint([FromBody] ConstraintsEntity c)
        {
            return ConstraintsBL.AddConstraint(c);
        }
    }
}
