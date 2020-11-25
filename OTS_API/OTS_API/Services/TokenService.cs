using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data;
using OTS_API.Models;
using OTS_API.Utilities;

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
            tokenMap.Add("MagicToken", new Token("Admin", UserRole.Admin, 24));
        }

        /// <summary>
        /// 添加一个token，返回tokenID
        /// </summary>
        /// <param name="token">要添加的token</param>
        /// <returns>tokenID</returns>
        public Task<string> AddTokenAsync(Token token)
        {
            return Task.Run(() =>
            {
                var t = this.tokenMap.Values.FirstOrDefault(t => t.UserID == token.UserID);
                if(t != null)
                {
                    this.tokenMap.Remove(t.ID);
                }

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

        /// <summary>
        /// 返回ID对应的token，同时检查是否过期
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>返回对应token，若过期或不存在返回null</returns>
        public Task<Token> GetTokenAsync(string id)
        {
            return Task.Run(() =>
            {
                if(id == null)
                {
                    return null;
                }
                if (tokenMap.ContainsKey(id))
                {
                    var token = tokenMap[id];
                    if (token.IsValid())
                    {
                        return token;
                    }
                    else
                    {
                        tokenMap.Remove(id);
                    }
                }
                return null;
            });
        }
    }
}
