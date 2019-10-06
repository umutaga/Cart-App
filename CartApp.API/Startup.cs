using System.Collections.Generic;
using System.Linq;
using System.Text;
using CartApp.Core.ApplicationService;
using CartApp.Core.ApplicationService.Services;
using CartApp.Core.DomainService;
using CartApp.Infrastructure.Data;
using CartApp.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CartApp.API
{
	public class Startup
	{
		#region Variables
		private IConfiguration _conf { get; }
		private IHostingEnvironment _env { get; }
		protected readonly string _cs = "";
		#endregion

		public Startup(IHostingEnvironment env)
		{
			_env = env;
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			_conf = builder.Build();
			_cs = _conf.GetConnectionString("DefaultConnection");
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			services.AddSingleton<IConfiguration>(_conf);
			services.AddDbContext<CartAppContext>(opt => opt.UseSqlServer(_cs));


			services.AddScoped<ICustomerRepository, CustomerRepository>();
			services.AddScoped<ICustomerService, CustomerService>();

			services.AddScoped<ICartRepository, CartRepository>();
			services.AddScoped<ICartService, CartService>();

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					.AddJwtBearer(jwtBearerOptions =>
					{
						jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
						{
							ValidateActor = true,
							ValidateAudience = true,
							ValidateLifetime = true,
							ValidateIssuerSigningKey = true,
							ValidIssuer = _conf["Issuer"],
							ValidAudience = _conf["Audience"],
							IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf["SigningKey"]))
						};
					});


			services.AddMvc().AddJsonOptions(options =>
			{
				options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				options.SerializerSettings.MaxDepth = 3;
			});



			services.ConfigureSwaggerGen(o =>
				o.OperationFilter<AuthorizationHeaderParameterOperationFilter>()
			);
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "Cart API", Version = "v1" });
			});
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				using (var scope = app.ApplicationServices.CreateScope())
				{
					var ctx = scope.ServiceProvider.GetService<CartAppContext>();
				}
			}
			else
			{
				using (var scope = app.ApplicationServices.CreateScope())
				{
					var ctx = scope.ServiceProvider.GetService<CartAppContext>();
					ctx.Database.EnsureCreated();
				}
				app.UseHsts();
				app.UseHttpsRedirection();
			}

			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cart API v1");
			});

			app.UseAuthentication();
			app.UseMvc();

		}

		public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
		{
			public void Apply(Operation operation, OperationFilterContext context)
			{
				var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
				var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter);
				var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);

				if (isAuthorized && !allowAnonymous)
				{
					if (operation.Parameters == null)
						operation.Parameters = new List<IParameter>();

					operation.Parameters.Add(new NonBodyParameter
					{
						Name = "Authorization",
						In = "header",
						Description = "Bearer {token}",
						Required = true,
						Type = "string"
					});
				}
			}
		}
	}

}
