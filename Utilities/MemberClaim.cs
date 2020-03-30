using System;
using System.Collections.Generic;

namespace ProjectTracker.Utilities
{
  public class MemberClaim
  {
    public string ClaimType { get; set; }
    public bool IsSelected { get; set; }
    public string Description { get; set; }

    public string GetDescription(string claimType)
    {
      Dictionary<string, string> description = new Dictionary<string, string>()
      {
        {"SuperAdmin", "Admin priviledges + Control Member Permission"},
        {"Admin", "Create, Read, Update, and Delete all Projects & Tasks"},
        {"Manager", "Create, Read, Update, and Delete Projects & Tasks the Member is part of"},
        {"Member", "Create, Read, Update, and Delete the Member created"},
      };

      return description[claimType];
    }
  }
}