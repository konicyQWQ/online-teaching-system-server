using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data;

namespace OTS_API.Services
{
    public class TokenService : DBService
    {
        public TokenService(ILogger<TokenService> logger, ILogger<DBService> logger1)
            : base(logger1)
        {

        }

        public Task<string> SetToken(string userID)
        {
            return Task.Run(() =>
            {
                return "lasjflk";
            });
        }
    }
}
