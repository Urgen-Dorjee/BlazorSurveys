using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorSurveys.Server.Hubs;
using BlazorSurveys.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BlazorSurveys.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SurveyController : ControllerBase
	{

		private readonly IHubContext<SurveyHub, ISurveyHub> hubContext;
		
		public SurveyController(IHubContext<SurveyHub, ISurveyHub> surveyHub)
		{
			this.hubContext = surveyHub;
		}

		private static ConcurrentBag<Survey> surveys = new()
		{
			// feel free to initialize here some sample surveys like:
			new Survey()
			{
				Id = Guid.Parse("b00c58c0-df00-49ac-ae85-0a135f75e01b"),
				Title = "Are you excited about .NET 5.0?",
				ExpiresAt = DateTime.Now.AddMinutes(10),
				Options = new List<string> { "Yes", "Nope", "meh" },
				Answers = new List<SurveyAnswer>
				{
					new() {Option = "Yes"},
					new() {Option = "Yes"},
					new() {Option = "Yes"},
					new() {Option = "Nope"},
					new() {Option = "meh"}
				}
			}
		};

		[HttpGet]
		public IEnumerable<SurveySummary> GetSurveys()
		{
			return surveys.Select(s => s.ToSummary());
		}

		[HttpGet("{id}")]
		public ActionResult GetSurvey(Guid id)
		{
			var survey = surveys.SingleOrDefault(t => t.Id == id);
			if (survey == null) return NotFound();
			return new JsonResult(survey);
		}

		[HttpPut]
		public async Task<Survey> AddSurvey([FromBody] AddSurveyModel addSurveyModel)
		{
			Survey survey = new();
			if (addSurveyModel.Minutes != null)
			{
				survey = new Survey
				{
					Title = addSurveyModel.Title,
					ExpiresAt = DateTime.Now.AddMinutes(addSurveyModel.Minutes.Value),
					Options = addSurveyModel.Options.Select(o => o.OptionValue).ToList()
				};
				surveys.Add(survey);
			}
			await hubContext.Clients.All.SurveyAdded(survey.ToSummary());
			return survey;
		}

		[HttpPost("{surveyId}/answer")]
		public async Task<ActionResult> AnswerSurvey(Guid surveyId, [FromBody]SurveyAnswer
			answer)
		{
			var survey = surveys.SingleOrDefault(t => t.Id == surveyId);
			if (survey == null) return NotFound();
			// WARNING: this isn’t thread safe since we store answers in a List!
			survey.Answers.Add(new SurveyAnswer
			{
				SurveyId = surveyId,
				Option = answer.Option
			});
			await hubContext.Clients.Group(surveyId.ToString()).SurveyUpdated(survey);
			return new JsonResult(survey);
		}
	}
}
