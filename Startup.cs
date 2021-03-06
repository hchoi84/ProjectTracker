﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using ProjectTracker.Securities;

namespace ProjectTracker
{
  public class Startup
  {
    private readonly IConfiguration _config;
    public Startup(IConfiguration config)
    {
      _config = config;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc(options => {
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        options.Filters.Add(new AuthorizeFilter(policy));
      });

      services.AddIdentity<Member, IdentityRole>(options => {
        options.Password.RequiredLength = 10;
        options.Password.RequiredUniqueChars = 3;

        options.SignIn.RequireConfirmedEmail = true;
      })
      .AddEntityFrameworkStores<AppDbContext>()
      .AddDefaultTokenProviders();

      services.AddScoped<IProject, SqlProjectRepo>();
      services.AddScoped<ITask, SqlTaskRepo>();
      services.AddScoped<ITaskStatus, SqlTaskStatusRepo>();
      services.AddScoped<IMember, SqlMemberRepo>();
      services.AddScoped<IProjectMember, SqlProjectMemberRepo>();
      services.AddScoped<ITaskMember, SqlTaskMemberRepo>();
      services.AddTransient<IAuthorizationHandler, CanAccessActionsHandler>();
      services.AddTransient<IAuthorizationHandler, CanAccessTasksHandler>();
      services.AddSingleton<DataProtectionStrings>();

      services.AddDbContextPool<AppDbContext>(options => options.UseMySql(_config.GetConnectionString("DbConnection")));

      services.AddSession();

      services.ConfigureApplicationCookie(options =>
      {
        options.AccessDeniedPath = new PathString("/AccessDenied");
      });

      services.AddAuthorization(options =>
      {
        options.AddPolicy("SuperAdmin", policy => policy.RequireClaim("SuperAdmin", "true"));
        
        options.AddPolicy("Admin", policy => policy.RequireClaim("Admin", "true"));

        options.AddPolicy("CanAccessActions", policy => policy.AddRequirements(new CanAccessActionsRequirement()));
        options.AddPolicy("CanAccessTasks", policy => policy.AddRequirements(new CanAccessTasksRequirement()));
      });

      services.AddAuthentication()
        .AddGoogle(options => {
          options.ClientId = GoogleAPI.ClientId;
          options.ClientSecret = GoogleAPI.ClientSecret;
      });
    }
    
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        app.UseStatusCodePagesWithReExecute("/Error/{0}");
      }

      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseSession();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{Id?}");
      });
    }
  }
}
