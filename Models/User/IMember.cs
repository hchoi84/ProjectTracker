using System.Collections.Generic;

namespace ProjectTracker.Models
{
  public interface IMember
  {
    Member Create(Member member);
    Member GetMember(int id);
    IEnumerable<Member> GetAllMembers();
    Member Update(Member member);
    Member Delete(int id);
  }
}