using Entity.Course;
using Entity.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.StudentCourses
{
    public class StudentCourses
    {
        public int StudentId { get; set; }
        public int CoursesId { get; set; }
        public Student Student { get; set; }
        public Courses Courses { get; set; }
    }
}
