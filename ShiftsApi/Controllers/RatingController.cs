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
    [RoutePrefix("api/Rating")]
    public class RatingController : ApiController
    {
        //פונקציה להחזרת רשימת דירוגים
        [Route("GetAllRatings")]
        [HttpGet]
        public List<RatingEntity> GetAllRatings()
        {
            return RatingBL.GetAllRating();
        }

        //פונקציה להחזרת דירוג ע"י קוד
        [Route("GetRatingById/{id}")]
        [HttpGet]
        public RatingEntity GetRatingById(string id)
        {
            return RatingBL.GetRatingById(id);
        }
        //פונקציה למחיקת דירוג
        [Route("DeleteRating/{id}")]
        [HttpDelete]
        public List<RatingEntity> DeleteRating(string id)
        {
            return RatingBL.DeleteRating(id);
        }
        //פונקציה לעדכון דירוג
        [Route("UpdateRating")]
        [HttpPost]
        public List<RatingEntity> UpdateRating([FromBody] RatingEntity r)
        {
            return RatingBL.UpdateRating(r);
        }
        //פונקציה להוספת דירוג
        [Route("AddRating")]
        [HttpPut]
        public List<RatingEntity> AddRating([FromBody] RatingEntity r)
        {
            return RatingBL.AddRating(r);
        }
    }
}
