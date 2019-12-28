using System.Collections.Generic;
using ProjectTracker.Models;

namespace ProjectTracker.ViewModels
{
  public class HomeIndexViewModel
  {
    public HomeIndexViewModel()
    {
      List<Member> Members = new List<Member>();
    }
    
    public int ProjectsCount { get; set; }
    public int TasksCount { get; set; }
    public List<Member> Members { get; set; }
    public List<string> MemberIds { get; set; }
  }
}