using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ProjectTracker.Models;
using System.Linq;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.DataProtection;

namespace ProjectTracker.Securities
{
  public class CanAccessTasksHandler : AuthorizationHandler<CanAccessTasksRequirement>
  {
    // Checks to see if the user is the creator or is part of ProjectMember
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _dbContext;
    private readonly IDataProtector _protectProjectId;
    public CanAccessTasksHandler(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext, IDataProtectionProvider dataProtectionProvider, DataProtectionStrings dataProtectionStrings)
    {
      _httpContextAccessor = httpContextAccessor;
      _dbContext = dbContext;
      _protectProjectId = dataProtectionProvider.CreateProtector(dataProtectionStrings.ProjectId);
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
      
      int projId = Convert.ToInt32(_protectProjectId.Unprotect(projectId.ToString()));
      var memberId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

      var isCreator = _dbContext.Projects.FirstOrDefault(p => p.Id == projId).MemberId == memberId;
      var isInProjectMember = _dbContext.ProjectMembers.Any(pm => 
        pm.ProjectId == projId && 
        pm.MemberId == memberId);
      var isSuperAdmin = context.User.HasClaim(c => c.Type == "SuperAdmin" && c.Value == "true");

      if (isCreator || isInProjectMember || isSuperAdmin)
      {
        context.Succeed(requirement);
      }
      
      return Task.CompletedTask;
    }
  }
}