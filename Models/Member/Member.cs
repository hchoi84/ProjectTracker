using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProjectTracker.Models
{
  public class Member : IdentityUser
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    [NotMapped]
    public string EncryptedId { get; set; }

    public string GetFullName 
    { 
      get {return $"{FirstName} {LastName}"; }
    }
  }
}