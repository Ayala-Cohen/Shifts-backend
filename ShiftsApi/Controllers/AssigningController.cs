﻿using System;
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
        public List<AssigningEntity> GetAssigning(int business_id)
        {
            return AssigningBL.GetAssigning(business_id);
        }

        //פונקציה להחזרת העובדים שדירגו משמרת מסוימת בדירוג גבוה על מנת לערוך את השיבוץ
        [Route("GetEmployeesWithHighRating/{business_id}/{shift_id}")]
        [HttpGet]
        public Dictionary<int, List<EmployeesEntity>> GetEmployeesWithHighRating(int business_id, int shift_id)
        {
            return AssigningBL.GetEmployeesWithHighRating(business_id, shift_id);
        }

        //פונקציה להפעלת שיבוץ
        public List<AssigningEntity> ActivateAssigning(int business_id)
        {
            AssigningBL.AssigningActivity(business_id);
            return GetAssigning(business_id);
        }
    }
}
