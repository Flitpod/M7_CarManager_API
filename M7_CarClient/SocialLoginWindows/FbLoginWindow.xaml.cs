using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfSocialLogin
{
    /// <summary>
    /// Interaction logic for FbLogin.xaml
    /// </summary>
    public partial class FbLoginWindow : Window
    {
        public string? AccessToken { get; set; }
        public DateTime? TokenExpires { get; set; }
        public FbLoginWindow()
        {
            InitializeComponent();
            string returnURL = WebUtility.UrlEncode("https://www.facebook.com/connect/login_success.html");
            string scopes = "email";
            File.WriteAllText("fblink.txt", string.Format("https://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}&response_type=token%2Cgranted_scopes&scope={2}&display=popup", new object[] { Config.FbAppId, returnURL, scopes }));
            wbrowser.Navigate(new Uri(string.Format("https://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}&response_type=token%2Cgranted_scopes&scope={2}&display=popup", new object[] { Config.FbAppId, returnURL, scopes })));
        }

        private void wbrowser_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Uri.AbsolutePath == "/connect/login_success.html")
            {
                if (e.Uri.Query.Contains("error"))
                {
                    ExtractURLInfo("?", e.Uri.Query);
                    this.DialogResult = false;
                }
                else
                {
                    ExtractURLInfo("#", e.Uri.Fragment);
                    this.DialogResult = true;
                }
                this.Close();
            }
        }

        private void ExtractURLInfo(string inpTrimChar, string urlInfo)
        {
            string fragments = urlInfo.Trim(char.Parse(inpTrimChar)); // Trim the hash or the ? mark
            string[] parameters = fragments.Split(char.Parse("&")); // Split the url fragments / query string 

            foreach (string parameter in parameters)
            {
                string[] name_value = parameter.Split(char.Parse("=")); // Split the input

                switch (name_value[0])
                {
                    case "access_token":
                        this.AccessToken = name_value[1];
                        break;
                    case "expires_in":
                        double expires = 0;
                        if (double.TryParse(name_value[1], out expires))
                        {
                            this.TokenExpires = DateTime.Now.AddSeconds(expires);
                        }
                        else
                        {
                            this.TokenExpires = DateTime.Now;
                        }
                        break;
                }
            }
        }
    }
}
