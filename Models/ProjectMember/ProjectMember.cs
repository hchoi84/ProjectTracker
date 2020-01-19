using System;

namespace ProjectTracker.Models
{
  public class ProjectMember
  {
    public int ProjectId { get; set; }
    public string MemberId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Project Project { get; set; }
    public Member Member { get; set; }
  }
}