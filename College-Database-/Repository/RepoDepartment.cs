using Contracts;
using Entity.Context;
using Entity.Course;
using Entity.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class RepoDepartment : RepoBase<Department>, IRepoDepartment
    {
        public RepoDepartment(RepoContext repoContext) : base(repoContext)
        {

        }
    }
}
