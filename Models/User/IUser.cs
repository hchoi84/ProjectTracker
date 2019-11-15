using System.Collections.Generic;

namespace ProjectTracker.Models
{
  public interface IUser
  {
    User Create(User user);
    User GetUser(int id);
    IEnumerable<User> GetAllUsers();
    User Update(User user);
    User Delete(int id);
  }
}