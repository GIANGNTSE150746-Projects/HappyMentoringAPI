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
    
    public class MentorSkillsController : ODataController
    {
        private readonly MentorSkillBusinessEntity _entity;

        public MentorSkillsController(IUnitOfWork work)
        {
            _entity = new MentorSkillBusinessEntity(work);
        }

        [HttpGet("odata/MentorSkills/GetMentorSkills")]
        [ProducesResponseType(typeof(IEnumerable<MentorSkill>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMentorSkills()
        {
            try
            {
                return StatusCode(200, await _entity.GetMentorSkillsAsync());
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/MentorSkills/GetMentorSkill/{id}")]
        [ProducesResponseType(typeof(MentorSkill), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMentorSkill(string id)
        {
            try
            {
                return StatusCode(200, await _entity.GetMentorSkillAsync(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/MentorSkills/GetMentorSkillsByMentorId/{mentorId}")]
        [ProducesResponseType(typeof(MentorSkill), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMentorSkillsByMentorId(string mentorId)
        {
            try
            {
                return StatusCode(200, await _entity.GetMentorSkillsByMentorIdAsync(mentorId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("odata/MentorSkills/PutMentorSkill/{id}")]
        [ProducesResponseType(typeof(MentorSkill), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutMentorSkill(string id, ODataActionParameters parameters)
        {
            MentorSkill MentorSkill = ((JObject)parameters["MentorSkill"]).ToObject<MentorSkill>();
            if (id != MentorSkill.Id)
            {
                return BadRequest();
            }

            try
            {
                return StatusCode(200, await _entity.UpdateMentorSkills(MentorSkill));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("odata/MentorSkills/PostMentorSkill")]
        [ProducesResponseType(typeof(MentorSkill), 201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostMentorSkill(ODataActionParameters parameters)
        {
            try
            {
                MentorSkill MentorSkill = ((JObject)parameters["MentorSkill"]).ToObject<MentorSkill>();
                return StatusCode(201, await _entity.AddMentorSkills(MentorSkill));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("odata/MentorSkills/DeleteMentorSkill/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteMentorSkill(string id)
        {
            try
            {
                await _entity.RemoveMentorSkills(id);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("odata/MentorSkills/DeleteMentorSkillsBySkillId/{skillId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteMentorSkillsBySkillId(string skillId)
        {
            try
            {
                await _entity.RemoveMentorSkillsBySkillId(skillId);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
