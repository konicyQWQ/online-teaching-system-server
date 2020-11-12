using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OTS_API.Models;
using OTS_API.Services;

namespace OTS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeworkController : ControllerBase
    {
        private readonly TokenService tokenService;
        private readonly HomeworkService homeworkService;

        public HomeworkController(TokenService tokenService, HomeworkService homeworkService)
        {
            this.tokenService = tokenService;
            this.homeworkService = homeworkService;
        }

        [HttpGet]
        [Route("all")]
        public async Task<dynamic> OnGetCourseHomeworkAsync(int courseID, string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if(t == null)
                {
                    throw new Exception("Toke is Invalid!");
                }
                var role = t.Role;
                if(role != UserRole.Admin)
                {
                    
                }
                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
