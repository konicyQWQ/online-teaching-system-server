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
    public class SystemController : ControllerBase
    {
        private readonly EventService eventService;
        private readonly TokenService tokenService;

        public SystemController(EventService eventService, TokenService tokenService)
        {
            this.eventService = eventService;
            this.tokenService = tokenService;
        }

        [HttpPost]
        public async Task<dynamic> OnSystemAnounceAsync([FromForm] string title, [FromForm] string content, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("Token is Invalid!");
                }
                if (t.Role != UserRole.Admin)
                {
                    throw new Exception("权限不足");
                }
                await eventService.AddSystemMessageAsync(title, content, t.UserID);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Update")]
        public async Task<dynamic> OnUpdateSystemAnounceAsync([FromForm] int id, [FromForm] string title, [FromForm] string content, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("Token is Invalid!");
                }
                if (t.Role != UserRole.Admin)
                {
                    throw new Exception("权限不足");
                }
                await eventService.UpdateSystemMessageAsync(id, title, content, t.UserID);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Remove")]
        public async Task<dynamic> OnRemoveSystemAnounceAsync([FromForm] int id, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("Token is Invalid!");
                }
                if (t.Role != UserRole.Admin)
                {
                    throw new Exception("权限不足");
                }
                await eventService.RemoveSystemMessageAsync(id);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        public async Task<dynamic> OnGetSystemAnounceAsync(int start, int limit)
        {
            try
            {
                var list = await eventService.GetSystemMessageListAsync(start, limit);
                return new { Res = true, List = list };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        public async Task<dynamic> OnGetRelatedInfoAsync(int start, int limit, string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("Token is Invalid!");
                }
                if (t.Role != UserRole.Admin)
                {
                    var resList = await eventService.GetRelatedEventListAsync(t.UserID, start, limit);
                    return new { Res = true, EventList = resList };
                }
                var list = await eventService.GetSystemMessageListAsync(start, limit);
                return new { Res = true, EventList = list };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }
    }
}
