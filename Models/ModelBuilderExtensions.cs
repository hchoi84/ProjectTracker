using System;
using Microsoft.EntityFrameworkCore;

namespace ProjectTracker.Models
{
  public static class ModelBuilderExtensions
  {
    public static void Seed(this ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Project>().HasData(
        new Project() 
        {
          Id = 1,
          MemberId = "1",
          ProjectName = "Project Tracker",
          Created = new DateTime(2019, 10, 30, 12, 03, 00),
          Updated = new DateTime(2019, 10, 31, 2, 40, 00),
          Deadline = DateTime.Now.AddMonths(1),
          Summary = "Lorem ipsum dolor, sit amet consectetur adipisicing elit. Quisquam doloremque eaque temporibus obcaecati, aut reprehenderit repellat reiciendis deserunt. Quas nulla corporis et cum eaque, tempora voluptatum pariatur blanditiis iusto expedita. Lorem ipsum dolor, sit amet consectetur adipisicing elit. Explicabo placeat nulla blanditiis dolores nesciunt quaerat quos asperiores eaque possimus eum."
        },
        new Project() 
        {
          Id = 2,
          MemberId = "1",
          ProjectName = "Golfio ChannelAdvisor API",
          Created = new DateTime(2019, 11, 06, 11, 30, 00),
          Updated = new DateTime(2019, 11, 06, 11, 30, 00),
          Deadline = DateTime.Now.AddMonths(1),
          Summary = "Allow data pulling from ChannelAdvisor and report generating without needing to login or do other manual work. Purpose is to increase productivity in other areas and reduce manual report creation."
        }
      );

      modelBuilder.Entity<Task>().HasData(
        new Task()
        {
          Id = 1,
          ProjectId = 1,
          StatusId = 1,
          MemberId = "1",
          TaskName = "Implement Task Feature",
          Description = "Implement CRUD operation for Project tasks",
          Created = new DateTime(2019, 11, 06, 13, 34, 00),
          Updated = new DateTime(2019, 11, 06, 13, 34, 00),
          Deadline = new DateTime(2019, 11, 06, 13, 34, 00).AddMonths(1),
        },
        new Task()
        {
          Id = 2,
          ProjectId = 1,
          StatusId = 1,
          MemberId = "1",
          TaskName = "Implement SQL",
          Description = "Transition all data to utilize SQL",
          Created = new DateTime(2019, 11, 06, 13, 34, 00),
          Updated = new DateTime(2019, 11, 06, 13, 34, 00),
          Deadline = new DateTime(2019, 11, 06, 13, 34, 00).AddMonths(1),
        },
        new Task()
        {
          Id = 3,
          ProjectId = 1,
          StatusId = 1,
          MemberId = "1",
          TaskName = "Design touch-up on Project view",
          Description = "Project title and description to go on top. Rest can stay the same for now",
          Created = new DateTime(2019, 11, 06, 13, 34, 00),
          Updated = new DateTime(2019, 11, 06, 13, 34, 00),
          Deadline = new DateTime(2019, 11, 06, 13, 34, 00).AddMonths(1),
        }
      );

      modelBuilder.Entity<TaskStatus>().HasData(
        new TaskStatus
        {
          Id = 1,
          OrderPriority = 20,
          StatusName = "Pending",
          IsDefault = true,
          Created = new DateTime(2019, 11, 09),
          Updated = new DateTime(2019, 11, 09)
        },
        new TaskStatus
        {
          Id = 2,
          OrderPriority = 40,
          StatusName = "Assigned",
          IsDefault = false,
          Created = new DateTime(2019, 11, 09),
          Updated = new DateTime(2019, 11, 09)
        },
        new TaskStatus
        {
          Id = 3,
          OrderPriority = 60,
          StatusName = "Completed",
          IsDefault = false,
          Created = new DateTime(2019, 11, 09),
          Updated = new DateTime(2019, 11, 09)
        }
      );

      modelBuilder.Entity<Member>().HasData(
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
      );
    }
  }
}