using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace OTS_API.Services
{
    abstract public class DBService
    {
        private readonly ILogger<DBService> logger;
        protected MySqlConnection sqlConnection;

        public DBService(ILogger<DBService> logger)
        {
            this.logger = logger;
            this.sqlConnection = new MySqlConnection(Config.connStr);

            try
            {
                this.sqlConnection.Open();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw;
            }
        }

        ~DBService()
        {
            this.sqlConnection.Close();
        }
    }
}