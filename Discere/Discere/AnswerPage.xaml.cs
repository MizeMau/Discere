using Discere.Database;
using Discere.Service;
using System.Diagnostics;

namespace Discere;

[QueryProperty("card", "card")]
public partial class AnswerPage : ContentPage, IQueryAttributable
{
    Card card;
    AIService ai;
    public AnswerPage()
	{
		InitializeComponent();

        if (ai == null)
        {
            ai = new AIService();
        }
    }
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        card = (query["card"] as Card)!;
        LabelQuestion.Text = card.Question;
        LabelAnswer.Text = card.Answer;
        LabelUserAnswer.Text = card.UserAnswer;

        _ = Task.Run(RunLocalInference);
    }

    async void RunLocalInference()
    {
        LabelAIComment.Text = "Bewerte Antwort...";
        string result = await ai.EvaluateAsync(
            card.Question,
            card.Answer,
            card.UserAnswer
        );
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LabelAIComment.Text = result;
            System.Diagnostics.Debug.WriteLine(result);
        });
    }
}