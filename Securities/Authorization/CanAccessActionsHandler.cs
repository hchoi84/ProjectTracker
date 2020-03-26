using System;
using System.Linq;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using ProjectTracker.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.DataProtection;

namespace ProjectTracker.Securities
{
  public class CanAccessActionsHandler : AuthorizationHandler<CanAccessActionsRequirement>
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _dbContext;
    private readonly IDataProtector _protectProjectId;
    public CanAccessActionsHandler(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext, IDataProtectionProvider dataProtectionProvider, DataProtectionStrings dataProtectionStrings)
    {
      _httpContextAccessor = httpContextAccessor;
      _dbContext = dbContext;
      _protectProjectId = dataProtectionProvider.CreateProtector(dataProtectionStrings.ProjectId);
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanAccessActionsRequirement requirement)
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