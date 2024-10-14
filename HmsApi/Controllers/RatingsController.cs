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
    public class RatingsController : ODataController
    {
        private readonly RatingBusinessEntity _entity;

        public RatingsController(IUnitOfWork work)
        {
            _entity = new RatingBusinessEntity(work);
        }

        [HttpGet("odata/Ratings/GetRatings")]
        [ProducesResponseType(typeof(IEnumerable<Rating>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRatings()
        {
            try
            {
                return StatusCode(200, await _entity.GetRatingsAsync());
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/Ratings/GetRating/{id}")]
        [ProducesResponseType(typeof(Rating), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRating(string id)
        {
            try
            {
                return StatusCode(200, await _entity.GetRatingAsync(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("odata/Ratings/PutRating/{id}")]
        [ProducesResponseType(typeof(Rating), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutRating(string id, ODataActionParameters parameters)
        {
            Rating Rating = ((JObject)parameters["Rating"]).ToObject<Rating>();

            if (id != Rating.Id)
            {
                return BadRequest();
            }

            try
            {
                return StatusCode(200, await _entity.UpdateRating(Rating));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("odata/Ratings/PostRating")]
        [ProducesResponseType(typeof(Rating), 201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostRating(ODataActionParameters parameters)
        {
            try
            {
                Rating Rating = ((JObject)parameters["Rating"]).ToObject<Rating>();
                return StatusCode(201, await _entity.AddRating(Rating));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("odata/Ratings/DeleteRating/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRating(string id)
        {
            try
            {
                await _entity.RemoveRating(id);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
