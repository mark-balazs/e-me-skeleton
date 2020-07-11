﻿using System;
using System.Collections.Generic;
using System.Text;
using e_me.Model.Model;
using Microsoft.EntityFrameworkCore;

namespace e_me.Model.DbContext
{
    public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Setting> Settings { get; set; }
    }
}
