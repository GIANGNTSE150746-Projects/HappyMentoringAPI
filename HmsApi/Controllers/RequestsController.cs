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
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Formatter;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;

namespace HmsApi.Controllers
{
    [System.Web.Http.OData.ODataRouting]
    [Authorize]
    public class RequestsController : ODataController
    {
        private readonly RequestBusinessEntity _entity;

        public RequestsController(IUnitOfWork work)
        {
            _entity = new RequestBusinessEntity(work);
        }
        [HttpGet("odata/Requests/GetRequestForMentor")]
        [ProducesResponseType(typeof(IEnumerable<Request>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRequestForMentor(string key)
        {
            try
            {
                return StatusCode(200, await _entity.GetRequestsForMentorAsync(key));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/Requests/GetFloatingRequest")]
        [ProducesResponseType(typeof(IEnumerable<Request>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFloatingRequest(string key)
        {
            try
            {
                return StatusCode(200, await _entity.GetFloatingRequest(key));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //[HttpGet]
        [EnableQuery]
        [HttpGet("odata/Requests/GetRequests")]
        [ProducesResponseType(typeof(IEnumerable<Request>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRequests()
        {
            try
            {
                return StatusCode(200, await _entity.GetRequestsAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/Requests/GetConcludedRequests")]
        [ProducesResponseType(typeof(IEnumerable<Request>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetConcludedRequests()
        {
            try
            {
                return StatusCode(200, await _entity.GetConcludedRequestsAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/Requests/GetReportedRequests")]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetReportedRequests()
        {
            try
            {
                return StatusCode(200, await _entity.GetReportedRequestsAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/Requests/Mentee/{menteeId}")]
        [ProducesResponseType(typeof(IEnumerable<Request>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMenteeRequest(string menteeId)
        {
            try
            {
                return StatusCode(200, await _entity.GetMenteeRequests(menteeId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/Requests/5
        [EnableQuery]
        [HttpGet("odata/Requests/GetRequest/{key}")]
        [ProducesResponseType(typeof(Request), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRequest( string key)
        {
            try
            {
                return StatusCode(200, await _entity.GetRequestAsync(key));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [EnableQuery]
        [HttpPut("odata/Requests/PutRequest/{key}")]
        [ProducesResponseType(typeof(Request), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutRequest(string key, ODataActionParameters parameters)
        {

            Request Request = ((JObject)parameters["Request"]).ToObject<Request>();
            if (key != Request.Id)
            {
                return BadRequest();
            }

            try
            {
                return StatusCode(200, await _entity.UpdateRequest(Request));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("odata/Requests/ReleaseRequest/{MentorId}")]
        [ProducesResponseType(typeof(Request), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        //When admin deactivate a mentor account --> All mentor's pending requests will be set as MentorId to null
        public async Task<IActionResult> ReleaseRequest(string MentorId)
        {
            try
            {
                await _entity.ReleaseRequest(MentorId);
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        [EnableQuery]
        [HttpPost("odata/Requests/PostRequest")]
        [ProducesResponseType(typeof(Request), 201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostRequest(ODataActionParameters parameters)
        {
            try
            {
                Request Request = ((JObject)parameters["Request"]).ToObject<Request>();
                return StatusCode(201, await _entity.AddRequest(Request));
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

        [HttpPut("odata/Requests/UnflagReportedRequest")]
        [ProducesResponseType(typeof(Request), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UnflagReportedRequest(ODataActionParameters parameters)
        {
            try
            {
                ReportedRequestDTO reportedRequest = ((JObject)parameters["Request"]).ToObject<ReportedRequestDTO>();
                return StatusCode(200, await _entity.UnflagReportedRequest(reportedRequest));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [EnableQuery]
        [HttpDelete("odata/Requests/DeleteRequest/{key}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRequest(string key)
        {
            try
            {
                await _entity.RemoveRequest(key);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("odata/Requests/DeleteRequestByMenteeId/{MenteeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        //Admin delete mentee --> pending request of this mentee will be removed.
        public async Task<IActionResult> DeleteRequestByMenteeId(string MenteeId)
        {
            try
            {
                await _entity.DeleteRequestByMenteeId(MenteeId);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
