using System;
using kafka_poc.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace kafka_poc
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
            services.AddControllers();

            services.AddSingleton(new DatabaseConfig { Name = Configuration["DatabaseName"] });
            services.AddSingleton<DatabaseBootstrap.IDatabaseBootstrap, DatabaseBootstrap>();
            services.AddSingleton<PreferenceRetriever.IGetPreferencesById, PreferenceRetriever>();
            services.AddSingleton<PreferenceLister.IListPreferences, PreferenceLister>();
            services.AddSingleton<PreferenceCreator.ICreatePreferences, PreferenceCreator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
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

            serviceProvider.GetService<DatabaseBootstrap.IDatabaseBootstrap>().Setup();
        }
    }
}