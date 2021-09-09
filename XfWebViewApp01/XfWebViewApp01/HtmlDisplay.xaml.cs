using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XfWebViewApp01
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HtmlDisplay : ContentView
    {
        public static readonly BindableProperty HtmlContentProperty = BindableProperty.Create(nameof(HtmlContent), typeof(string), typeof(HtmlDisplay), propertyChanged: OnHtmlContentChanged);
        public string HtmlContent
        {
            get => GetValue(HtmlContentProperty) as string;
            set => SetValue(HtmlContentProperty, value);
        }

        public static readonly BindableProperty LinkClickedCommandProperty = BindableProperty.Create(nameof(LinkClickedCommand), typeof(ICommand), typeof(HtmlDisplay));
        public ICommand LinkClickedCommand
        {
            get => GetValue(LinkClickedCommandProperty) as ICommand;
            set => SetValue(LinkClickedCommandProperty, value);
        }

        public HtmlDisplay()
        {
            InitializeComponent();

            TheWebView.Navigating += TheWebView_Navigating;
            TheWebView.Navigated += TheWebView_Navigated;
        }

        static void OnHtmlContentChanged(Xamarin.Forms.BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is HtmlDisplay htmlDisplay)
            {
                if (newValue is string html)
                {
                    //set html content
                    htmlDisplay.SetHtmlSource(html);
                }
                else
                {
                    //clear content
                    htmlDisplay.SetHtmlSource(null);
                }
            }
        }

        private void SetHtmlSource(string html)
        {
            var source = new HtmlWebViewSource()
            {
                BaseUrl = "www.baseUrl.org/test/",
                Html = html
            };
            TheWebView.Source = source;
        }

        private void TheWebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            if (e.NavigationEvent != WebNavigationEvent.NewPage)
            {
                //unwanted navigation
                e.Cancel = true;
                return;
            }

            //TODO get link and trigger command
            var link = GetLinkForCurrentPlatform(e);

            if(link is null)
            {
                //unknown link
                e.Cancel = true;
                return;
            }

            TryExecuteLinkClickedCommand(link);
        }

        private string GetLinkForCurrentPlatform(WebNavigatingEventArgs e)
        {
            return Device.RuntimePlatform switch
            {
                Device.UWP => GetLink_UWP(e),
                Device.Android => GetLink_Android(e),
                Device.iOS => GetLink_iOS(e),
                _ => throw new Exception()
            };
        }

        private string GetLink_UWP(WebNavigatingEventArgs e)
        {
            var invalidMatch = Regex.Match(e.Url, @"about:blank.*");
            if (invalidMatch.Success)
            {
                return null;
            }

            var match = Regex.Match(e.Url, @"about:(?<link>.*)");
            if(match.Success is false)
            {
                return null;
            }

            var link = match.Groups["link"].Value;

            return link;
        }

        private string GetLink_Android(WebNavigatingEventArgs e)
        {
            return e.Url;
        }

        private string GetLink_iOS(WebNavigatingEventArgs e)
        {
            return e.Url;
        }

        private void TryExecuteLinkClickedCommand(string link)
        {
            if (LinkClickedCommand.CanExecute(link))
            {
                LinkClickedCommand.Execute(link);
            }
        }

        private void TheWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            //example: maybe call scroll to here
            //await ScrollToAsync("1.1.4.7");
        }

        private async Task ScrollToAsync(string link)
        {
            string script = $"document.getElementById(\"{link}\").scrollIntoView();";
            var result = await TheWebView.EvaluateJavaScriptAsync(script);
        }
    }
}