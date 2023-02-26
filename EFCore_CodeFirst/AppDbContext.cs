using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFCore_CodeFirst
{
    using System.Reflection.Emit;
    using System.Windows.Controls;

    public class AppDbContext : DbContext
    {
        private const string directory = "D:\\ITStep\\CSharp\\EFCore\\2023_02_24_EFCore_CodeFirst\\EFCore_CodeFirst\\StudentVisit.db";
        private const string ConnectionString = $"Data Source = {directory}";

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Visitation> Visitations => Set<Visitation>();
    }
}
