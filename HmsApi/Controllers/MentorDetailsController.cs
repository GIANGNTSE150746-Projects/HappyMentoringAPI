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
using Microsoft.AspNetCore.Authorization;

namespace HmsApi.Controllers
{
    [System.Web.Http.OData.ODataRouting]
    [Authorize]
    public class MentorDetailsController : ODataController
    {
        private readonly MentorDetailsBusinessEntity _entity;

        public MentorDetailsController(IUnitOfWork work)
        {
            _entity = new MentorDetailsBusinessEntity(work);
        }

        [HttpGet("odata/MentorDetails/GetMentorDetails")]
        [ProducesResponseType(typeof(IEnumerable<MentorDetail>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMentorDetails()
        {
            try
            {
                return StatusCode(200, await _entity.GetMentorDetailsAsync());
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/MentorDetails/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<MentorDetail>> GetMentorDetail(string id)
        //{
        //    try
        //    {
        //        return StatusCode(200, await _entity.GetMentorDetailAsync(id));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        // PUT: api/MentorDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutMentorDetail(string id, MentorDetail MentorDetail)
        //{
        //    if (id != MentorDetail.Id)
        //    {
        //        return BadRequest();
        //    }

        //    try
        //    {
        //        return StatusCode(200, await _entity.UpdateMentorDetail(MentorDetail));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        // POST: api/MentorDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<MentorDetail>> PostMentorDetail(MentorDetail MentorDetail)
        //{
        //    try
        //    {
        //        return StatusCode(201, await _entity.AddMentorDetail(MentorDetail));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        // DELETE: api/MentorDetails/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteMentorDetail(string id)
        //{
        //    try
        //    {
        //        await _entity.RemoveMentorDetail(id);
        //        return StatusCode(200);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}
    }
}
