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
    [RoutePrefix("api/Shifts")]
    public class ShiftsController : ApiController
    {
        //פונקציה להחזרת רשימת משמרות
        [Route("GetAllShifts/{business_id}")]
        [HttpGet]
        public List<ShiftsEntity> GetAllShifts(int business_id)
        {
            return ShiftsBL.GetAllShifts(business_id);
        }

        //פונקציה להחזרת משמרת ע"י קוד
        [Route("GetShiftById/{id}")]
        [HttpGet]
        public ShiftsEntity GetShiftById(int id)
        {
            return ShiftsBL.GetShiftById(id);
        }

        [Route("GetShiftInDayId/{shift_id}/{day}")]
        [HttpGet]
        //פונקציה לשליפת משמרת ליום
        public int GetShiftInDayId(int shift_id, string day)
        {
            return ShiftsBL.GetShiftInDayId(shift_id, day);
        }
        [Route("GetAllShiftsForDay/{business_id}")]
        [HttpGet]
        //פונקציה לשליפת רשימת משמרות ליום
        public List<Shift_In_DayEntity> GetAllShiftsForDay(int business_id)
        {
            return ShiftsBL.GetAllShiftsForDay(business_id);
        }
        //פונקציה למחיקת משמרת
        [Route("DeleteShift/{id}")]
        [HttpDelete]
        public List<ShiftsEntity> DeleteShift(int id)
        {
            return ShiftsBL.DeleteShift(id);
        }
        //פונקציה לעדכון משמרת
        [Route("UpdateShift")]
        [HttpPost]
        public List<ShiftsEntity> UpdateShift([FromBody] ShiftsEntity s)
        {
            return ShiftsBL.UpdateShift(s);
        }
        //פונקציה להוספת משמרת
        [Route("AddShift")]
        [HttpPut]
        public List<ShiftsEntity> AddShift([FromBody] ShiftsEntity s)
        {
            return ShiftsBL.AddShift(s);
        }
    }
}
