using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker;
using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace TeamsBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly string kbId;
        private readonly QnAMakerRuntimeClient qnaClient;
        public EchoBot(IConfiguration config)
        {
            kbId = config["QnAKnowledgeBase"];
            qnaClient = new QnAMakerRuntimeClient(
                new EndpointKeyServiceClientCredentials(
                    config["QnAEndpointKey"]))
            {
                RuntimeEndpoint = $"https://{config["QnAEndpointHost"]}.azurewebsites.net"
            };
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var response = await qnaClient.Runtime.GenerateAnswerAsync(
                kbId,
                new QueryDTO { Question = turnContext.Activity.Text }
            );

            foreach (var answer in response.Answers.Where(x => x.Score > 7.5))
            {
                await turnContext.SendActivityAsync(answer.Answer);
            }
        }
    }
}
