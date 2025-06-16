using CommunityToolkit.Maui.Views;
using MauiApp1.Views;

namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(Views.VocabularyPage), typeof(Views.VocabularyPage));

            Routing.RegisterRoute(nameof(Views.QuizPage), typeof(Views.QuizPage));

            Routing.RegisterRoute(nameof(QuizSelectionPage), typeof(QuizSelectionPage));

            Routing.RegisterRoute(nameof(MistakeReviewPage), typeof(MistakeReviewPage));

        }

        private void Setting_Clicked(object sender, EventArgs e)
        {
            Current.CurrentPage.ShowPopup(new Settings());
        }

    }
}
