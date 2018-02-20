using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CkEditorSample.Startup))]
namespace CkEditorSample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
