using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace College_Database.AutoMapper.DataObject.Get
{
    public class CoursesGetDTO
    {
        public int CoursesId { get; set; }
        public string CoursesName { get; set; }
        public DayOfWeek Day { get; set; }
        public DepartmentGetDTO Department { get; set; }
        public TeacherGetDTO Teacher { get; set; }
        public int Size { get; set; }
        public int Rest { get; set; }
        public int Credits { get; set; }
        public string Room { get; set; }
        public int StartPeriod { get; set; }
        public int Periods { get; set; }
    }
}
