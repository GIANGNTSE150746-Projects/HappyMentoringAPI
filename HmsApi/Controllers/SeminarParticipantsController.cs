using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObjectLibrary;
using DataAccessLibrary.Business_Entity;
using DataAccessLibrary.Interfaces;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Formatter;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;

namespace HmsApi.Controllers
{
    [System.Web.Http.OData.ODataRouting]
    [Authorize]
    public class SeminarParticipantsController : ODataController
    {
        private readonly SeminarParticipantBusinessEntity _entity;

        public SeminarParticipantsController(IUnitOfWork work)
        {
            _entity = new SeminarParticipantBusinessEntity(work);
        }

        [HttpGet("odata/SeminarParticipants/GetSeminarParticipants")]
        [ProducesResponseType(typeof(IEnumerable<SeminarParticipant>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSeminarParticipants()
        {
            try
            {
                return StatusCode(200, await _entity.GetSeminarParticipantsAsync());
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/SeminarParticipants/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<SeminarParticipant>> GetSeminarParticipant(string id)
        //{
        //    try
        //    {
        //        return StatusCode(200, await _entity.GetSeminarParticipantAsync(id));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        [HttpPut("odata/SeminarParticipants/PutSeminarParticipant/{id}")]
        [ProducesResponseType(typeof(SeminarParticipant), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutSeminarParticipant(string SeminarId, ODataActionParameters parameters)
        {
            SeminarParticipant SeminarParticipant = ((JObject)parameters["SeminarParticipant"]).ToObject<SeminarParticipant>();

            if (SeminarId != SeminarParticipant.SeminarId)
            {
                return BadRequest();
            }

            try
            {
                return StatusCode(200, await _entity.UpdateSeminarParticipant(SeminarParticipant));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("odata/SeminarParticipants/PostSeminarParticipant")]
        [ProducesResponseType(typeof(SeminarParticipant), 201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostSeminarParticipant(ODataActionParameters parameters)
        {
            try
            {
                SeminarParticipant SeminarParticipant = ((JObject)parameters["SeminarParticipant"]).ToObject<SeminarParticipant>();
                return StatusCode(201, await _entity.AddSeminarParticipant(SeminarParticipant));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("odata/SeminarParticipants/DeleteSeminarParticipant/{seminarId}/{menteeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteSeminarParticipant(string seminarId, string menteeId)
        {
            try
            {
                await _entity.RemoveSeminarParticipant(seminarId, menteeId);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
