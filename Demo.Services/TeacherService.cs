using Demo.Models;
using EasyDo.Application;
using EasyDo.Dependency;
using EasyDo.Domain;

namespace Demo.Services
{
    //[Aspect(typeof())]
    public class TeacherService : CrudAppService<Teacher>, ITeacherService
    {
        public TeacherService(IRepository<Teacher> repository) : base(repository)
        {
            
        }
    }
}
