using Discere.Database;
using Discere.Service;
using System.Diagnostics;

namespace Discere;

[QueryProperty("card", "card")]
public partial class AnswerPage : ContentPage, IQueryAttributable
{
    Card.DTO card;
    public AnswerPage()
	{
		InitializeComponent();
    }
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        card = (query["card"] as Card.DTO)!;
        LabelQuestion.Text = card.Question;
        LabelAnswer.Text = card.Answer;
        LabelUserAnswer.Text = card.UserAnswer;
    }
    public void OnFalseClick(object? sender, EventArgs e)
    {
        var db = new Card.Service();
        db.UpdateProperty(card.CardID, u => u.Difficulty, card.Difficulty + 10);
        GoHome();
    }
    public void OnCorrectClick(object? sender, EventArgs e)
    {
        var db = new Card.Service();
        db.UpdateProperty(card.CardID, u => u.Difficulty, card.Difficulty - 10);
        GoHome();
    }

    private void OnRunLocalAIEvaluationClick(object? sender, EventArgs e)
    {
        ButtonGenerateAI.IsVisible = false;
        AILoader.IsVisible = true;
        AILoader.IsRunning = true;
        _ = Task.Run(RunLocalAIEvaluation);
    }

    private async void RunLocalAIEvaluation()
    {
        string result = await AIService.EvaluateAsync(
            card.Question, 
            card.Answer,
            card.UserAnswer
        );
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LabelAIComment.Text = result;
            AILoader.IsVisible = false;
            AILoader.IsRunning = false;
            AICommentSection.IsVisible = true;
        });
    }

    private void GoHome()
    {
        Shell.Current.GoToAsync("//MainPage");
    }
}