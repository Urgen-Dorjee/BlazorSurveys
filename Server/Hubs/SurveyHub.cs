using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

//ReSharper disable all
namespace BlazorSurveys.Server.Hubs
{
	public class SurveyHub : Hub<ISurveyHub>
	{
		public async Task JoinSurveyGroup(Guid surveyId)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, surveyId.ToString());
		}
		public async Task LeaveSurveyGroup(Guid surveyId)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, surveyId.ToString());
		}
	}
}
