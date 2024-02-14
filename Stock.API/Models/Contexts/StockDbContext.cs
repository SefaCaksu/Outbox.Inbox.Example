using System;
using Microsoft.EntityFrameworkCore;
using Stock.API.Models.Entities;

namespace Stock.API.Models.Contexts
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<OrderInbox> OrderInboxes { get; set; }
    }
}

