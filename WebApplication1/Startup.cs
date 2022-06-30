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
            //����SignalR���� ���ÿ���
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
            // ������̬ҳ��
            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);
            //app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot")),
                // ����Linux ʹ��
                /*FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))*/
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            //ʹ�ÿ���			
            app.UseCors("CorsPolicy");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<HubMessage>("/SignalrHub");//signal ����
            });
        }
    }
}
/*signalr web client
 *��1���ڡ����������Դ�������� �У��Ҽ�������Ŀ��Ȼ��ѡ����ӡ� >���ͻ��˿⡱ ��
��2���ڡ���ӿͻ��˿⡱ �Ի����У����ڡ��ṩ���� ��ѡ��unpkg�� ��
��3�����ڡ��⡱ ������ @microsoft/signalr@3��Ȼ��ѡ����Ԥ��������°汾
��4��ѡ��ѡ���ض��ļ��� ��չ����dist/browser�� �ļ��У�Ȼ��ѡ��signalr.js�� �͡�signalr.min.js��
��5������Ŀ��λ�á� ����Ϊ wwwroot/js/ ��Ȼ��ѡ�񡰰�װ��
 */