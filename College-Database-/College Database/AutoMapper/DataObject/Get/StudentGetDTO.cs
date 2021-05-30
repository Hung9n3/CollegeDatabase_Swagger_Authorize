using Entity.Course;
using Entity.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace College_Database.AutoMapper.DataObject.Get
{
    public class StudentGetDTO
    {
        public int StudentId { get; set; }
        public string IdCard { get; set; }
        public int Bill { get; set; }
        public UserDTO UserModel { get; set; }
    }
}
