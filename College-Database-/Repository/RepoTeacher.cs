using Contracts;
using Entity.Context;
using Entity.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repository
{
    public class RepoTeacher : RepoBase<Teacher>, IRepoTeacher
    {
        public RepoTeacher(RepoContext repoContext) : base(repoContext)
        {

        }
        public override async Task<Teacher> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = await _repoContext.Teachers.Include(x => x.Courses).Include(x => x.Department).Include(x => x.UserModel).FirstOrDefaultAsync(x => x.TeacherId == id);
            return item;
        }
        public override async Task<List<Teacher>> FindAll()
        {
            var items = await _repoContext.Teachers.Include(x => x.Courses).Include(x => x.Department).Include(x => x.UserModel).ToListAsync();
            return items;
        }
    }
}