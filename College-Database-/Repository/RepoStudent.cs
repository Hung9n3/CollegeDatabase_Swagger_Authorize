using Contracts;
using Entity.Context;
using Entity.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepoStudent : RepoBase<Student>,IRepoStudent
    {
        public RepoStudent(RepoContext repoContext): base (repoContext)
        {

        }
        public override async Task<List<Student>> FindAll()
        {
            var student = await _repoContext.Students.Include(x => x.Department).Include(x => x.StudentCourses).ThenInclude(x => x.Courses).ThenInclude(x => x.Teacher)
.Include(x => x.UserModel)

                .ToListAsync();
            return student;
        }
    }
}
