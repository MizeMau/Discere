using Discere.Database;

namespace Discere
{
    public partial class MainPage : ContentPage
    {
        int count = 1;
        Card card;

        public MainPage()
        {
            InitializeComponent();
            card = new Card()
            {
                CardID = 1,
                Number = 1,
                Question = "Nennen Sie Beispiele Für Protokolle, welche auf der Vermittlungsschicht und der Transportschicht der OSI-Modells arbeiten.",
                Answer = $"Vermittlungsschicht:{Environment.NewLine}-IP{Environment.NewLine}-IPSec{Environment.NewLine}-ICMP{Environment.NewLine}Transportschicht{Environment.NewLine}-TCP{Environment.NewLine}-UDP"
            };
            LabelCardCount.Text = count.ToString();
            LabelCardNumber.Text = card.Number.ToString();
            LabelQuestion.Text = card.Question;
        }

        private void OnSubmitClicked(object? sender, EventArgs e)
        {
            card.UserAnswer = EditorUserAnswer.Text;
            Shell.Current.GoToAsync("//AnswerPage",
                new Dictionary<string, object>
                {
                    ["card"] = card
                });
        }

        private void OnSkipClicked(object? sender, EventArgs e)
        {

        }

        //private void OnCounterClicked(object? sender, EventArgs e)
        //{
        //    count++;

        //    if (count == 1)
        //        CounterBtn.Text = $"Clicked {count} time";
        //    else
        //        CounterBtn.Text = $"Clicked {count} times";

        //    SemanticScreenReader.Announce(CounterBtn.Text);
        //}
    }
}
