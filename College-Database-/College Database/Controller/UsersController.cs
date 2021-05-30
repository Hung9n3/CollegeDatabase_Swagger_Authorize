
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Entities.User;
using Entity.Context;
using Entity.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using Contracts;
using College_Database.AutoMapper.DataObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Entities.StudentCourses;
using Entity.Course;
using College_Database.AutoMapper.DataObject.Get;
using System.IO;
using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;

namespace WebApplication4.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserManager<UserModel> _userManager;
        private SignInManager<UserModel> _signInManager;
        private readonly ApplicationSettings _appSettings;
        private RepoContext _repoContext;
        private IMapper _mapper;
        private IRepoTeacher _repoTeacher;
        private IRepoStudent _repoStudent;
        private IRepoCourses _repoCourses;
        private readonly IWebHostEnvironment _environment;
        public string path { get; set; }


        public UserController(IMapper mapper,IRepoStudent repoStudent,IRepoTeacher repoTeacher,
              RepoContext repoContext,UserManager<UserModel> userManager, 
            SignInManager<UserModel> signInManager, IOptions<ApplicationSettings> appSettings, IRepoCourses repoCourses, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _repoContext = repoContext;
            _repoTeacher = repoTeacher;
            _repoCourses = repoCourses;
            _repoStudent = repoStudent;
            _mapper = mapper;
            _environment = environment;
        }
        
       
        [HttpPost]
        public async Task<Object> Register(ApplicationUserModel model)
        {
           
            var applicationUser = _mapper.Map<UserModel>(model);
            var result = await _userManager.CreateAsync(applicationUser, model.Password);
            var user = _repoContext.UserModel.Where(x => x.UserName.Contains(applicationUser.UserName)).FirstOrDefault();
            var department = await _repoContext.Departments.FindAsync(model.DepartmentId);
            if (result.Succeeded == true)
            {
                switch (applicationUser.Role)
                {
                    case "teacher":
                        {
                            try
                            {
                                var teacher = new Teacher();
                                teacher.Department = department;
                                teacher.UserModel = user;
                                _repoContext.Teachers.Add(teacher);
                                _repoContext.SaveChanges();
                                return Ok(result);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    case "student":
                        {
                            try
                            {
                                var student = new Student();
                                student.IdCard = model.IdCard;
                                student.UserModel = user;
                                student.Department = department;
                                _repoContext.Students.Add(student);
                                _repoContext.SaveChanges();
                                return Ok(result);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    case "admin":
                        {
                            try
                            {
                                return Ok(result);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    default:
                        return BadRequest("Lack of role !!");
                }
            }
            else return result;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserId", user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Role.ToString()),
                        new Claim("Phone",user.Phone.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }
            else
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<UserDTO> GetUserInfo()
        {
            string userId = User.Claims.First(c => c.Type == "UserId").Value;
            var user = await _repoContext.UserModel
                .Where(x => x.Id == userId).FirstOrDefaultAsync();
             var _user = _mapper.Map<UserDTO>(user);
            if(user.Role == "student")
            {
                var student = await _repoContext.Students.Include(x => x.StudentCourses).ThenInclude(x => x.Courses).Include(x => x.Department)
                    .Where(x => x.UserModel.Id == userId).FirstAsync();
                _mapper.Map(student,_user);
                IEnumerable<CoursesGetDTO> _course = _user.Courses.OrderBy(x => x.Day).ThenBy(x => x.StartPeriod);
                _user.Courses = _course.ToList();
            }
            if(user.Role == "teacher")
            {
                var teacher = await _repoContext.Teachers.Include(x => x.Courses).Include(x => x.Department).Where(x => x.UserModel.Id == userId).FirstAsync();
                _mapper.Map(teacher, _user);

                IEnumerable<CoursesGetDTO> _course = _user.Courses.OrderBy(x => x.Day).ThenBy(x => x.StartPeriod);
                _user.Courses = _course.ToList();
            }
            return _user;
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ApplyCourse(List<int> listCourses)
        {
            string userId = User.Claims.First(c => c.Type == "UserId").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var student = await _repoContext.Students.Include(x => x.UserModel).Where(x => x.UserModel.Id == userId).FirstOrDefaultAsync();
            foreach (int i in listCourses)
            {
                var courses = await _repoContext.Courses.FindAsync(i);
                courses.Rest = courses.Rest - 1;
                var studentcourses = new StudentCourses()
                {
                    Courses = courses,
                    Student = student
                };
                _repoCourses.Update(courses);
                _repoContext.StudentCourses.Add(studentcourses);
            }
           await _repoContext.SaveChangesAsync();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> UploadStudent(IFormFile file)
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
                    ApplicationUserModel user = new ApplicationUserModel
                    {
                        UserName = dataTable.Rows[i][0].ToString(),
                        FullName = dataTable.Rows[i][1].ToString(),
                        Password = "123456",
                        Phone = Int32.Parse(dataTable.Rows[i][2].ToString()),
                        Address = dataTable.Rows[i][3].ToString(),
                        Role = "student",
                        DepartmentId = _repoContext.Departments.Where(x => x.DepartmentName == dataTable.Rows[i][5].ToString()).FirstOrDefault().DepartmentId,
                        BirthDate = DateTime.Parse(dataTable.Rows[i][4].ToString()),
                        IdCard = dataTable.Rows[i][0].ToString(),
                    };

                    var result = await Register(user);

                }

            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UploadTeacher(IFormFile file)
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
                    ApplicationUserModel user = new ApplicationUserModel
                    {
                        UserName = dataTable.Rows[i][0].ToString(),
                        FullName = dataTable.Rows[i][1].ToString(),
                        Password = "123456",
                        Phone = Int32.Parse(dataTable.Rows[i][2].ToString()),
                        Address = dataTable.Rows[i][3].ToString(),
                        Role = "teacher",
                        DepartmentId = _repoContext.Departments.Where(x => x.DepartmentName == dataTable.Rows[i][4].ToString()).FirstOrDefault().DepartmentId,
                        BirthDate = DateTime.Parse(dataTable.Rows[i][5].ToString())
                    };

                    var result = await Register(user);

                }

            }
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
    }
}