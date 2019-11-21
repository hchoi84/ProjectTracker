// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

// namespace ProjectTracker.Models
// {
//   public class TestTaskStatusRepo : ITaskStatus
//   {
//     private List<TaskStatus> _taskStatus;

//     public TestTaskStatusRepo()
//     {
//       _taskStatus = new List<TaskStatus>()
//       {
//         new TaskStatus
//         {
//           Id = 1,
//           OrderPriority = 20,
//           StatusName = "Pending",
//           IsDefault = true,
//           Created = new DateTime(2019, 11, 09),
//           Updated = new DateTime(2019, 11, 09)
//         },
//         new TaskStatus
//         {
//           Id = 2,
//           OrderPriority = 40,
//           StatusName = "Assigned",
//           IsDefault = false,
//           Created = new DateTime(2019, 11, 09),
//           Updated = new DateTime(2019, 11, 09)
//         },
//         new TaskStatus
//         {
//           Id = 3,
//           OrderPriority = 60,
//           StatusName = "Completed",
//           IsDefault = false,
//           Created = new DateTime(2019, 11, 09),
//           Updated = new DateTime(2019, 11, 09)
//         }
//       };
//     }

//     public async Task<TaskStatus> AddAsync(TaskStatus taskStatus)
//     {
//       taskStatus.Id = _taskStatus.Max(ts => ts.Id) + 1;
//       if (taskStatus.OrderPriority == 0)
//       {
//         taskStatus.OrderPriority = _taskStatus.Max(ts => ts.OrderPriority) + 20;
//       }

//       // Singleton on IsDefault Task Status
//       if (taskStatus.IsDefault)
//       {
//         _taskStatus.FirstOrDefault(ts => ts.IsDefault).IsDefault = false;
//       }
//       taskStatus.Created = DateTime.Now;
//       taskStatus.Updated = DateTime.Now;
//       _taskStatus.Add(taskStatus);
//       return taskStatus;
//     }

//     public async Task<TaskStatus> DeleteAsync(int id)
//     {
//       TaskStatus taskStatusToDelete = _taskStatus.FirstOrDefault(ts => ts.Id == id);
//       if (taskStatusToDelete != null)
//       {
//         _taskStatus.Remove(taskStatusToDelete);
//         return taskStatusToDelete;
//       }
//       return null;
//     }

//     public async Task<List<TaskStatus>> GetAllTaskStatusAsync()
//     {
//       return _taskStatus;
//     }

//     public async Task<TaskStatus> GetTaskStatusAsync(int id)
//     {
//       return _taskStatus.FirstOrDefault(ts => ts.Id == id);
//     }

//     public async Task<TaskStatus> UpdateAsync(TaskStatus taskStatus)
//     {
//       TaskStatus taskStatusToUpdate = _taskStatus.FirstOrDefault(ts => ts.Id == taskStatus.Id);
//       if (taskStatusToUpdate != null)
//       {
//         taskStatusToUpdate.OrderPriority = taskStatus.OrderPriority;
//         taskStatusToUpdate.StatusName = taskStatus.StatusName;
//         if (taskStatusToUpdate.IsDefault)
//         {
//           _taskStatus.FirstOrDefault(ts => ts.IsDefault).IsDefault = false;
//           taskStatusToUpdate.IsDefault = taskStatus.IsDefault;
//         }
//         taskStatusToUpdate.Updated = DateTime.Now;
//         return taskStatus;
//       }
//       return null;
//     }

//     public async Task<int> GetDefaultTaskStatus()
//     {
//       return _taskStatus.FirstOrDefault(ts => ts.IsDefault).Id;
//     }
//   }
// }