using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FacebookGraph
{
    /// <summary>
    /// Interaction logic for FBAuth.xaml
    /// </summary>
    public partial class FBAuth : Window
    {
       public string AccessToken
        {
            set;
            get;
        }

        public string ExpiresIn
        {
            set;
            get;
        }
        string navigationUrl = "";
        public FBAuth(string clientID, string redirectUrl)
        {
            InitializeComponent();

            navigationUrl = String.Concat(String.Format(String.Concat("https://www.facebook.com/v2.11/dialog/oauth?",
                                                "client_id={0}&redirect_uri={1}&state=0&response_type=token"), clientID, redirectUrl)
                                                );
            browser.Navigate(new Uri("https://www.facebook.com").AbsoluteUri);
        }

        private void browser_Navigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
        {
            this.addressTextBox.Text = e.Url.ToString();

            if (this.addressTextBox.Text.StartsWith("https://www.facebook.com/connect/login_success.html"))
            {
                string queryString = e.Url.Fragment;
                string[] parameters = queryString.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string parameter in parameters)
                {
                    List<string> parameterList = parameter.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    if (parameterList.ElementAt(0).Equals("#access_token"))
                    {
                        AccessToken = parameterList.ElementAt(1);
                    }
                    else if (parameterList.ElementAt(0).Equals("expires_in"))
                    {
                        ExpiresIn = parameterList.ElementAt(1);
                    }
                }

               this.Close();
            }
        }

        private void authbtn_Click(object sender, RoutedEventArgs e)
        {
            

            browser.Navigate(new Uri(navigationUrl).AbsoluteUri);
        }
    }
}
