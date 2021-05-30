using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using College_Database.AutoMapper.DataObject;
using College_Database.AutoMapper.DataObject.Get;
using College_Database.AutoMapper.DataObject.Post;
using Entity.Course;
using Entity.User;

namespace College_Database.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUserModel, UserModel>().ForMember(x => x.Id, opt => opt.Ignore());
            CreateMap<ApplicationUserModel, Student>().ForMember(x => x.StudentId, opt => opt.Ignore());
            CreateMap<ApplicationUserModel, Teacher>().ForMember(x => x.TeacherId, opt => opt.Ignore());
            CreateMap<UserModel, UserDTO>();
            CreateMap<DepartmentDTO, Department>().ForMember(x => x.DepartmentId, opt => opt.Ignore());
            CreateMap<Department, DepartmentGetDTO>();
            CreateMap<CoursesPostDTO, Courses>().ForMember(x => x.CoursesId, opt => opt.Ignore()).ForMember(x => x.StudentCourses, opt => opt.Ignore());
            CreateMap<Courses, CoursesDTO>().ForMember(x => x.Students, c => c.MapFrom(c => c.StudentCourses.Select(cs => cs.Student)));
            CreateMap<Student, UserDTO>().ForMember(x => x.Department, c => c.MapFrom(c => c.Department))
                .ForMember(x => x.Courses, c => c.MapFrom(c => c.StudentCourses.Select(cs => cs.Courses)));
            CreateMap<Teacher, UserDTO>().ForMember(x => x.Department, c => c.MapFrom(c => c.Department)).ForMember(x => x.Courses, c => c.MapFrom(c => c.Courses));
            CreateMap<Teacher, TeacherDTO>().ForMember(x => x.UserModel, c => c.MapFrom(c => c.UserModel))
                .ForMember(x => x.Department, c => c.MapFrom(c => c.Department)).ForMember(x => x.Courses, c => c.MapFrom(c => c.Courses));
            CreateMap<Teacher, TeacherGetDTO>();
            CreateMap<Student, StudentDTO>().ForMember(x => x.Courses, c => c.MapFrom(c => c.StudentCourses.Select(cs => cs.Courses)))
                .ForMember(x => x.UserModel, c => c.MapFrom(c => c.UserModel)).ForMember(x => x.Department, c => c.MapFrom(c => c.Department));
            CreateMap<DepartmentPostDTO, Department>().ForMember(x => x.DepartmentId, opt => opt.Ignore());
            CreateMap<Department, DepartmentGetDTO>();
            CreateMap<Courses, CoursesGetDTO>();
            CreateMap<Student, StudentGetDTO>();
        }
    }
}
