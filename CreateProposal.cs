using AdaptiveCards.Templating;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QnABot
{
    public class UserProfile
    {

        public string name { get; set; }
        public string email { get; set; }

    }

    public class UserProfileDialog : ComponentDialog
    {
        private const string InitialId = nameof(UserProfileDialog);

        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;

        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
             return await base.OnBeginDialogAsync(innerDc, options, cancellationToken);
        }

        public UserProfileDialog(UserState userState)
            : base(nameof(UserProfileDialog))
        {
            _userProfileAccessor = userState.CreateProperty<UserProfile>("UserProfile");

            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                RiskProfileStepAsync,
                AmountStepAsync,
                LastStepAsync
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), MinAmtValidatorAsync));
            //AddDialog(new TextPrompt(nameof(TextPrompt)));
            //AddDialog(new TextPrompt(nameof(TextPrompt)));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> RiskProfileStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            List<string> RiskProfiles = new List<string>()
                {
                    "Wealth Conservation",
                    "Income",
                    "Balanced Growth",
                    "Market Growth",
                    "Opportunistic Growth"
                };
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the user's response is received.
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Please select a risk profile"),
                    Choices = ChoiceFactory.ToChoices(RiskProfiles),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> AmountStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["riskprofile"] = ((FoundChoice)stepContext.Result).Value;

       
                return await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions { Prompt = MessageFactory.Text("How much are you looking to invest in a Model Portfolio?") },
                cancellationToken);
        }

        private async Task<DialogTurnResult> LastStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["mininvamt"] = (int)stepContext.Result;


            List<Investment> investments = FilterModels(stepContext);

            if (!(investments!= null && investments.Count > 0))
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"I did not get results for this search criteria."), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }


            // We can send messages to the user at any point in the WaterfallStep.
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Models with " + stepContext.Values["riskprofile"] + " risk profile and investment amount: " + stepContext.Values["mininvamt"]), cancellationToken);

            ItemClass itemobj = new ItemClass();
            itemobj.items = investments;

           
            
            // Create a Template instance from the template payload
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(File.ReadAllText(Path.Combine(".", "Cards", "ModelList.json")));

            // "Expand" the template - this generates the final Adaptive Card payload
            string cardJson = template.Expand(itemobj);

            var card2CardFin = UpdateAdaptivecardAttachment(cardJson);
            var resp = MessageFactory.Attachment(card2CardFin, ssml: "Models List");


            
            await stepContext.Context.SendActivityAsync(resp, cancellationToken); 
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            //return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter email address.") },
            //cancellationToken);

            /*var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    Attachments = new List<Attachment>() { card2CardFin },
                    Type = ActivityTypes.Message
                     // You can comment this out if you don't want to display any text. Still works.
                }
            };*/
            //  await stepContext.PromptAsync(nameof(TextPrompt), opts);
            // return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            // return await stepContext.Context.SendActivityAsync(resp, cancellationToken);
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter the email address.") }, cancellationToken);


        }

        

        private async Task<DialogTurnResult> HandleResponseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Do something with step.result
            // Adaptive Card submissions are objects, so you likely need to JObject.Parse(step.result)

            //await stepContext.Context.SendActivityAsync($"INPUT: {stepContext.Result}");

            var pdfPath = "http://walleadvisor.azurewebsites.net/File/investment_proposal.pdf";
            Attachment attachment = new Attachment();
            attachment.ContentType = "application/pdf";
            attachment.ContentUrl = pdfPath;
            attachment.Name ="Here is Investment proposal based on selected preference. A copy has been mailed to the client.";


            var resp = MessageFactory.Attachment(attachment, ssml: "Investment Proposal");
            await stepContext.Context.SendActivityAsync(resp, cancellationToken);


            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        

        private static Task<bool> MinAmtValidatorAsync(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            int val;
            // This condition is our validation rule. You can also change the value at this point.
            return Task.FromResult(promptContext.Recognized.Succeeded && promptContext.Recognized.Value > 0);
        }

        private static Attachment UpdateAdaptivecardAttachment(string updateAttch)
        {
            var adaptiveCardAttch = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(updateAttch),
            };
            return adaptiveCardAttch;
        }

        private List<Investment> FilterModels(WaterfallStepContext stepContext)
        {
            var json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/walle.json");
            ItemClass itemobj = JsonConvert.DeserializeObject<ItemClass>(json);
            List<Investment> items = itemobj.items;
            List<Investment> filterdItems;

            float mininvamount = float.Parse(stepContext.Values["mininvamt"].ToString());
            string riskProfile = stepContext.Values["riskprofile"].ToString();

            filterdItems = items.Where(item => (item.name.ToLower().Contains(riskProfile.ToLower()) && float.Parse(item.minInvAmt) >= mininvamount)).ToList();
            return (filterdItems);
        }


    }
}
