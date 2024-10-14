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
using DTOsLibrary;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Formatter;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;

namespace HmsApi.Controllers
{
    [System.Web.Http.OData.ODataRouting]
    [Authorize]
    
    public class SeminarsController : ODataController
    {
        private readonly SeminarBusinessEntity _entity;

        public SeminarsController(IUnitOfWork work)
        {
            _entity = new SeminarBusinessEntity(work);
        }

        [HttpGet("odata/Seminars/GetSeminars")]
        [ProducesResponseType(typeof(IEnumerable<Seminar>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSeminars()
        {
            try
            {
                return StatusCode(200, await _entity.GetSeminarsAsync());
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/Seminars/GetSeminar/{id}")]
        [ProducesResponseType(typeof(Seminar), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSeminar(string id)
        {
            try
            {
                return StatusCode(200, await _entity.GetSeminarAsync(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("odata/Seminars/PutSeminar/{id}")]
        [ProducesResponseType(typeof(Seminar), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutSeminar(string id, ODataActionParameters parameters)
        {
            Seminar Seminar = ((JObject)parameters["Seminar"]).ToObject<Seminar>();
            if (id != Seminar.Id)
            {
                return BadRequest();
            }

            try
            {
                return StatusCode(200, await _entity.UpdateSeminar(Seminar));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("odata/Seminars/PostSeminar")]
        [ProducesResponseType(typeof(Seminar), 201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostSeminar(ODataActionParameters parameters)
        {
            try
            {
                Seminar Seminar = ((JObject)parameters["Seminar"]).ToObject<Seminar>();
                return StatusCode(201, await _entity.AddSeminar(Seminar));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("odata/Seminars/DeleteSeminar/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteSeminar(string id)
        {
            try
            {
                await _entity.RemoveSeminar(id);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("odata/Seminars/ReleaseSeminar/{MentorId}")]
        [ProducesResponseType(typeof(Request), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        //When admin deactivate a mentor account --> All mentor's future seminars will be set as deleted
        public async Task<IActionResult> ReleaseSeminar(string MentorId)
        {
            try
            {
                await _entity.ReleaseSeminar(MentorId);
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/Seminars/Mentee/{menteeId}")]
        [ProducesResponseType(typeof(MenteeSeminarDTO), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetMenteeSeminars(string menteeId)
        {
            try
            {
                return StatusCode(200, await _entity.GetMenteeSeminarsAsync(menteeId));
            } catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/Seminars/Mentor/{mentorId}")]
        [ProducesResponseType(typeof(MentorOtherSeminarDTO), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetMentorSeminars(string mentorId)
        {
            try
            {
                return StatusCode(200, await _entity.GetMentorSeminarsAsync(mentorId));
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/Seminars/Mentor/Other/{mentorId}")]
        [ProducesResponseType(typeof(MentorOtherSeminarDTO), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetMentorOtherSeminars(string mentorId)
        {
            try
            {
                return StatusCode(200, await _entity.GetMentorOtherSeminarsAsync(mentorId));
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
