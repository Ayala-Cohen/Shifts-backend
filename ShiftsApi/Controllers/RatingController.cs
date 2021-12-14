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
        //פונקציה להחזרת רשימת דירוגים של עובד מסוים
        [Route("GetAllRatings/{employee_id}")]
        [HttpGet]
        public List<RatingEntity> GetAllRatingsOfEmployee(string employee_id)
        {
            return RatingBL.GetAllRatingOfEmployee(employee_id);
        }

        //פונקציה להחזרת דירוג ע"י קוד
        [Route("GetRatingById/{e_id}/{s_in_day}")]
        [HttpGet]
        public RatingEntity GetRatingById(string e_id, int s_in_day)
        {
            return RatingBL.GetRatingById(e_id, s_in_day);
        }
        //פונקציה למחיקת דירוג
        [Route("DeleteRating/{e_id}/{s_in_day}")]
        [HttpDelete]
        public List<RatingEntity> DeleteRating(string e_id, int s_in_day)
        {
            return RatingBL.DeleteRating(e_id, s_in_day);
        }
        //פונקציה לעדכון דירוג
        [Route("UpdateRating")]
        [HttpPost]
        public List<RatingEntity> UpdateRating([FromBody] RatingEntity r)
        {
            return RatingBL.UpdateRating(r);
        }
        //פונקציה להוספת דירוג
        [Route("AddRating/{day}")]
        [HttpPut]
        public List<RatingEntity> AddRating([FromBody] RatingEntity r, string day)
        {
            return RatingBL.AddRating(r, day);
        }
    }
}
