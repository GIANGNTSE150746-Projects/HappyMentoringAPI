using DataAccessLibrary.Business_Entity;
using DataAccessLibrary.Interfaces;
using DTOsLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HmsApi.Controllers
{
    [System.Web.Http.OData.ODataRouting]
    [Authorize]
    
    public class ReportController : Controller
    {
        private readonly RequestBusinessEntity _entity;

        public ReportController(IUnitOfWork work)
        {
            _entity = new RequestBusinessEntity(work);
        }

        [HttpGet("odata/Reports/GetAdminReport/{filter}")]
        [ProducesResponseType(typeof(IEnumerable<AdminReportDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAdminReport(string filter)
        {
            try
            {
                return StatusCode(200, await _entity.GetAdminReportAsync(filter));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
