using MvvmHelpers.Commands;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XfWebViewApp01
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HtmlPage : ContentPage
    {
        public string HtmlCode { get; set; }

        public ICommand HtmlLinkClickedCommand => new AsyncCommand<string>(OnHtmlLinkClicked);

        public HtmlPage()
        {
            HtmlCode = GetHtml();

            InitializeComponent();

            BindingContext = this;
        }

        private string GetHtml()
        {
            var html = Properties.Resources.example;
            return html;
        }

        private async Task OnHtmlLinkClicked(string link)
        {
            await DisplayAlert("Link clicked", link, "OK");
        }
    }
}