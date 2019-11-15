using System;

namespace ProjectTracker.Models
{
  public class User
  {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    public String GetFullName 
    { 
      get {return $"{FirstName} {LastName}"; }
    }
  }
}