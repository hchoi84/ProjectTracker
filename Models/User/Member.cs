using System;

namespace ProjectTracker.Models
{
  public class Member
  {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    public string GetFullName 
    { 
      get {return $"{FirstName} {LastName}"; }
    }
  }
}