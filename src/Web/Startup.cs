using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.DependencyInjection;
using  Microsoft.Framework.Logging;

namespace WebAPIApplication
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
        }

        // This method gets called by a runtime.
        // Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Cors support to the service
            services.AddCors();
            
            services.AddLogging();
            
            services.AddMvc();
            
            // Add all SignalR related services to IoC.
            services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
                options.Hubs.EnableJavaScriptProxies = false;
            });
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
            // Enable CORS
            app.UseCors(policy => policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            
            // Configure the HTTP request pipeline.
            app.UseStaticFiles();

            // Add MVC to the request pipeline.
            app.UseMvc();
            
            // Configure SignalR
            app.UseSignalR();
        }
    }
}