using College_Database.AutoMapper.DataObject.Get;
using Entities.StudentCourses;
using Entity.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace College_Database.AutoMapper.DataObject
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public string IdCard { get; set; }
        public DepartmentGetDTO Department { get; set; }
        public int Bill { get; set; }
        public bool Paid { get; set; }
        public int salary { get; set; }
        public int Phone { get; set; }
        public string Role { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address {get;set; }
        public ICollection<CoursesGetDTO> Courses { get; set; }
    }
}
