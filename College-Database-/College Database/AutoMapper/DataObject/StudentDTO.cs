using College_Database.AutoMapper.DataObject.Get;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace College_Database.AutoMapper.DataObject
{
    public class StudentDTO
    {
        public int StudentId { get; set; }
        public string IdCard { get; set; }
        public int Bill { get; set; }
        public bool Paid { get; set; }
        public DepartmentGetDTO Department { get; set; }
        public ICollection<CoursesGetDTO> Courses { get; set; }
        public UserDTO UserModel { get; set; }
    }
}
