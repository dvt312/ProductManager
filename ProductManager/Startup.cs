using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProductManager.Startup))]
namespace ProductManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
