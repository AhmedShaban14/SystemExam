using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ExamSystem.Startup))]
namespace ExamSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
