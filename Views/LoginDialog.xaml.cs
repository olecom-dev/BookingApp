// SignInContentDialog.xaml.cs
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using BookingApp.Classes;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using BookingApp.Views;

namespace BookingApp
{
    public enum SignInResult
    {
        SignInOK,
        SignInFail,
        SignInCancel,
        Nothing
    }

    public sealed partial class LoginDialog : ContentDialog
    {
        public int userID;
        
        public int id;
        public int LoginCount;
        public SignInResult Result { get;  set; }
      //  public List<User> users = new List<User>();
        public string ConnectionString = (App.Current as App).ConnectionString;
        

        

        public LoginDialog()
            
        {
            this.InitializeComponent();
            this.Opened += LoginDialog_Opened;
            this.Closing += LoginDialog_Closing;
            
        }
        public List<User> GetUser(string connectionString)
        {
             string getUserQuery = "Select ID, Username, Password, Role, LastLogin , Counter from Users;";
            var users = new List<User>();
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
                            while (reader.Read())
                            {
                                var userauth = new User
                                {
                                    ID = reader.GetInt32(0),

                                    Username = reader.GetString(1),

                                    Password = reader.GetString(2),
                                    Role = reader.GetString(3),
                                    LastLogin = reader.GetDateTime(4),
                                    Counter = reader.GetInt32(5)
                                };
                                users.Add(userauth);
                            }
                            return users;

                        }
                       
                    }
                   
                }


            }




            return null;

        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
            
       /*     var us = GetUser(ConnectionString);

            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            // Ensure the user name and password fields aren't empty. If a required field
            // is empty, set args.Cancel = true to keep the dialog open.
            if (string.IsNullOrEmpty(userNameTextBox.Text))
            {
                
                errorTextBlock.Text = "User name is required.";
                args.Cancel = true;
            }
            if (string.IsNullOrEmpty(passwordTextBox.Password))
            {
                
                errorTextBlock.Text = "Password is required.";
                args.Cancel = true;
            }



            // If you're performing async operations in the button click handler,
            // get a deferral before you await the operation. Then, complete the
            // deferral when the async operation is complete.


            if(!string.IsNullOrEmpty(passwordTextBox.Password) && !string.IsNullOrEmpty(userNameTextBox.Text))
            {
                var u = us.Where (f=>Encryption.Decrypt(f.Password).Equals(passwordTextBox.Password)).Where(g=>g.Username.Equals(userNameTextBox.Text)).Distinct().FirstOrDefault();

     

                    
                    if (u!=null)
                    {


                    userID = u.ID;
                    GlobalVariables.LastLogin = u.LastLogin;
                        this.Result = SignInResult.SignInOK;

                        
                        // saveUserNameCheckBox.IsChecked = true;

                        //  this.Hide();

                    }




                    else
                    {


                        this.Result = SignInResult.SignInFail;
                    // args.Cancel = true;
                    
                        ClearUserName();


                    }

                




            } */

    
        }
        private void PasswordTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                this.Hide();
            }

        }

        private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // User clicked Cancel, ESC, or the system back button.
            this.Result = SignInResult.SignInCancel;
            
        }

        void LoginDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            this.Result = SignInResult.Nothing;

            // If the user name is saved, get it and populate the user name field.
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            
 
                if (roamingSettings.Values["userName"] != null)
                {
                    this.Result = SignInResult.SignInOK;
                 //   saveUserNameCheckBox.IsChecked = true;
               //     userNameTextBox.Text = roamingSettings.Values["userName"].ToString();
                
                }
            
        }

        void LoginDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            var us = GetUser(ConnectionString);

            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            // Ensure the user name and password fields aren't empty. If a required field
            // is empty, set args.Cancel = true to keep the dialog open.
            if (string.IsNullOrEmpty(userNameTextBox.Text))
            {

                errorTextBlock.Text = "User name is required.";
                args.Cancel = true;
            }
            if (string.IsNullOrEmpty(passwordTextBox.Password))
            {

                errorTextBlock.Text = "Password is required.";
                args.Cancel = true;
            }



            // If you're performing async operations in the button click handler,
            // get a deferral before you await the operation. Then, complete the
            // deferral when the async operation is complete.


            if (!string.IsNullOrEmpty(passwordTextBox.Password) && !string.IsNullOrEmpty(userNameTextBox.Text))
            {
                var u = us.Where(f => Encryption.Decrypt(f.Password).Equals(passwordTextBox.Password)).Where(g => g.Username.Equals(userNameTextBox.Text)).Distinct().FirstOrDefault();




                if (u != null)
                {


                    userID = u.ID;
                    GlobalVariables.LastLogin = u.LastLogin;
                    this.Result = SignInResult.SignInOK;


                    // saveUserNameCheckBox.IsChecked = true;

                    //  this.Hide();

                }




                else
                {


                    this.Result = SignInResult.SignInFail;
                    // args.Cancel = true;

                    ClearUserName();


                }






            }

            //   Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            //     roamingSettings.Values["userName"] = userNameTextBox.Text;
            var query = "Update Users set LastLogin=@LastLogin where ID=" + userID;

            // If sign in was successful, save or clear the user name based on the user choice.
            if (this.Result == SignInResult.SignInOK)
            {
                SqlConnection con = new SqlConnection(ConnectionString);
                SqlCommand command = new SqlCommand(query);
                command.Parameters.AddWithValue("@LastLogin", DateTime.Now);
                command.Connection = con;
                try
                {
                    con.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException eSql)
                {
                    MessageBox.DisplayDialog("Fehler", eSql.Message);
                }
                finally
                {
                    con.Close();
                }
                
                // userID = userNameTextBox.Text;
                
                ((Window.Current.Content as Frame).Content as MainPage).SetAccount(userID.ToString()); 
     /*           if (saveUserNameCheckBox.IsChecked == true)
                {
                    SaveUserName();
                    
                }
                else
                {
                    ClearUserName();
                }
     */
               
            }
             if (this.Result == SignInResult.SignInFail)
            {
                args.Cancel = true;
                errorTextBlock.Text = "Benutzername / Passwort unbekannt";
                
            }
            if (this.Result == SignInResult.SignInCancel)
            {
                
                App.Current.Exit();
            }
            if (this.Result == SignInResult.Nothing)
            {
                args.Cancel = true;
                errorTextBlock.Text = "Benutzername / Passwort unbekannt";
            }

            // If the user entered a name and checked or cleared the 'save user name' checkbox, then clicked the back arrow,
            // confirm if it was their intention to save or clear the user name without signing in.
            if (this.Result == SignInResult.SignInOK && !string.IsNullOrEmpty(userNameTextBox.Text))
            {
          /*      if (saveUserNameCheckBox.IsChecked == false)
                {
                    
                    FlyoutBase.SetAttachedFlyout(this, (FlyoutBase)this.Resources["DiscardNameFlyout"]);
                    FlyoutBase.ShowAttachedFlyout(this);
                }
                if (saveUserNameCheckBox.IsChecked == true && !string.IsNullOrEmpty(userNameTextBox.Text))
                {
                   // args.Cancel = true;
                    FlyoutBase.SetAttachedFlyout(this, (FlyoutBase)this.Resources["SaveNameFlyout"]);
                    FlyoutBase.ShowAttachedFlyout(this);
                }
          */
            }
        }

        private void SaveUserName()
        {
         //   Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
        //    roamingSettings.Values["userName"] = userNameTextBox.Text;
            
            this.Hide();
        }

        private void ClearUserName()
        {
        //    Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

         //   roamingSettings.Values["userName"] = userNameTextBox.Text;
            userNameTextBox.Text = string.Empty;
            passwordTextBox.Password = string.Empty;
        }

        // Handle the button clicks from the flyouts.
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveUserName();
            
        }

        private void DiscardButton_Click(object sender, RoutedEventArgs e)
        {
            ClearUserName();
            FlyoutBase.GetAttachedFlyout(this).Hide();
        }

        // When the flyout closes, hide the sign in dialog, too.
        private void Flyout_Closed(object sender, object e)
        {
            this.Hide();
        }


        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (revealModeCheckBox.IsChecked == true)
            {
                passwordTextBox.PasswordRevealMode = PasswordRevealMode.Visible;
            }
            else
            {
                passwordTextBox.PasswordRevealMode = PasswordRevealMode.Hidden;
            }
        }

    }
}
