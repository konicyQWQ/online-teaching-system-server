using Microsoft.Extensions.Logging;
using OTS_API.DatabaseContext;
using OTS_API.Models;
using OTS_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTS_API.Services
{
    public class PasswordRetrieveService
    {
        private readonly ILogger<PasswordRetrieveService> logger;

        private Dictionary<string, SToken> tokenMap;

        public PasswordRetrieveService(ILogger<PasswordRetrieveService> logger)
        {
            this.logger = logger;
            this.tokenMap = new Dictionary<string, SToken>();
        }

        public async Task<string> AddSTokenAsync(User userInfo, string email)
        {
            if (userInfo == null)
            {
                throw new Exception("User Not Found!");
            }
            if (userInfo.Email != email)
            {
                throw new Exception("Email is Incorrect!");
            }
            var t = new SToken(userInfo.Id, email);
            var tID = CodeGenerator.GetCode(20);
            t.ID = tID;
            await SendValidatingEmailAsync(t);

            tokenMap.Add(tID, t);
            return tID;
        }

        public Task VerifyAsync(string tokenID, string validationCode)
        {
            return Task.Run(() =>
            {
                if (tokenID == null || !tokenMap.ContainsKey(tokenID))
                {
                    throw new Exception("请先登录");
                }
                var token = tokenMap[tokenID];
                if (!token.IsValid())
                {
                    tokenMap.Remove(tokenID);
                    throw new Exception("请先登录");
                }
                if (token.ValidationCode != validationCode)
                {
                    throw new Exception("Code is Incorrect!");
                }
                token.IsVerified = true;
            });
        }

        public async Task ResendEmailAsync(string tokenID)
        {
            if (tokenID == null || !tokenMap.ContainsKey(tokenID))
            {
                throw new Exception("请先登录");
            }
            var token = tokenMap[tokenID];
            if (!token.IsValid())
            {
                tokenMap.Remove(tokenID);
                throw new Exception("请先登录");
            }
            await SendValidatingEmailAsync(token);
        }

        public Task<string> ResetVerifyAsync(string tokenID)
        {
            return Task.Run(() =>
            {
                if (tokenID == null || !tokenMap.ContainsKey(tokenID))
                {
                    throw new Exception("请先登录");
                }
                var token = tokenMap[tokenID];
                if (!token.IsValid())
                {
                    tokenMap.Remove(tokenID);
                    throw new Exception("请先登录");
                }
                if (!token.IsVerified)
                {
                    throw new Exception("Need to Verify Identity First!");
                }
                var userID = token.UserID;
                tokenMap.Remove(tokenID);
                return userID;
            });
        }

        private Task SendValidatingEmailAsync(SToken token)
        {
            return Task.Run(() =>
            {
                token.ValidationCode = CodeGenerator.GetCode(8);
                logger.LogInformation("Validation Code for User(id: " + token.UserID + ") is: " + token.ValidationCode);
            });
        }
    }
}
