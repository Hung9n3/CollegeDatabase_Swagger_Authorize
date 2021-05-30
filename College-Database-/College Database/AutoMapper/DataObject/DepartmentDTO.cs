using Entity.BaseModel;
using Entity.Course;
using Entity.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace College_Database.AutoMapper.DataObject
{
    public class DepartmentDTO 
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public ICollection<Courses> Courses { get; set; }
        public ICollection<Teacher> Teachers { get; set; }
    }
}
