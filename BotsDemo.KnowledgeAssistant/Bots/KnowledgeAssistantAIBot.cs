using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.KernelMemory;
using Microsoft.Teams.AI.AI.Models;
using Newtonsoft.Json.Linq;

namespace BotsDemo.KnowledgeAssistant.Bots
{
    public class KnowledgeAssistantAIBot(MemoryWebClient _client, IHttpClientFactory _clientFactory) : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var response = string.Empty;
            bool messageWithFileDownloadInfo = turnContext.Activity.Attachments?.FirstOrDefault()?.ContentType == FileDownloadInfo.ContentType;

            if (messageWithFileDownloadInfo)
            {
                var text = await GetFileText(turnContext.Activity.Attachments.First());
                await _client.ImportTextAsync(text);
                response = "The file was uploaded!";
            }
            else
            {
                var answer = await _client.AskAsync(turnContext.Activity.Text);
                response = answer.Result;
            }

            var replyActivity = MessageFactory.Text(response);
            await turnContext.SendActivityAsync(replyActivity, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Welcome to the Simple AI Bot! I can help you with any questions. What would you like to know";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText), cancellationToken);
                }
            }
        }

        private async Task<string> GetFileText(Attachment file)
        {
            var fileDownload = JObject.FromObject(file.Content).ToObject<FileDownloadInfo>();

            using var client = _clientFactory.CreateClient();
            var fileResponse = await client.GetAsync(fileDownload.DownloadUrl);
            return await fileResponse.Content.ReadAsStringAsync();
        }
    }
}
