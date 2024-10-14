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
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.Authorization;

namespace HmsApi.Controllers
{
    [System.Web.Http.OData.ODataRouting]
    [Authorize]
    public class TeachingThreadsController : ODataController
    {
        private readonly TeachingThreadBusinessEntity _entity;

        public TeachingThreadsController(IUnitOfWork work)
        {
            _entity = new TeachingThreadBusinessEntity(work);
        }

        [HttpGet("odata/TeachingThreads/GetTeachingThreadByRequestId/{requestId}")]
        [ProducesResponseType(typeof(IEnumerable<TeachingThread>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTeachingThreadByRequestId(string requestId)
        {
            try
            {
                return StatusCode(200, await _entity.GetTeachingThreadByRequestId(requestId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/TeachingThreads/GetTeachingThreads")]
        [ProducesResponseType(typeof(IEnumerable<TeachingThread>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTeachingThreads()
        {
            try
            {
                return StatusCode(200, await _entity.GetTeachingThreadsAsync());
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/TeachingThreads/GetTeachingThread/{id}")]
        [ProducesResponseType(typeof(TeachingThread), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTeachingThread(string id)
        {
            try
            {
                return StatusCode(200, await _entity.GetTeachingThreadAsync(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("odata/TeachingThreads/PutTeachingThread/{id}")]
        [ProducesResponseType(typeof(TeachingThread), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutTeachingThread(string id, ODataActionParameters parameters)
        {
            TeachingThread TeachingThread = ((JObject)parameters["TeachingThread"]).ToObject<TeachingThread>();

            if (id != TeachingThread.Id)
            {
                return BadRequest();
            }

            try
            {
                return StatusCode(200, await _entity.UpdateTeachingThread(TeachingThread));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("odata/TeachingThreads/PostTeachingThread")]
        [ProducesResponseType(typeof(TeachingThread), 201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostTeachingThread(ODataActionParameters parameters)
        {
            TeachingThread TeachingThread = ((JObject)parameters["TeachingThread"]).ToObject<TeachingThread>();
            try
            {
                return StatusCode(201, await _entity.AddTeachingThread(TeachingThread));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("odata/TeachingThreads/DeleteTeachingThread/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteTeachingThread(string id)
        {
            try
            {
                await _entity.RemoveTeachingThread(id);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
