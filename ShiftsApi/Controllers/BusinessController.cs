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
    [RoutePrefix("api/Business")]
    public class BusinessController : ApiController
    {
        //פונקציה להחזרת רשימת העסקים
        [Route("GetAllBusinesses")]
        [HttpGet]
        public List<BusinessEntity> GetAllBusinesses()
        {
            return BusinessBL.GetAllBusinesses();
        }

        //פונקציה להחזרת עסק ע"י קוד
        [Route("GetBusinessById/{id}")]
        [HttpGet]
        public BusinessEntity GetBusinessById(int id)
        {
            return BusinessBL.GetBusinessById(id);
        }
        //פונקציה למחיקת עסק
        [Route("DeleteBusiness/{id}")]
        [HttpDelete]
        public List<BusinessEntity> DeleteBusiness(int id)
        {
            return BusinessBL.DeleteBusiness(id);
        }
        //פונקציה לעדכון עסק
        [Route("UpdateBusiness")]
        [HttpPost]
        public List<BusinessEntity> UpdateBusiness([FromBody] BusinessEntity b)
        {
            return BusinessBL.UpdateBusiness(b);
        }
        //פונקציה להוספת עסק
        [Route("AddBusiness")]
        [HttpPut]
        public List<BusinessEntity> AddBusiness([FromBody] BusinessEntity b)
        {
            return BusinessBL.AddBusiness(b);
        }

        [Route("GetBusinessBydirectorDetails/{email}/{password}")]
        [HttpGet]
        public BusinessEntity GetBusinessBydirectorDetails(string email, string password)
        {
            return BusinessBL.GetBusinessBydirectorDetails(email, password);
        }
    }
}
