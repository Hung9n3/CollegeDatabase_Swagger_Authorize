using Entity.Course;
using Entity.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace College_Database.AutoMapper.DataObject.Post
{
    public class TeacherPostDTO
    {
        public int TeacherId { get; set; }
        public bool IsHead { get; set; } = false;
        public int Salary { get; set; }
        public ICollection<Courses> Courses { get; set; }
        public Department Department { get; set; }
        public UserModel UserModel { get; set; }
    }
}
