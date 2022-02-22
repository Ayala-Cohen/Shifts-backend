using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BL;
using Entity;
using DAL;

namespace ShiftsApi.Controllers
{
    [RoutePrefix("api/Business")]
    public class BusinessController : ApiController
    {
        //פונקציה להחזרת רשימת העסקים
        [Route("GetAllBusinesses")]
        [HttpGet]
        public IHttpActionResult GetAllBusinesses()
        {
            return Ok(BusinessBL.GetAllBusinesses());
        }

        //פונקציה להחזרת עסק ע"י קוד
        [Route("GetBusinessById/{id}")]
        [HttpGet]
        public IHttpActionResult GetBusinessById(int id)
        {
            return Ok(BusinessBL.GetBusinessById(id));
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

        [Route("saveLogo/{business_id}")]
        [HttpPost]
        public string SaveLogo(int business_id)
        {
            var httpRequest = HttpContext.Current.Request;
            var postedFile = httpRequest.Files["Logo"];
            string filePath = @"D:\Ayala\FinalProject\Shifts\src\assets\images\";
            string fileName ="";
            if (postedFile != null)
            {
                string name = postedFile.FileName;
                name = name.Substring(0, name.IndexOf('.'));
                fileName = name + business_id.ToString() + Path.GetExtension(postedFile.FileName);
                filePath += fileName;
                if (!File.Exists(filePath))
                {
                    postedFile.SaveAs(filePath);
                }
            }
            return fileName;
        }

        [Route("GetBusinessBydirectorDetails/{email}/{password}")]
        [HttpGet]
        public BusinessEntity GetBusinessBydirectorDetails(string email, string password)
        {
            return BusinessBL.GetBusinessBydirectorDetails(email, password);
        }
        //[Route("GetLogo/{business_id}")]
        //[HttpPost]
        //public string GetLogo(int business_id)
        //{
        //    var httpRequest = HttpContext.Current.Request;
        //    var postedFile = httpRequest.Files["Logo"];
        //    string filePath = "";
        //    if (postedFile != null)
        //    {
        //        string name = postedFile.FileName;
        //        name = name.Substring(0, name.IndexOf('.'));
        //        var fileName = name + business_id.ToString() + Path.GetExtension(postedFile.FileName);
        //        filePath = HttpContext.Current.Server.MapPath(@"~/Files/images/" + fileName);
        //        if (!File.Exists(filePath))
        //            postedFile.SaveAs(filePath);
        //    }
        //    return filePath;
        //}
    }
}
