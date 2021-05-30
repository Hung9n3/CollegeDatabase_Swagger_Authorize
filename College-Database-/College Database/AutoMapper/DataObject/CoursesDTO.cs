using College_Database.AutoMapper.DataObject.Get;
using Entities.StudentCourses;
using Entity.BaseModel;
using Entity.Course;
using Entity.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace College_Database.AutoMapper.DataObject
{
    public class CoursesDTO
    {
        public int CoursesId { get; set; }
        public string CoursesName { get; set; }
        public DayOfWeek Day { get; set; }
        public DepartmentGetDTO Department { get; set; }
        public TeacherGetDTO Teacher { get; set; }
        public ICollection<StudentGetDTO> Students { get; set; }
        public int Size { get; set; }
        public int Rest { get; set; }
        public int Credits { get; set; }
        public string Room { get; set; }
        public int StartPeriod { get; set; }
        public int Periods { get; set; }
    }
}
