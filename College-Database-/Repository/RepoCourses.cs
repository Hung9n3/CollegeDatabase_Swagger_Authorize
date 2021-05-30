using Contracts;
using Entity.Context;
using Entity.Course;
using Entity.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repository
{
    public class RepoCourses : RepoBase<Courses>, IRepoCourses
    {
        public RepoCourses(RepoContext repoContext) : base(repoContext)
        {
        }
        public override async Task<List<Courses>> FindAll()
        {
            var items = await _repoContext.Courses.Include(x => x.Teacher).ThenInclude(x => x.UserModel)
                                            .Include(x => x.Department).AsNoTracking().ToListAsync();
            return items;
        }
        public override async Task<Courses> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = await _repoContext.Courses.Include(x => x.Teacher).ThenInclude(x => x.UserModel).Include(x => x.Department).Include(x => x.StudentCourses)
                .ThenInclude(x => x.Student).ThenInclude(x => x.UserModel).Where(x => x.CoursesId == id).FirstAsync();
            return item;
        }
    }
}
