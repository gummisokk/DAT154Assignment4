using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Json;
using Teacher_Assessment.Models;

namespace Teacher_Assessment.Services
{
    internal class APIservice
    {
        private readonly HttpClient _httpClient;

        public APIservice(HttpClient httpClient)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:5002/") }; //eller 5001??? Alt etter kva port API-et køyrer på
        }
        public async Task<List<StudentAction>> GetTimeline(string caseId)
        {
            return await _httpClient.GetFromJsonAsync<List<StudentAction>>($"cases/{caseId}/timeline");
        }
        public async Task AddNote(string caseId, TeacherNote note)
        {
            await _httpClient.PostAsJsonAsync($"cases/{caseId}/notes", note);
        }
    }
}
