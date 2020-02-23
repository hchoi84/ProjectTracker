using System;

namespace ProjectTracker.Models
{
  public class TaskMember
  {
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string MemberId { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public Task Task { get; set; }
    public Member Member { get; set; }
  }
}