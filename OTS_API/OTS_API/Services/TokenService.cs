using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data;
using OTS_API.Models;
using OTS_API.Common;

namespace OTS_API.Services
{
    public class TokenService
    {
        private readonly ILogger<TokenService> logger;
        private Dictionary<string, Token> tokenMap;

        public TokenService(ILogger<TokenService> logger)
        {
            this.logger = logger;
            tokenMap = new Dictionary<string, Token>();
        }

        public Task<string> SetToken(Token token)
        {
            return Task.Run(() =>
            {
                var code = CodeGenerator.GetCode(15);
                while (tokenMap.ContainsKey(code))
                {
                    code = CodeGenerator.GetCode(15);
                }
                token.ID = code;
                this.tokenMap.Add(code, token);

                return code;
            });
        }
    }
}
