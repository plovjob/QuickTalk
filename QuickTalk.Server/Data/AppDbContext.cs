﻿using QuickTalk.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace QuickTalk.Server.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
        {
            
        }
    }
}
