using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Web.WebView2.Core;
using RestSharp;

public class ApiResponse
{
    public string Status { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string SkinUrl { get; set; }
    public string DownloadUrl { get; set; }
}

public static class Authenticator
{
    public static async Task<ApiResponse> CheckLogin(string email, string password)
    {
        var client = new RestClient($"{Definitions.BaseURL}:3551");
        var request = new RestRequest("/api/launcher/login", Method.Post);

        request.AddJsonBody(new
        {
            email = email,
            password = password
        });

        request.AddHeader("X-Launcher-Version", Definitions.CurrentVersion);

        try
        {
            var response = await client.ExecuteAsync(request);

            if (string.IsNullOrWhiteSpace(response.Content))
            {
                return new ApiResponse { Status = "Error" };
            }

            return JsonConvert.DeserializeObject<ApiResponse>(response.Content);
        }
        catch
        {
            return new ApiResponse { Status = "Error" };
        }
    }
}
