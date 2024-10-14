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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HmsLibrary;
using DTOsLibrary;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Formatter;
using Newtonsoft.Json.Linq;

namespace HmsApi.Controllers
{
    [System.Web.Http.OData.ODataRouting]
    [Authorize]
    public class UsersController : ODataController
    {
        private readonly UserBusinessEntity _entity;

        public UsersController(IUnitOfWork work)
        {
            _entity = new UserBusinessEntity(work);
        }

        [HttpGet("odata/Users/GetUsers")]
        [ProducesResponseType(typeof(IEnumerable<User>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                return StatusCode(200, await _entity.GetUsersAsync());
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/Users/GetMentors")]
        public async Task<ActionResult<IEnumerable<MentorDTO>>> GetMentors()
        {
            try
            {
                return StatusCode(200, await _entity.GetMentorsAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/Users/GetMentees")]
        public async Task<ActionResult<IEnumerable<MentorDTO>>> GetMentees()
        {
            try
            {
                return StatusCode(200, await _entity.GetMenteesAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("odata/Users/GetUser/{id}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            try
            {
                return StatusCode(200, await _entity.GetUserAsync(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/Users/email/test@gmail.com
        [HttpGet("odata/Users/GetUserByEmail/{email}")]
        [ProducesResponseType(typeof(UserDTO), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                return StatusCode(200, await _entity.GetUserByEmailAsync(email));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //[HttpPut("Mentees/{menteeId}")]
        //[ProducesResponseType(typeof(User), 200)]
        //[ProducesResponseType(500)]
        //[ProducesResponseType(400)]
        //public async Task<IActionResult> MenteeUploadCv(string menteeId, [FromBody] string cvUrl)
        //{
        //    try
        //    {
        //        return StatusCode(200, await _entity.MenteeUploadCv(menteeId, cvUrl));
        //    } catch (ApplicationException ae)
        //    {
        //        return StatusCode(400, ae.Message);
        //    } catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        // POST api/Users/login
        [HttpPost("odata/Users/Login")]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        [AllowAnonymous]
        public async Task<IActionResult> Login(ODataActionParameters parameters)
        {
            try
            {
                LoginUserDTO loginUser = ((JObject)parameters["LoginUserDTO"]).ToObject<LoginUserDTO>();

                User user = await _entity.Login(loginUser.Email, loginUser.Password);

                if (user != null)
                {
                    if (user.Role == UserRole.MENTOR && user.MentorDetail.WorkStatus == MentorWorkingStatus.PENDING)
                    {
                        throw new Exception("You have registered as a mentor and you are being reviewed by our administrators! Please try again later...");
                    }
                    user.Password = "";

                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, HmsUtils.CreateGuid())
                };

                    var authSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(HmsConfiguration.Secret));

                    // Token
                    var token = new JwtSecurityToken(
                        issuer: HmsConfiguration.JwtIssuer,
                        audience: HmsConfiguration.JwtAudience,
                        expires: DateTime.Now.AddHours(2),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(
                            authSigningKey, SecurityAlgorithms.HmacSha256)
                        );

                    return StatusCode(200, new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token)
                    });
                } else
                {
                    throw new Exception("Login failed! Please check your login information and try again...");
                }

            } catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPut("odata/Users/PutUser/{id}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutUser(string id, ODataActionParameters parameters)
        {
            User User = ((JObject)parameters["User"]).ToObject<User>();

            if (id != User.Id)
            {
                return BadRequest();
            }

            try
            {
                return StatusCode(200, await _entity.UpdateUser(User));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("odata/Users/PutMentor/{id}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutMentor(string id, ODataActionParameters parameters)
        {
            MentorDTO MentorDTO = ((JObject)parameters["MentorDTO"]).ToObject<MentorDTO>();

            if (id != MentorDTO.Id)
            {
                return BadRequest();
            }

            try
            {
                return StatusCode(200, await _entity.UpdateMentor(MentorDTO));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("odata/Users/PostUser")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(User), 201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostUser(ODataActionParameters parameters)
        {
            try
            {
                User User = ((JObject)parameters["User"]).ToObject<User>();
                return StatusCode(201, await _entity.AddUser(User));
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

        [HttpDelete("odata/Users/DeleteUser/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                await _entity.RemoveUser(id);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
