using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Contracts.Interfaces;
using Contracts.Models;

namespace XTestBackEnd
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
            var connectionString = Configuration.GetConnectionString("AzureConnection");
            services.AddControllers();
            services.AddSingleton<IRepository<User>>(new AzureRepository<User>(connectionString, null));
            services.AddSingleton<IRepository<MeetingRecord>>(new AzureRepository<MeetingRecord>(connectionString, null));
            services.AddSingleton<IRepository<MeetingType>>(new AzureRepository<MeetingType>(connectionString, null));
            services.AddSingleton<IRepository<WorkingTime>>(new AzureRepository<WorkingTime>(connectionString, null));
            services.AddSingleton<IRepository<UnplannedDay>>(new AzureRepository<UnplannedDay>(connectionString, null));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
