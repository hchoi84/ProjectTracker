using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ProjectTracker.Models;
using System.Linq;
using System;
using System.Security.Claims;

namespace ProjectTracker.Securities
{
  public class CanAccessTasksHandler : AuthorizationHandler<CanAccessTasksRequirement>
  {
    // Checks to see if the user is the creator or is part of ProjectMember
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _dbContext;
    public CanAccessTasksHandler(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
    {
      _httpContextAccessor = httpContextAccessor;
      _dbContext = dbContext;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanAccessTasksRequirement requirement)
    {
      var routeValues = _httpContextAccessor.HttpContext.Request.RouteValues;
      if (routeValues.Count == 0)
      {
        return Task.CompletedTask;
      }

      object projectId;
      routeValues.TryGetValue("projectId", out projectId);
      if (projectId == null)
      {
        return Task.CompletedTask;
      }

      var memberId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

      var isCreator = _dbContext.Projects.FirstOrDefault(p => p.Id == Convert.ToInt32(projectId)).MemberId == memberId;
      var isInProjectMember = _dbContext.ProjectMembers.Any(pm => 
        pm.ProjectId == Convert.ToInt32(projectId) && 
        pm.MemberId == memberId);

      if (isCreator || isInProjectMember)
      {
        context.Succeed(requirement);
      }
      
      return Task.CompletedTask;
    }
  }
}