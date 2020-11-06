﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OTS_API.Models;

namespace OTS_API.DatabaseContext
{
    public class OTSDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public OTSDbContext(DbContextOptions<OTSDbContext> options) : base(options)
        {

        }
    }
}