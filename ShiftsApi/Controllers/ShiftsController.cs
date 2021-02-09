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
        [Route("GetAllShifts")]
        [HttpGet]
        public List<ShiftsEntity> GetAllShifts()
        {
            return ShiftsBL.GetAllShifts();
        }

        //פונקציה להחזרת משמרת ע"י קוד
        [Route("GetShiftById/{id}")]
        [HttpGet]
        public ShiftsEntity GetShiftById(int id)
        {
            return ShiftsBL.GetShiftById(id);
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
