using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectTracker.Models
{
  public class TestUserRepo : IUser
  {
    private List<User> _user;
    public TestUserRepo()
    {
      _user = new List<User>()
      {
        new User
        {
          Id = 1,
          FirstName = "Howard",
          LastName = "Choi",
          Created = new DateTime(2019, 11, 15),
          Updated = new DateTime(2019, 11, 15),
        },
        new User
        {
          Id = 2,
          FirstName = "Kenny",
          LastName = "Ock",
          Created = new DateTime(2019, 11, 15),
          Updated = new DateTime(2019, 11, 15),
        }
      };
    }

    public User Create(User newUser)
    {
      newUser.Id = _user.Count == 0 ? 1 : _user.Max(u => u.Id) + 1;
      newUser.Created = DateTime.Now;
      newUser.Updated = DateTime.Now;

      _user.Add(newUser);
      return newUser;
    }

    public User Delete(int id)
    {
      User user = _user.FirstOrDefault(u => u.Id == id);
      if (user == null)
        return null;

      _user.Remove(user);
      return user;
    }

    public IEnumerable<User> GetAllUsers()
    {
      return _user;
    }

    public User GetUser(int id)
    {
      User user = _user.FirstOrDefault(u => u.Id == id);
      if (user == null)
        return null;

      return user;
    }

    public User Update(User updateUser)
    {
      User user = _user.FirstOrDefault(u => u.Id == updateUser.Id);
      if (user == null)
        return null;

      user.FirstName = updateUser.FirstName;
      user.LastName = updateUser.LastName;
      user.Updated = DateTime.Now;

      return user;
    }
  }
}