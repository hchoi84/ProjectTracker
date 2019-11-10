using System;

namespace ProjectTracker.Models
{
  public class TaskStatus
  {
    public int Id { get; set; }
    public int OrderPriority { get; set; }
    public string StatusName { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
  }

  public enum TaskStatusEnum{};
}