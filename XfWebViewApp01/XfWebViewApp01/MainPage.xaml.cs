using MvvmHelpers.Commands;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace XfWebViewApp01
{
    public partial class MainPage : ContentPage
    {
        public ICommand OpenHtmlPageCommand => new AsyncCommand(OpenHtmlPageAsync);

        public MainPage()
        {
            InitializeComponent();

            this.BindingContext = this;
        }

        private async Task OpenHtmlPageAsync()
        {
            await Navigation.PushModalAsync(new HtmlPage());
        }
    }
}
