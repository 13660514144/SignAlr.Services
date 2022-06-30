using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.HubService;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //配置SignalR服务 配置跨域
            services.AddCors(options => options.AddPolicy("CorsPolicy",
              builder =>
              {
                  builder.AllowAnyMethod()
                 .AllowAnyHeader()
                 .WithOrigins()
                 .AllowCredentials();
              }));
            services.AddControllers();
            services.AddSingleton<ClientObject>();
            services.AddSignalR().AddHubOptions<HubMessage>(options =>
            {
                options.MaximumReceiveMessageSize = 1024 * 1024;// long.MaxValue;                
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                options.EnableDetailedErrors = true;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 开启静态页面
            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);
            //app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot")),
                // 下面Linux 使用
                /*FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))*/
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            //使用跨域			
            app.UseCors("CorsPolicy");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<HubMessage>("/SignalrHub");//signal 总线
            });
        }
    }
}
/*signalr web client
 *（1）在“解决方案资源管理器” 中，右键单击项目，然后选择“添加” >“客户端库” 。
（2）在“添加客户端库” 对话框中，对于“提供程序” ，选择“unpkg” 。
（3）对于“库” ，输入 @microsoft/signalr@3，然后选择不是预览版的最新版本
（4）选择“选择特定文件” ，展开“dist/browser” 文件夹，然后选择“signalr.js” 和“signalr.min.js”
（5）将“目标位置” 设置为 wwwroot/js/ ，然后选择“安装”
 */