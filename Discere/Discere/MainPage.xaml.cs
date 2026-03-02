using Discere.Database;
using System.Diagnostics;

namespace Discere
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        Card.Model card;

        public MainPage()
        {
            InitializeComponent();

            this.Appearing += OnAppearing;
        }

        private async void OnAppearing(object? sender, EventArgs e)
        {
            var dbPath = Preferences.Get("DatabasePath", null);

            if (string.IsNullOrEmpty(dbPath) || !File.Exists(dbPath))
            {
#if ANDROID
                await AndroidFilePermissions.ImportDatabaseAsync();
#endif
            }
            var db = new Card.Service();
            Card.Model card = null;

            if (count == 0)
            {
                count = db.GetQuery().Count();
                LabelCardCount.Text = count.ToString();
            }

            int totalWeight = db.GetQuery().Sum(s => s.Difficulty);
            var random = new Random();
            var target = random.Next(1, totalWeight + 1);
            int cumulative = 0;

            foreach (var dbCard in db.GetQuery())
            {
                cumulative += dbCard.Difficulty;

                if (target <= cumulative)
                {
                    card = dbCard;
                    break;
                }
            }

            if (card == null) return;

            card.Question = card.Question.Replace("\n", "\n\n");
            card.Question = card.Question.Replace("\t", "");
            card.Answer = card.Answer.Replace("\n", "\n\n");
            card.Answer = card.Answer.Replace("\t", "");

            this.card = new Card.DTO()
            {
                CardID = card.CardID,
                Number = card.Number,
                Question = card.Question,
                Answer = card.Answer,
                Difficulty = card.Difficulty
            };
            LabelCardNumber.Text = card.Number.ToString();
            LabelQuestion.Text = card.Question;

            switch(card.Difficulty)
            {
                case >120:
                    LabelDifficulty.Text = "Hard"; 
                    break;
                case <80:
                    LabelDifficulty.Text = "Easy"; 
                    break;
                default:
                    LabelDifficulty.Text = "Medium";
                    break;
            }
        }

        private void OnSubmitClicked(object? sender, EventArgs e)
        {
            var card = new Card.DTO()
            {
                CardID = this.card.CardID,
                Number = this.card.Number,
                Question = this.card.Question,
                Answer = this.card.Answer,
                Difficulty = this.card.Difficulty,
                UserAnswer = EditorUserAnswer.Text
            };
            Shell.Current.GoToAsync("//AnswerPage",
                new Dictionary<string, object>
                {
                    ["card"] = card
                });
        }
    }
}
