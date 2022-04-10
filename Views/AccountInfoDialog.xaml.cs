using BookingApp.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Inhaltsdialogfeld" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace BookingApp.Views
{
    public sealed partial class AccountInfoDialog : ContentDialog
    {
        public int uid;
        public string ConnectionString = (App.Current as App).ConnectionString;
        public User users = new User();
        public AccountInfoDialog(int id)
        {
            this.InitializeComponent();
            this.uid = id;
            users = GetUser(ConnectionString);

            tbLastLogin.Text = GlobalVariables.LastLogin.ToString();
            tbUsername.Text = users.Username;
            tbFirstname.Text = users.Firstname;
            tbLastname.Text = users.Lastname;
            tbEMail.Text = users.EMail;
            pbPassword.Password = Encryption.Decrypt(users.Password);
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            pbPassword.IsEnabled = true;
            pbPasswordRepeat.IsEnabled = true;
            tblPasswordRepeat.Visibility = Visibility.Visible;
            pbPasswordRepeat.Visibility = Visibility.Visible;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
        private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
        public User GetUser(string connectionString)
        {
            string getUserQuery = "Select * from Users where ID =" + this.uid;
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException eSql)
                {
                    MsgBox.Show(eSql.Message);
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

                                    Firstname = reader.GetString(1),

                                    Lastname = reader.GetString(2),
                                    Username = reader.GetString(3),
                                    Password = reader.GetString(4),
                                    Role = reader.GetString(5),
                                    IsActive = reader.GetBoolean(6),
                                    EMail = reader.GetString(7),
                                    LastLogin = reader.GetDateTime(8),
                                    Counter = reader.GetInt32(9)
                                };
                                return userauth;
                            }
                            

                        }

                    }

                }


            }




            return null;

        }
    }
}
