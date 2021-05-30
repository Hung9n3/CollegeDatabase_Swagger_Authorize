
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Entity.Course;
using Entity.BaseModel;

namespace Entity.User
{
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }
        public bool IsHead { get; set; } = false;
        public int Salary { get; set; }
        public ICollection<Courses> Courses { get; set; }
        public Department Department { get; set; }
        public UserModel UserModel { get; set; }
    }
}
