using Entities.StudentCourses;
using Entity.Course;
using Entity.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entity.Context
{
    public class RepoContext : IdentityDbContext
    {
        public RepoContext(DbContextOptions<RepoContext> options) : base(options)
        {

        }
        public DbSet<Courses> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
       public DbSet<Student> Students { get; set; }
       public DbSet<Teacher> Teachers { get; set; }
       public DbSet<UserModel> UserModel { get; set; }
        public DbSet<StudentCourses> StudentCourses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            modelBuilder.Entity<StudentCourses>().HasKey(sc => new { sc.StudentId, sc.CoursesId });
            base.OnModelCreating(modelBuilder);
        }
    }
}
