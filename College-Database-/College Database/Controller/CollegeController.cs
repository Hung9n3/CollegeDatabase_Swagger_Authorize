using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using College_Database.AutoMapper.DataObject;
using College_Database.AutoMapper.DataObject.Get;
using College_Database.AutoMapper.DataObject.Post;
using Contracts;
using Entities.User;
using Entity.Context;
using Entity.Course;
using Entity.User;
using ExcelDataReader;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OfficeOpenXml;

namespace College_Database.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CollegeController : ControllerBase
    {
        private UserManager<UserModel> _userManager;
        private SignInManager<UserModel> _signInManager;
        private readonly ApplicationSettings _appSettings;
        private RepoContext _repoContext;
        private IMapper _mapper;
        private IRepoTeacher _repoTeacher;
        private IRepoStudent _repoStudent;
        private IRepoCourses _repoCourses;
        private IRepoDepartment _repoDepartment;
        private readonly IWebHostEnvironment _environment;
        public string path { get; set; }
        public CollegeController(IMapper mapper, IRepoStudent repoStudent, IRepoTeacher repoTeacher 
                                , RepoContext repoContext, UserManager<UserModel> userManager,
                                SignInManager<UserModel> signInManager, IOptions<ApplicationSettings> appSettings, IRepoCourses repoCourses,IRepoDepartment repoDepartment,IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _repoContext = repoContext;
            _repoCourses = repoCourses;
            _repoTeacher = repoTeacher;
            _repoStudent = repoStudent;
            _mapper = mapper;
            _repoDepartment = repoDepartment;
            _environment = environment;
        }
       [HttpPost]
        public async Task<IActionResult> AddDepartment(DepartmentPostDTO _department)
        {
            var check = await _repoContext.Departments.ToListAsync();
            foreach(Department c in check)
            {
                if(c.DepartmentName ==_department.DepartmentName)
                {
                    return Ok("Duplicate department name");
                }
            }
            var department = _mapper.Map<Department>(_department);
            _repoContext.Add(department);
            await _repoContext.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> AddCourses(CoursesPostDTO _course)
        {
            var department = await _repoContext.Departments.FindAsync(_course.DepartmentId);
            var teacher = await _repoContext.Teachers.FindAsync(_course.TeacherId);
            _course.Department = department;
            _course.Teacher = teacher;
            _course.Rest = _course.Size;
            _repoContext.Add(_mapper.Map<Courses>(_course));
            await _repoContext.SaveChangesAsync();
            return Ok();
        }
        [HttpGet]
        [Authorize(Roles = Role.Teacher)]
        public async Task<List<CoursesDTO>> GetAllCourses()
        {
            var items = await _repoCourses.FindAll();
            List<CoursesDTO> _courses = new List<CoursesDTO>(); 
            foreach(Courses c in items)
            {
                var course = _mapper.Map<CoursesDTO>(c);
                course.Teacher.TeacherName = c.Teacher.UserModel.FullName;
                _courses.Add(course);
            }
            return _courses;
        }
        [HttpGet]
        [Authorize(Roles = Role.Student)]
        public async Task<List<CoursesDTO>> GetAllCoursesStudentRole()
        {
            var items = await _repoCourses.FindAll();
            List<CoursesDTO> _courses = new List<CoursesDTO>();
            foreach (Courses c in items)
            {
                var course = _mapper.Map<CoursesDTO>(c);
                course.Teacher.TeacherName = c.Teacher.UserModel.FullName;
                _courses.Add(course);
            }
            return _courses;
        }
        [HttpGet("{id}")]
        public async Task<CoursesDTO> GetCoursesById(int id)
        {
            var course = await _repoContext.Courses.Include(x => x.Teacher).ThenInclude(x => x.UserModel).
                Include(x => x.StudentCourses).ThenInclude(x => x.Student).ThenInclude(x => x.UserModel).Include(x => x.Department)
                .Where(x => x.CoursesId == id).FirstAsync();
            var _course = _mapper.Map<CoursesDTO>(course);
            if (course.Teacher.UserModel.FullName != null)
            {
                _course.Teacher.TeacherName = course.Teacher.UserModel.FullName;
            }
            return _course;
        }
        [HttpGet]
        [Authorize(Policy = "TeacherOnly")]
        public async Task<List<Department>> GetDepartments()
        {
            var result = await _repoContext.Departments.Include(x => x.Courses).Include(x=>x.Teachers).Include(x=>x.Students).ToListAsync();
            return result;
        }

        
        [HttpPost("{id}")]
        public async Task<IActionResult> TeacherCourses(List<int> listCourses, int id)
        {
            var teacher = await _repoTeacher.FindByIdAsync(id);
            foreach(int i in listCourses)
            {
                var course = await _repoContext.Courses.FindAsync(i);
                teacher.Courses.Add(course);
            }
            _repoTeacher.Update(teacher);
            await _repoTeacher.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("{cityid}/{districtid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUserInfo(ApplicationUserModel applicationUserModel, int cityid,int districtid)
        {

            var userId = User.Claims.First(c => c.Type == "UserId").Value;
            var user = await _repoContext.UserModel
                .FirstOrDefaultAsync(x => x.Id == userId);
            _mapper.Map(applicationUserModel, user);
            await _repoContext.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("{id}")]
        public async Task<List<TeacherDTO>> GetTeacherByDepartment(int id)
        {
            var Teacher = await _repoContext.Teachers.Include(x => x.Courses).Include(x => x.UserModel).Include(x => x.Department)
                .Where(x => x.Department.DepartmentId == id).ToListAsync();
            List<TeacherDTO> teacher = new List<TeacherDTO>();
            
            foreach (Teacher t in Teacher)
            {
                var _teacher = _mapper.Map<TeacherDTO>(t);
                
                teacher.Add(_teacher);
            }
          
            return teacher;
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCourses(CoursesPostDTO _courses)
        {
            var teacher = await _repoTeacher.FindByIdAsync(_courses.TeacherId);
            _courses.Teacher = teacher;
            var department = await _repoContext.Departments.FindAsync(_courses.DepartmentId);
            _courses.Department = department;
            var course = await _repoCourses.FindByIdAsync(_courses.CoursesId);
            _mapper.Map(_courses, course);
            _repoContext.Courses.Update(course);
            await _repoCourses.SaveChangesAsync();
            return Ok();
        }
        [HttpGet]
        public async Task<List<StudentDTO>> GetAllStudent()
        {
            var students = await _repoStudent.FindAll();
            List<StudentDTO> _students = new List<StudentDTO>();
            foreach(Student s in students)
            {
                var _student = _mapper.Map<StudentDTO>(s);
                _students.Add(_student);
            }
            return _students;
        }
        [HttpGet("{id}")]
        public async Task<List<StudentDTO>> GetStudentByDepartment(int id)
        {
            var students = await _repoContext.Students.Include(x => x.Department).Include(x => x.UserModel).Include(x => x.StudentCourses).ThenInclude(x => x.Courses)
                .ThenInclude(x => x.Teacher)
                .Where(x => x.Department.DepartmentId == id).ToListAsync();
            List<StudentDTO> _students = new List<StudentDTO>();
            foreach (Student s in students)
            {
                var _student = _mapper.Map<StudentDTO>(s);
                _students.Add(_student);
            }
            return _students;
        }
        [HttpGet]
        public async Task<List<TeacherDTO>> GetAllTeacher()
        {
            var teachers = await _repoTeacher.FindAll();
            List<TeacherDTO> _teachers = new List<TeacherDTO>();
            foreach (Teacher s in teachers)
            {
                var _teacher = _mapper.Map<TeacherDTO>(s);
                _teachers.Add(_teacher);
            }
            return _teachers;
        }
        [HttpPut]
        public async Task<IActionResult> UpdateUserInfo(ApplicationUserModel _user)
        {
            var user = await _repoContext.UserModel.FindAsync(_user.Id);
            _mapper.Map(_user, user);
            var department = await _repoContext.Departments.FindAsync(_user.DepartmentId);
            var _teacher = new Teacher();
            var _student = new Student();
            if(user.Role == "teacher")
            {
                var teacher = await _repoContext.Teachers.Include(x => x.Department).Where(x => x.UserModel.Id == user.Id).FirstAsync();
                teacher.Department = department;
                _mapper.Map(_user, teacher);
                _teacher = teacher;
            }
            if(user.Role == "student")
            {
                var student = await _repoContext.Students.Include(x => x.Department).Where(x => x.UserModel.Id == user.Id).FirstAsync();
                student.Department = department;
                _mapper.Map(_user, student);
                _student = student;
            }
            _repoContext.Update<Teacher>(_teacher);
            _repoContext.Update<Student>(_student);
            _repoContext.Update<UserModel>(user);
            await _repoContext.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCourse(List<int> id)
        {
           await _repoCourses.Delete(id);
            await _repoCourses.SaveChangesAsync();
            return Ok();
        }



        private async Task UploadAsync(IFormFile fileEntry)
        {
            path = Path.Combine(_environment.ContentRootPath, "Upload", fileEntry.Name);
            var ms = new MemoryStream();
            await fileEntry.CopyToAsync(ms);
            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                ms.WriteTo(file);
            }
        }


        [HttpPost]
        public async Task<IActionResult> UploadDepartment(IFormFile file)
        {
            await UploadAsync(file);

            var fileName = path;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {

                IExcelDataReader reader;

                reader = ExcelReaderFactory.CreateReader(stream);

                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                };

                var dataSet = reader.AsDataSet(conf);

                var dataTable = dataSet.Tables[0];


                for (var i = 0; i < dataTable.Rows.Count; i++)
                {
                    DepartmentPostDTO department = new DepartmentPostDTO
                    {
                        DepartmentName = dataTable.Rows[i][1].ToString(),
                    };

                    var result = await AddDepartment(department);

                }

            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UploadCourse(IFormFile file)
        {
            await UploadAsync(file);

            var fileName = path;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {

                IExcelDataReader reader;

                reader = ExcelReaderFactory.CreateReader(stream);

                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                };

                var dataSet = reader.AsDataSet(conf);

                var dataTable = dataSet.Tables[0];


                for (var i = 0; i < dataTable.Rows.Count; i++)
                {
                    DayOfWeek dof = DayOfWeek.Monday;
                    switch (dataTable.Rows[i][6].ToString())
                    {
                        case "Monday":
                            dof = DayOfWeek.Monday;
                            break;
                        case "Tuesday":
                            dof = DayOfWeek.Tuesday;
                            break;
                        case "Wednesday":
                            dof = DayOfWeek.Wednesday;
                            break;
                        case "Thursday":
                            dof = DayOfWeek.Thursday;
                            break;
                        case "Friday":
                            dof = DayOfWeek.Friday;
                            break;
                        case "Saturday":
                            dof = DayOfWeek.Saturday;
                            break;
                    }
                    CoursesPostDTO course = new CoursesPostDTO
                    {
                        CoursesName = dataTable.Rows[i][1].ToString(),
                        DepartmentId = _repoContext.Departments.Where(x => x.DepartmentName == dataTable.Rows[i][2].ToString()).FirstOrDefault().DepartmentId,
                        TeacherId =   _repoContext.Teachers.Include(x => x.UserModel).Include(x => x.Department).Where(x => x.UserModel.FullName == dataTable.Rows[i][3].
                        ToString() && x.Department.DepartmentId == _repoContext.Departments.Where(x => x.DepartmentName == dataTable.Rows[i][2].ToString()).FirstOrDefault().DepartmentId).FirstOrDefault().TeacherId,
                        Credits = Int32.Parse(dataTable.Rows[i][4].ToString()),
                        Size = Int32.Parse(dataTable.Rows[i][5].ToString()),
                        Day = dof,
                        StartPeriod = Int32.Parse(dataTable.Rows[i][7].ToString()),
                        Periods = Int32.Parse(dataTable.Rows[i][8].ToString()),
                        Room = dataTable.Rows[i][9].ToString(),
                    };

                    var result = await AddCourses(course);

                }

            }
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ExportExcel(int id)
        {
            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                // Set author for excel file
                package.Workbook.Properties.Author = "Aspodel";
                // Set title for excel file
                package.Workbook.Properties.Title = "Student List";
                // Add comment to excel file
                package.Workbook.Properties.Comments = "Hello (^_^)";
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = "No";
                worksheet.Cells[1, 2].Value = "Student Name";
                worksheet.Cells[1, 3].Value = "Id Card";
                worksheet.Cells[1, 4].Value = "Birthdate";

                // Style for Excel 
                worksheet.DefaultColWidth = 12;
                worksheet.Column(2).Width = 35;
                worksheet.Cells.Style.Font.Size = 16;


                //Export Data from Table employees

                var studentList =  GetCoursesById(id).Result.Students;
                var list = studentList.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    worksheet.Cells[i + 2, 1].Value = i + 1;
                    worksheet.Cells[i + 2, 2].Value = item.UserModel.FullName;
                    worksheet.Cells[i + 2, 3].Value = item.IdCard;
                    worksheet.Cells[i + 2, 4].Value = item.UserModel.BirthDate.ToString("MM/dd/yyyy");
                }

                fileContents = package.GetAsByteArray();
            }

            if (fileContents == null || fileContents.Length == 0)
            {
                return NoContent();
            }

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "Students.xlsx");
        }

    }
}
