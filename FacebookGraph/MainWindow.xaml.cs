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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Facebook;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;
using System.IO;

namespace FacebookGraph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        FacebookClient fb = new FacebookClient();
        ObservableCollection<PageInfo> pages = new ObservableCollection<PageInfo>();
        string searchword = "";
        int pagelimit = 1000;
        Thread th = null;

        string accesst = "";
       

        private void searchEngine(object obj)
        {
            try
            {
                getPages();
            }
            catch (Exception ex)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
                {
                    searchbtn.IsEnabled = true;
                    total_progressbar.Visibility = Visibility.Hidden;
                    MessageBox.Show("Search Stopped."+ex.Message);

                });
            }
            
        }
        public MainWindow()
        {
            InitializeComponent();
            pagelistview.ItemsSource = pages;
        }

        //Get the access token for this request
        public void getAccessToken()
        {
            dynamic result = fb.Get("oauth/access_token", new
            {
                client_id = "549750338734965",
                client_secret = "bdca278e624dc87aabf7f37bc874add2",
                grant_type = "client_credentials"
            });
            //fb.AccessToken = result.access_token;
            fb.AccessToken = accesst;
        }

        public  void getPages()
        {
            pages = new ObservableCollection<PageInfo>();
            getAccessToken();// get the access token of the app
            //
            JObject o =JObject.Parse(fb.Get("search", new
            {
                q = searchword,
                type = "page",
                limit=pagelimit.ToString()
               
            }).ToString());

            JArray pagesIdArray=JArray.Parse(o["data"].ToString());
            bool fd = false;
            bool td = false;
            string fromdt = "";
            string todt = "";
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
                {
                    total_progressbar.Value = 0;
                    total_progressbar.Visibility = Visibility.Visible;
                    fd = fromdate.SelectedDate.HasValue;
                    td = todate.SelectedDate.HasValue;
                    fromdt = fromdate.SelectedDate.Value.ToLongDateString();
                    todt = todate.SelectedDate.Value.ToLongDateString();
                });
            for (int i = 0; i < pagesIdArray.Count; i++)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
                {
                   
                    total_progressbar.Value = ((double)i / (double)pagesIdArray.Count) * 100.0;

                });
                try
                {

                    if (fd && td)
                    {
                        //search page for date range of post if available
                        JObject ob = JObject.Parse(fb.Get(pagesIdArray[i]["id"].ToString() + "/posts", new
                        {
                            limit = "1",
                            until = todt,
                            since = fromdt

                        }).ToString());
                        JArray tempArray = JArray.Parse(ob["data"].ToString());
                        if (tempArray.Count > 0)
                        {
                            //The page exist and has a post during the time range

                            //search for the page with the id
                            JObject pageJO = JObject.Parse(fb.Get(pagesIdArray[i]["id"].ToString(), new
                            {
                                fields = "category,about,emails,username,name,website,location,phone,link,founded",

                            }).ToString());

                            //parse the result into the pageinfo object and then to the collection
                            PageInfo pinfo = new PageInfo()
                            {
                                Name = (pageJO["username"] != null) ? pageJO["username"].ToString() : "",
                                NameBusiness = (pageJO["name"] != null) ? pageJO["name"].ToString() : "",
                                About = (pageJO["about"] != null) ? pageJO["about"].ToString().Replace(",", " ") : "",
                                Phone = (pageJO["phone"] != null) ? pageJO["phone"].ToString() : "",
                                Email = (pageJO["emails"] != null) ? pageJO["emails"][0].ToString() : "",
                                Website = (pageJO["website"] != null) ? pageJO["website"].ToString() : "",
                                FacebookPage = (pageJO["link"] != null) ? pageJO["link"].ToString() : "",
                                Catergory = (pageJO["category"] != null) ? pageJO["category"].ToString() : "",
                            };

                            JObject pJo = (pageJO["location"] != null) ? JObject.Parse(pageJO["location"].ToString()) : null;
                            if (pJo != null)
                            {
                                pinfo.Location = (pJo["street"] != null) ? pJo["street"].ToString().Replace(",", " ") : "";
                                pinfo.Location += (pJo["country"] != null) ? pJo["country"].ToString() : "";
                            }

                            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
                            {
                                pages.Add(pinfo);
                                pagelistview.ItemsSource = pages;
                                searchbtn.IsEnabled = true;
                                noOfCursor.IsEnabled = true;
                                

                            });
                        }
                    }
                    else
                    {
                        //search for the page with the id
                        JObject pageJO = JObject.Parse(fb.Get(pagesIdArray[i]["id"].ToString(), new
                        {
                            fields = "category,about,emails,username,name,website,location,phone,link,founded",

                        }).ToString());


                        //parse the result into the pageinfo object and then to the collection
                        PageInfo pinfo = new PageInfo()
                        {
                            Name = (pageJO["username"] != null) ? pageJO["username"].ToString() : "",
                            NameBusiness = (pageJO["name"] != null) ? pageJO["name"].ToString() : "",
                            About = (pageJO["about"] != null) ? pageJO["about"].ToString().Replace(",", " ") : "",
                            Phone = (pageJO["phone"] != null) ? pageJO["phone"].ToString() : "",
                            Email = (pageJO["emails"] != null) ? pageJO["emails"][0].ToString() : "",
                            Website = (pageJO["website"] != null) ? pageJO["website"].ToString() : "",
                            FacebookPage = (pageJO["link"] != null) ? pageJO["link"].ToString() : "",
                            Catergory = (pageJO["category"] != null) ? pageJO["category"].ToString() : "",
                        };
                        JObject pJo = (pageJO["location"] != null) ? JObject.Parse(pageJO["location"].ToString()) : null;
                        if (pJo != null)
                        {
                            pinfo.Location = (pJo["street"] != null) ? pJo["street"].ToString().Replace(",", " ") : "";
                            pinfo.Location += (pJo["country"] != null) ? pJo["country"].ToString() : "";
                        }
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
                        {
                            pages.Add(pinfo);
                            pagelistview.ItemsSource = pages;
                            searchbtn.IsEnabled = true;
                            noOfCursor.IsEnabled = true;
                            

                        });
                    }
                }
                catch (Exception ex)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
                        {
                            MessageBox.Show(ex.Message);
                        });
                }
                
            }
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
            {
                MessageBox.Show("Search Completed");
            });

        }

        private async void searchbtn_Click(object sender, RoutedEventArgs e)
        {

            searchword = searchtxt.Text;
            pagelimit = int.Parse(noOfCursor.Text);


            if (searchword == "" || searchword == null)
            {
                MessageBox.Show("Something is wrong!!!\n Check the Search word or page limit must be integer");
            }
            else
            {
                //Start the thread
                th = new Thread(searchEngine);
                th.Start();
                searchbtn.IsEnabled = false;//disable the search button until the work is done
                noOfCursor.IsEnabled = false;
                savefile_btn.IsEnabled = true;
            }


            //await getPages();
        }

        private void searchtxt_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void savefile_btn_Click(object sender, RoutedEventArgs e)
        {
            th.Abort();
            total_progressbar.Visibility = Visibility.Hidden;

            if (pages.Count > 0)
            {
                int count = 1;//numbers the pages
                //naming of file
                Random rd = new Random();
                string nwfilename = "PageDiscovery" + rd.Next(9000) + ".csv";
                string newfile = @"..\..\..\..\..\..\..\Documents\" + nwfilename;
                FileStream f = new FileStream(newfile, FileMode.Create);
                //writing to file
                using (StreamWriter sw = new StreamWriter(f, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine("No , Name, Username, FacebookPage, Category, Website, Phone, Email, Location, About");
                    foreach (PageInfo E in pages)
                    {
                        sw.WriteLine(count + "  ,  " + E.Name + "  ,  " + E.NameBusiness + "  ,  " + E.FacebookPage + "  ,  " + E.Catergory + "  ,  " + E.Website + "  ,  " + E.Phone + "  ,  " + E.Email + "  ,  " + E.Location + "  ,  " + E.About.Replace("\n", " "));
                        count++;
                    }
                }
                //Displaying file info
                FileInfo f_info = new FileInfo(newfile);
                this.ShowMessageAsync("Saved File Info", "The file name is: " + nwfilename + "\n File path is:" + f_info.DirectoryName);
                f.Close();
            }
            else
            {
                MessageBox.Show("Result is empty. Try again");
            }
            
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FBAuth fbAuthenticate = new FBAuth("549750338734965", "https://www.facebook.com/connect/login_success.html");
            fbAuthenticate.ShowDialog();

            accesst = fbAuthenticate.AccessToken;
            MessageBox.Show(accesst);
        }
    }
}
