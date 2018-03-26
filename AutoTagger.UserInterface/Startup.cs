namespace AutoTagger.UserInterface
{
    using System;

    using AutoTagger.Clarifai.Standard;
    using AutoTagger.Contract;
    using AutoTagger.Database.Standard;
    using AutoTagger.UserInterface.Controllers;
    using AutoTagger.UserInterface.Controllers.FIlter;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    
    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }



            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(
                c =>
                    {
                    });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });

            app.UseMvc();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddTransient<IAutoTaggerDatabase, AutoTaggerDatabase>();
            services.AddTransient<ITaggingProvider, ClarifaiImageTagger>();

            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "Auto Tagger", Version = "v1" });
                    c.OperationFilter<FileOperation>();
                });
        }
    }
}
