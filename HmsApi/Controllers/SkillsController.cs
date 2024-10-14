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
    public class SkillsController : ODataController
    {
        private readonly SkillBusinessEntity _entity;

        public SkillsController(IUnitOfWork work)
        {
            _entity = new SkillBusinessEntity(work);
        }

        [HttpGet("odata/Skills/GetSkills")]
        [ProducesResponseType(typeof(IEnumerable<Skill>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSkills()
        {
            try
            {
                return StatusCode(200, await _entity.GetSkillsAsync());
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/Skills/GetSkill/{id}")]
        [ProducesResponseType(typeof(Skill), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSkill(string id)
        {
            try
            {
                return StatusCode(200, await _entity.GetSkillAsync(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/Skills/GetSkillDTO")]
        [ProducesResponseType(typeof(Skill), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSkillDTO()
        {
            try
            {
                return StatusCode(200, await _entity.GetSkillDTO());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("odata/Skills/PutSkill/{id}")]
        [ProducesResponseType(typeof(Skill), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutSkill(string id, ODataActionParameters parameters)
        {
            Skill Skill = ((JObject)parameters["Skill"]).ToObject<Skill>();

            if (id != Skill.Id)
            {
                return BadRequest();
            }

            try
            {
                return StatusCode(200, await _entity.UpdateSkill(Skill));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("odata/Skills/PostSkill")]
        [ProducesResponseType(typeof(Skill), 201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostSkill(ODataActionParameters parameters)
        {
            try
            {
                Skill Skill = ((JObject)parameters["Skill"]).ToObject<Skill>();
                return StatusCode(201, await _entity.AddSkill(Skill));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("odata/Skills/DeleteSkill/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteSkill(string id)
        {
            try
            {
                await _entity.RemoveSkill(id);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
