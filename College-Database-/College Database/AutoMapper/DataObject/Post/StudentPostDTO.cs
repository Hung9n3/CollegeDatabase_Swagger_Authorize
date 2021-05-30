using Entities.StudentCourses;
using Entity.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace College_Database.AutoMapper.DataObject.Post
{
    public class StudentPostDTO
    {
        public int StudentId { get; set; }
        public string IdCard { get; set; }
        public int Bill { get; set; }
        public bool Paid { get; set; }
        public ICollection<StudentCourses> StudentCourses { get; set; }
        public UserModel UserModel { get; set; }
    }
}
