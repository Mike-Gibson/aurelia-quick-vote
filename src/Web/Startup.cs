using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.Logging;
using WebAPIApplication.Data;
using Microsoft.Framework.DependencyInjection;

namespace WebAPIApplication
{
    public class Startup
    {
        private IConfigurationRoot _configuration;
        
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(appEnv.ApplicationBasePath)
              .AddJsonFile("config.json")
              .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            _configuration = builder.Build();
        }

        // This method gets called by a runtime.
        // Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            
            services.AddMvc();
            
            // Add all SignalR related services to IoC.
            services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
                options.Hubs.EnableJavaScriptProxies = false;
            });
            
            // Add EF services to the services container.
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<DataContext>(options => options.UseSqlServer(_configuration["Data:DefaultConnection:ConnectionString"]));
                
            services.AddScoped<IVotingRepository, VotingRepository>();
            services.AddScoped<IUnitOfWork>(provider => provider.GetService<DataContext>());
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
        {
            loggerfactory.AddConsole();

//              app.UseErrorHandler(errorApp =>
//              {
//                  // Normally you'd use MVC or similar to render a nice page.
//                  errorApp.Run(async context =>
//                  {
//                      context.Response.StatusCode = 500;
//                      context.Response.ContentType = "text/html";
//                      await context.Response.WriteAsync("<html><body>\r\n");
//                      await context.Response.WriteAsync("We're sorry, we encountered an un-expected issue with your application.<br>\r\n");
//  
//                      var error = context.GetFeature<IErrorHandlerFeature>();
//                      if (error != null)
//                      {
//                          // This error would not normally be exposed to the client
//                          await context.Response.WriteAsync("<br>Error: " + HtmlEncoder.Default.HtmlEncode(error.Error.Message) + "<br>\r\n");
//                      }
//                      await context.Response.WriteAsync("<br><a href=\"/\">Home</a><br>\r\n");
//                      await context.Response.WriteAsync("</body></html>\r\n");
//                      await context.Response.WriteAsync(new string(' ', 512)); // Padding for IE
//                  });
//              });
//              

            // Add the platform handler to the request pipeline.
            app.UseIISPlatformHandler();
            
            // Configure the HTTP request pipeline.
            app.UseStaticFiles();

            // Add MVC to the request pipeline.
            app.UseMvc();
            
            // Configure SignalR
            app.UseSignalR();
        }
    }
}