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

        public Task<string> AddSTokenAsync(User userInfo, string email)
        {
            return Task.Run(() =>
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
                t.ValidationCode = CodeGenerator.GetCode(8);

                logger.LogInformation("Validation Code for User(id: " + userInfo.Id + ") is: " + t.ValidationCode);

                tokenMap.Add(tID, t);
                return tID;
            });
        }

        public Task VerifyAsync(string tokenID, string validationCode)
        {
            return Task.Run(() =>
            {
                if (tokenID == null || !tokenMap.ContainsKey(tokenID))
                {
                    throw new Exception("Token is Invalid!");
                }
                var token = tokenMap[tokenID];
                if (!token.IsValid())
                {
                    tokenMap.Remove(tokenID);
                    throw new Exception("Token is Invalid!");
                }
                if (token.ValidationCode != validationCode)
                {
                    token.ValidationCode = CodeGenerator.GetCode(10);

                    logger.LogInformation("Validation Code for User(id: " + token.UserID + ") is: " + token.ValidationCode);

                    throw new Exception("Code is Incorrect!");
                }
                token.IsVerified = true;
            });
        }

        public Task<string> ResetVerifyAsync(string tokenID)
        {
            return Task.Run(() =>
            {
                if (tokenID == null || !tokenMap.ContainsKey(tokenID))
                {
                    throw new Exception("Token is Invalid!");
                }
                var token = tokenMap[tokenID];
                if (!token.IsValid())
                {
                    tokenMap.Remove(tokenID);
                    throw new Exception("Token is Invalid!");
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
    }
}
