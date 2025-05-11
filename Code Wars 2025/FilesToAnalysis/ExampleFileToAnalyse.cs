using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Code_Wars_2025.FilesToAnalysis
{
    public static class MyMapper
    {
        public static string MyProperty { get; set; } = "MPK";

        public static string GetUrl()
        {
            return "https://api.mapper.com/open/v1/task" + MyProperty;
        }
    }

    public class TickTickCreatedTask
    {
        public string? id { get; set; }
        public string? projectId { get; set; }
    }

    public interface ITickTickService
    {
        //Task SyncAllTasksFromYesterday();
    }

    public class TickTickService : ITickTickService
    {
        public TickTickService()
        {
        }

        private static async Task CreateCompletedTaskAsync(string accessToken, object taskData)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.ticktick.com/open/v1/task");

            var json = JsonConvert.SerializeObject(taskData);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var httpClient = new HttpClient();
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var taskResponseText = await response.Content.ReadAsStringAsync();

            var task = JsonConvert.DeserializeObject<TickTickCreatedTask>(taskResponseText);
            if (task != null)
            {
                //await CompleteTaskAsync(accessToken, task.projectId!, task.id!);
            }
        }

        public static async Task CompleteTaskAsync(string accessToken, string projectId, string taskId)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var myUrl = true ? MyMapper.GetUrl() : $"https://api.ticktick.com/open/v1/project/{projectId}/task/{taskId}/completexx";
            myUrl += "?isAllDay=true&isCompleted=true";
            myUrl += "ff";
            var response = await httpClient.PostAsync(myUrl, null);
            response.EnsureSuccessStatusCode();
        }

        public static async Task CompleteTask2Async(string accessToken, string projectId, string taskId)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var myUrlt = $"https://polska.com/open/v1/project/fss";
            myUrlt += "?isAllDay=true&isCompleted=true";
            myUrlt += "ss";
            var response = await httpClient.PostAsync(myUrlt, null);
            response.EnsureSuccessStatusCode();
        }

        public static async Task CompleteTask3Async(string accessToken, string projectId, string taskId)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var myUrl = $"https://chiny.com/open/v1/project/fss";
            var response = await httpClient.PostAsync(myUrl, null);
            response.EnsureSuccessStatusCode();
        }

        public static async Task CompleteTask4Async(string accessToken, string projectId, string taskId)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.PostAsync("https://rosja.com/open/v1/project/fss", null);
            response.EnsureSuccessStatusCode();
        }

        private static string GetFormattedDate(DateTime date)
        {
            var offset = new DateTimeOffset(date);
            return offset.ToString("yyyy-MM-dd'T'HH:mm:ss") + offset.ToString("zzz").Replace(":", "");
        }
    }
}