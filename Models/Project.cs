using System;

namespace ProjectTracker.Models
{
  public class Project
  {
    public int Id { get; set; }
    public string ProjectName { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public DateTime Deadline { get; set; }
    public string Creator { get; set; }

    public String GetRemaining()
    {
      var diff = this.Deadline.Subtract(DateTime.Now);
      return (Convert.ToInt32(diff.TotalDays).ToString() + " Day(s)");
    }
  }
}