using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestFuncApp.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RestFuncApp.Function
{
    public static class TaskActivities
    {
        public static readonly List<TaskItem> Items = new List<TaskItem>();
        [FunctionName("TaskCreation")]
        public static async Task<IActionResult> TaskCreation(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "task")] HttpRequest req,
             ILogger log)
        {
            log.LogInformation("Creating a new Task list item");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<TaskCreateModel>(requestBody);

            var task = new TaskItem() { TaskDescription = input.TaskDescription };
            Items.Add(task);
            return new OkObjectResult(task);
        }
        [FunctionName("GetAllTasks")]
        public static IActionResult GetAllTasks(
            [HttpTrigger(AuthorizationLevel.Anonymous,
                "get", Route = "task")]
            HttpRequest req, ILogger log)
        {
            log.LogInformation("Getting Task list items");
            return new OkObjectResult(Items);
        }
        [FunctionName("GetTaskById")]
        public static IActionResult GetTaskById(
           [HttpTrigger(AuthorizationLevel.Anonymous,
                "get", Route = "task/{id}")]
            HttpRequest req, string id)
        {
            var task = Items.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(task);
        }

        [FunctionName("UpdateTask")]
        public static async Task<IActionResult> UpdateTask(
            [HttpTrigger(AuthorizationLevel.Anonymous,
                "put", Route = "task/{id}")]
            HttpRequest req, string id)
        {
            var task = Items.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return new NotFoundResult();
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<TaskUpdateModel>(requestBody);

            task.IsCompleted = updated.IsCompleted;
            if (!string.IsNullOrEmpty(updated.TaskDescription))
            {
                task.TaskDescription = updated.TaskDescription;
            }
            return new OkObjectResult(task);
        }

        [FunctionName("DeleteTask")]
        public static IActionResult DeleteTask(
            [HttpTrigger(AuthorizationLevel.Anonymous,
                "delete", Route = "task/{id}")]
            HttpRequest req, string id)
        {
            var task = Items.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return new NotFoundResult();
            }
            Items.Remove(task);
            return new OkResult();
        }

    }
}
