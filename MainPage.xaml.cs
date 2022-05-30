using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using System.Data.SqlClient;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
//using Xamarin.Forms;
using BookingApp.Views;
using BookingApp.Classes;
using Button = Windows.UI.Xaml.Controls.Button;
using Windows.System;

using Windows.UI.Xaml.Media.Animation;
using System.Reflection;
using Windows.UI;
using System.Drawing;
using Color = Windows.UI.Color;
using User = BookingApp.Classes.User;
using Windows.UI.Popups;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace BookingApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        
    public Frame contentFrame1 = new Frame();
        public string connectionString = (App.Current as App).ConnectionString;
        public int userId;
        
        public MainPage()
        {
            
            this.InitializeComponent();
            //  LoadApplication(new Xamarin.Forms.Application());
            GlobalVariables.TaxRate = 7;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) =>
            {
                

                if (contentFrame.CanGoBack)
                {
                    e.Handled = true;
                 contentFrame.GoBack();
                }
            };
            ApplicationViewTitleBar appTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
            ApplicationView.GetForCurrentView().Title = "Hotelmanagement";
            // Set the title bar background and forground color
            appTitleBar.BackgroundColor = Colors.Snow;
            appTitleBar.ForegroundColor = Colors.Black;

            // Set the title bar inactive colors
            appTitleBar.InactiveBackgroundColor = Colors.DarkGray;
            appTitleBar.InactiveForegroundColor = Colors.LightGray;

            // Set the title bar button colors
            appTitleBar.ButtonBackgroundColor = Colors.DarkRed;
            appTitleBar.ButtonForegroundColor = Colors.Snow;

            // Title bar button hover state colors
            appTitleBar.ButtonHoverBackgroundColor = Colors.Orange;
            appTitleBar.ButtonHoverForegroundColor = Colors.Snow;

            // Title bar button inctive state colors
            appTitleBar.ButtonInactiveBackgroundColor = Colors.Red;
            appTitleBar.ButtonInactiveForegroundColor = Colors.Snow;

            // Title bar button pressed state colors
            appTitleBar.ButtonPressedBackgroundColor = Colors.IndianRed;
            appTitleBar.ButtonPressedForegroundColor = Colors.Snow;






        }



        private async  void Page_Loaded(object sender, RoutedEventArgs e)
        {
           
                LoginDialog dia = new LoginDialog();
          await dia.ShowAsync();

          //  setAccount("");
            








          //  this.contentFrame.Navigate(typeof(CustomerPage));
            //  if (localSettings.Values["userID"] != null)
            // tblockAccount.Text = localSettings.Values["userID"].ToString();
            


            foreach (var item in NavView.MenuItems)
            {


                if (item is NavigationViewItem)
                {
                    
                 //   NavView.SelectedItem = item;
                    // NavView.IsPaneOpen = true;
                 //   this.contentFrame.Navigate(typeof(CustomerPage));
                }

                


            }





        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {


                contentFrame.Navigate(typeof(BlankPage));
            

       /*     try
            {
              //  NavView.SelectedItem = 4;
               // this.contentFrame.Navigate(typeof(CustomerPage));
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); } */
        }

        internal static void FindChildren<T>(List<T> results, DependencyObject startNode)
  where T : DependencyObject
        {
            int count = VisualTreeHelper.GetChildrenCount(startNode);
            for (int i = 0; i < count; i++)
            {
                DependencyObject current = VisualTreeHelper.GetChild(startNode, i);
                if ((current.GetType()).Equals(typeof(T)) || (current.GetType().GetTypeInfo().IsSubclassOf(typeof(T))))
                {
                    T asType = (T)current;
                    results.Add(asType);
                }
                FindChildren<T>(results, current);
            }
        }
        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {

            NavView.IsPaneOpen = true;

            if (args.IsSettingsInvoked)
            {
                this.contentFrame.Navigate(typeof(SettingsPage));

            }
            else
            {
                AnimateFrame();
            }
        }

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {

            Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);


        }
        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }
        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            /*    Thickness contentF = contentFrame.Margin;
                if (NavView.IsPaneOpen == true)
                {
                    contentF.Left = 300;
                    this.contentFrame.Margin = contentF;
                }
                else
                {
                    contentF.Left = 0;
                    this.contentFrame.Margin = contentF;
                } */

            if (args.IsSettingsSelected)
            {

                contentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                AnimateFrame();

            }

        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            //     this.contentFrame.Navigate(typeof(SettingsPage));
            NavView.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
        }
        private async void Press(object sender, RoutedEventArgs e)
        {
            var options = new Windows.System.LauncherOptions
            {
                TreatAsUntrusted = false,
                DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseNone
            };
            await Windows.System.Launcher.LaunchUriAsync(new Uri("calculator:"), options);

        }
        private void NavView_PaneOpening(NavigationView NavView, object sender)
        {

            AnimateFrame();
        }
        private void NavView_PaneClosing(NavigationView NavView, object sender)
        {

            AnimateFrame();
        }

        public void TbText_Changing(object sender, TextBoxTextChangingEventArgs e)
        {
            
        }
        public async void SetAccount(string id)
        {
            mfiUsers.Visibility = Visibility.Collapsed;
            List<User> userList = new List<User>();
            userId = Int32.Parse(id);

            if (id != "")
            {
      //          Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
                string getUserQuery = "Select  ID, Username, Password, Role, IsActive from Users where ID = " + Int32.Parse(id);



                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (SqlException eSql)
                    {
                        MessageBox.DisplayDialog("Fehler",eSql.Message);
                    }
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = getUserQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                var userauth = new User();
                                while (reader.Read())
                                {
                                   // mfiUsers.Visibility = Visibility.Collapsed;
                                    

                                    userauth.ID = reader.GetInt32(0);

                                    userauth.Username = reader.GetString(1);

                                    userauth.Password = reader.GetString(2);
                                    userauth.Role = reader.GetString(3);
                                   // tblockAccount.Width = 200;
                                    
                                    userauth.IsActive = reader.GetBoolean(4);

                                   userList.Add(userauth);
                                    tblockAccount.Text = userauth.Username + " (" + userauth.Role.Trim() + ")";

                                    if (userauth.Role.ToString().Contains("admin") & userauth.IsActive == true)
                                    {

                                        mfiUsers.Visibility = Visibility.Visible;

                                        this.contentFrame.Navigate(typeof(UserAccount));
                                        break;

                                    }
                                    else if (userauth.Role.ToString().Contains("user") & userauth.IsActive == true)
                                    {
                                        mfiUsers.Visibility = Visibility.Collapsed;
                                        
                                       
                                        this.contentFrame.Navigate(typeof(RoomsPage));
                                        break;
                                    }
                                    else if (userauth.IsActive == false)
                                    {
                                        this.contentFrame.Navigate(typeof(BlankPage));
                                        MessageDialog messageDialog = new MessageDialog("Es liegt keine Berechtigung vor.\nWenden Sie sich an den Administrator!");
                                        await messageDialog.ShowAsync();

                                        App.Current.Exit();
                                        


}
                                }
                              //  return userauth;
                                
                            }
                        }




                    }
                    conn.Close();

 


                }
                
                
            }

         //   return null;

        }
        public void AnimateFrame()
        {
            var ItemContent = NavView.SelectedItem as NavigationViewItem;

            /*        Thickness contentF = contentFrame.Margin;
                    if (NavView.IsPaneOpen == true)
                    {
                        contentF.Left = 300;
                        this.contentFrame.Margin = contentF;

                    }
                    if(NavView.IsPaneOpen==false)
                    {
                        contentF.Left = 0;
                        this.contentFrame.Margin = contentF;
                      } */
          
          
            if (ItemContent != null)
            {
                switch (ItemContent.Tag)
                {
                    case "Rooms_Page":
                        {
                            contentFrame.Navigate(typeof(RoomsPage));
                            break;
                        }
                    case "Customer_Page":
                        {
                           this.contentFrame.Navigate(typeof(CustomerPage));
                        


                            break;
                        }

                    case "Reserved_Page":
                        {
                          this.contentFrame.Navigate(typeof(ReservedPage),"A1");
                            break;
                        }
                    case "AllBookings_Page":
                        {
                            contentFrame.Navigate(typeof(AllBookingsPage));
                            break;
                        }
                    case "Restaurant_Page":
                        {
                            contentFrame.Navigate(typeof(RestaurantPage));
                            break;
                        }
   
                    case "NewData_Page":
                        {   
                            contentFrame.Navigate(typeof(NewDataPage));
                            break;
                        }
             


                }
            }



        }
        private async void NavigationViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavigationViewItem navigationViewItem = sender as NavigationViewItem;
            navigationViewItem.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 250, 250));
            
           
            if (NavView.IsPaneOpen)
            {

                
                navigationViewItem.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

            }


            else
            {
                navigationViewItem.Foreground = new SolidColorBrush(Color.FromArgb(255,255, 250, 250));
            }
          //  string UserID = Environment.GetEnvironmentVariable("userID");
            LoginDialog dia = new LoginDialog();
             ContentDialogResult res= await dia.ShowAsync();
            if(res == ContentDialogResult.Primary)
                
            {

            }


        }
        private  void LoginDialogButton_Click(object sender, RoutedEventArgs e)
        {
            LoginDialog l = new LoginDialog();

            if (l.Result == SignInResult.SignInOK)
            {
                
            }
            else if (l.Result == SignInResult.SignInFail)
            {
                // Sign in failed.
            }
            else if (l.Result == SignInResult.SignInCancel)
            {
                // Sign in was cancelled by the user.
            }
        }
        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
        private void MfiUsers_Click( object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(UserAccount));
        }
        private async void MfiUserDate_Click(object sender, RoutedEventArgs e)
        {
            
            AccountInfoDialog accountInfoDialog = new AccountInfoDialog(userId);
           await accountInfoDialog.ShowAsync();
        }
        private async void MfiUsersLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginDialog l = new LoginDialog();
            contentFrame.Navigate(typeof(BlankPage));
            tblockAccount.Text = "Mein Konto";
            await l.ShowAsync();
        }
        private void MfiNewData_Click(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(NewDataPage));
        }
        private async void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            Environment.SetEnvironmentVariable("userID", "-1");
            var p = contentFrame.Content as Page;
            if(p.GetType() == typeof(UserAccount))
            {
                CDialog c = new CDialog();
                await c.ShowAsync();
            }
            if(p.GetType() == typeof(RestaurantPage))
            {
                NewProductDialog npd = new NewProductDialog();
                await npd.ShowAsync();
            }
            if(p.GetType()== typeof(CustomerPage))
            {
                Environment.SetEnvironmentVariable("customerID", "-1");
                contentFrame.Navigate(typeof(CustomerPage));
            }
        }
        private void MfiUserNew_Click(object sender, RoutedEventArgs e)
        {

           
            contentFrame.Navigate(typeof(CustomerPage));
        }
        private async void MfiNewProduct_Click(object sender, RoutedEventArgs e)
        {
            ContentDialogResult cdr = new ContentDialogResult();
            NewProductDialog npd = new NewProductDialog();
            cdr = await npd.ShowAsync();
               if(cdr == ContentDialogResult.Secondary)
            {
                contentFrame.Navigate(typeof(RestaurantPage));
            }
        }
        private void MfiBillComplete_Click(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(PrintAllRestaurantBookings));
        }
    }
    
}
