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
            _ = ai.Init();
        }
    }
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        card = (query["card"] as Card)!;
        LabelQuestion.Text = card.Question;
        LabelAnswer.Text = card.Answer;
        LabelUserAnswer.Text = card.UserAnswer;

        LabelAIComment.Text = "Bewerte Antwort...";

        // run AI evaluation in background
        var result = ai.EvaluateAsync(
            card.Question,
            card.Answer,
            card.UserAnswer
        );

        LabelAIComment.Text = result;
    }
}