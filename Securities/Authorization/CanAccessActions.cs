using System;
using System.Linq;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using ProjectTracker.Models;
using System.Security.Claims;

namespace ProjectTracker.Securities
{
  public class CanAccessActions : AuthorizationHandler<CustomClaims>
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _dbContext;
    public CanAccessActions(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
    {
      _httpContextAccessor = httpContextAccessor;
      _dbContext = dbContext;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomClaims requirement)
    {
      var routeValues = _httpContextAccessor.HttpContext.Request.RouteValues;
      if (routeValues.Count == 0)
      {
        return Task.CompletedTask;
      }
      // var queryValues = _httpContextAccessor.HttpContext.Request.Query;
      // if (queryValues.Count == 0)
      // {
      //   return Task.CompletedTask;
      // }
      
      object projectId;
      routeValues.TryGetValue("projectId", out projectId);
      // StringValues projectId;
      // queryValues.TryGetValue("projectId", out projectId);

      var creatorId = _dbContext.Projects.FirstOrDefault(p => p.Id == Convert.ToInt32(projectId)).MemberId;

      var isCreator = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value == creatorId;

      if (context.User.HasClaim(c => c.Type == "SuperAdmin" && c.Value == "true") ||
          context.User.HasClaim(c => c.Type == "Admin" && c.Value == "true") ||
          isCreator)
      {
        context.Succeed(requirement);
      }

      return Task.CompletedTask;
    }
  }
}