using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorSurveys.Shared
{
	public class SurveyHttpClient
	{
		private readonly HttpClient _http;

		public SurveyHttpClient(HttpClient http)
		{
			_http = http;
		}

		public async Task<SurveySummary[]> GetSurveys()
		{
			return await _http.GetFromJsonAsync<SurveySummary[]>("api/survey");
		}

		public async Task<Survey> GetSurvey(Guid surveyId)
		{
			return await _http.GetFromJsonAsync<Survey>($"api/survey/{surveyId}");
		}

		public async Task<HttpResponseMessage> AddSurvey(AddSurveyModel survey)
		{
			return await _http.PutAsJsonAsync<AddSurveyModel>("api/survey", survey);
		}

		public async Task AnswerSurvey(Guid surveyId, SurveyAnswer answer)
		{
			await _http.PostAsJsonAsync($"api/survey/{ surveyId}/ answer", answer);

		}
	}
}
