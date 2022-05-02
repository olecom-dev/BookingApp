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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BookingApp.Classes;
using System.Data.SqlClient;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace BookingApp.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class UserAccount : Page
    {
        
        public string connectionString = (App.Current as App).ConnectionString;
        public List<User> users = new List<User>();
        public UserAccount()
        {
            this.InitializeComponent();
            users = GetUser();
            BindListView();
            
            
        }
        public List<User> GetUser()
        {
            List<User> userList = new List<User>();


            //    Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            string getUserQuery = "Select * from Users";



                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (SqlException eSql)
                    {
                        MessageBox.DisplayDialog("Fehler", eSql.Message);
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
                                // mfiUsers.Visibility = Visibility.Collapsed;

                                var userauth = new User
                                {
                                    ID = reader.GetInt32(0),
                                    Firstname = reader.GetString(1),
                                    Lastname = reader.GetString(2),

                                    Username = reader.GetString(3),

                                    Password = reader.GetString(4),
                                    Role = reader.GetString(5),
                                    IsActive = reader.GetBoolean(6),
                                    EMail = reader.GetString(7)
                                };
                                userList.Add(userauth);
                                

                            }
                            return userList;

                            }
                        }




                    }
                    conn.Close();




                }
            return null;

            }
        public async void StackPanel_Tapped(object sender, RoutedEventArgs e)
        {
            var i =this.ListView1.SelectedIndex;
            var z = users[i].ID;
            Environment.SetEnvironmentVariable("userID", z.ToString());
            CDialog c = new CDialog();
           var result= await c.ShowAsync();
            
            if(result == ContentDialogResult.None){
                BindListView();
                if (users[i].Role.Contains("user"))
                {
                   
                    
                }

            }


        }
        public  void BindListView()
        {
            ListView1.ItemsSource = GetUser();
        }
    }
}
