using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZuHuanJingDemo2.Models;

namespace ZuHuanJingDemo2.Data
{
    public class ZuHuanJingDemo2Context : DbContext
    {
        public ZuHuanJingDemo2Context (DbContextOptions<ZuHuanJingDemo2Context> options)
            : base(options)
        {
        }

        public DbSet<ZuHuanJingDemo2.Models.Member> Member { get; set; } = default!;

        public DbSet<ZuHuanJingDemo2.Models.Course>? Course { get; set; }

        public DbSet<ZuHuanJingDemo2.Models.License>? License { get; set; }
    }
}
