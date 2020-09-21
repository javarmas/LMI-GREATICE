using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebApp_LMIGREATICE.Startup))]
namespace WebApp_LMIGREATICE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage(@"Data Source=JAAN\SQLSERVER2017;Initial Catalog=DB_LMIGREATICE;Integrated Security=True");
            app.UseHangfireDashboard();
            app.UseHangfireServer();

        }
    }
}
