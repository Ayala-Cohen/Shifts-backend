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
        public List<AssigningEntity> GetAssigning(int business_id)
        {
            return AssigningBL.GetAssigning(business_id);
        }
    }
}
