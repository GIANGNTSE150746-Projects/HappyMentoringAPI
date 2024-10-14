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
    public class CvsController : ODataController
    {
        private readonly CvBusinessEntity _entity;

        public CvsController(IUnitOfWork work)
        {
            _entity = new CvBusinessEntity(work);
        }

        [HttpGet("odata/Cvs/GetCvs")]
        [ProducesResponseType(typeof(IEnumerable<Cv>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCvs()
        {
            try
            {
                return StatusCode(200, await _entity.GetCvsAsync());
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/Cvs/GetCv/{id}")]
        [ProducesResponseType(typeof(Cv), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCv(string id)
        {
            try
            {
                return StatusCode(200, await _entity.GetCvAsync(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("odata/Cvs/PutCv/{id}")]
        [ProducesResponseType(typeof(Cv), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutCv(string id, ODataActionParameters parameters)
        {
            Cv cv = ((JObject)parameters["Cv"]).ToObject<Cv>();

            if (id != cv.Id)
            {
                return BadRequest();
            }

            try
            {
                return StatusCode(200, await _entity.UpdateCv(cv));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("odata/Cvs/PostCv")]
        [ProducesResponseType(typeof(Cv), 201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostCv(ODataActionParameters parameters)
        {
            try
            {
                Cv cv = ((JObject)parameters["Cv"]).ToObject<Cv>();
                return StatusCode(201, await _entity.AddCv(cv));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("odata/Cvs/DeleteCv/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCv(string id)
        {
            try
            {
                await _entity.RemoveCv(id);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
