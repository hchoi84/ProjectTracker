using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectTracker.Models
{
  public class TestMemberRepo : IMember
  {
    private List<Member> _members;
    public TestMemberRepo()
    {
      _members = new List<Member>()
      {
        new Member
        {
          Id = "1",
          UserName = "howard.choi@email.com",
          Email = "howard.choi@email.com",
          FirstName = "Howard",
          LastName = "Choi",
          Created = new DateTime(2019, 11, 15),
          Updated = new DateTime(2019, 11, 15),
        },
        new Member
        {
          Id = "2",
          UserName = "kenny.ock@email.com",
          Email = "kenny.ock@email.com",
          FirstName = "Kenny",
          LastName = "Ock",
          Created = new DateTime(2019, 11, 15),
          Updated = new DateTime(2019, 11, 15),
        }
      };
    }

    public Member Create(Member newMember)
    {
      newMember.Id = _members.Count == 0 ? "1" : _members.Max(u => u.Id) + 1;
      newMember.Created = DateTime.Now;
      newMember.Updated = DateTime.Now;

      _members.Add(newMember);
      return newMember;
    }

    public Member Delete(int id)
    {
      Member member = _members.FirstOrDefault(u => u.Id == id.ToString());
      if (member == null)
        return null;

      _members.Remove(member);
      return member;
    }

    public IEnumerable<Member> GetAllMembers()
    {
      return _members;
    }

    public Member GetMember(string id)
    {
      Member member = _members.FirstOrDefault(u => u.Id == id.ToString());
      if (member == null)
        return null;

      return member;
    }

    public Member Update(Member updateMember)
    {
      Member member = _members.FirstOrDefault(u => u.Id == updateMember.Id);
      if (member == null)
        return null;

      member.FirstName = updateMember.FirstName;
      member.LastName = updateMember.LastName;
      member.Updated = DateTime.Now;

      return member;
    }
  }
}