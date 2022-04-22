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

// Die Elementvorlage "Inhaltsdialogfeld" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace BookingApp.Views
{
    public sealed partial class UserEditDialog : ContentDialog
           {
      //  private string password;
        public enum MyResult
        {
            Yes,
            No,
            Cancle,
            Nothing
        }
        public string connectionString = (App.Current as App).ConnectionString;
        public List<User> users = new List<User>();
        public string id;
        public string password;
        public string query = "";
        public UserEditDialog()
        {
            this.InitializeComponent();
           
            List<string> roles = new List<string>();
            tbRole.SelectedIndex = 0;
            // Put some colors to the list
            roles.Add("user");
            roles.Add("admin");
            tbRole.ItemsSource = roles;
            id = Environment.GetEnvironmentVariable("userID");
            if (id == "-1")
            {
                this.Title = "Benutzer anlegen";
                tbID.Text = id;
                tblPassword.Visibility = Visibility.Visible;
                pbPassword.Visibility = Visibility.Visible;
                tblPasswordRepeat.Visibility = Visibility.Visible;
                pbPasswordRepeat.Visibility = Visibility.Visible;
            }
            else
            {


                users = GetUser(id);
                if (users[0] != null)
                {
                    tbID.Text = users[0].ID.ToString();
                    tbFirstname.Text = users[0].Firstname;
                    tbLastname.Text = users[0].Lastname;
                    tbUsername.Text = users[0].Username;
                    pbPassword.Password = Encryption.Decrypt(users[0].Password);
                    password = pbPassword.Password;
                    if (users[0].Role.Contains("admin"))
                        tbRole.SelectedIndex = 0;
                    if (users[0].Role.Contains("user"))
                        tbRole.SelectedIndex = 1;
                    ;
                    tbEMail.Text = users[0].EMail;
                    cbIsActive.IsChecked = users[0].IsActive;
                }
            }
            

        }


        public List<User> GetUser(string id)
        {
            List<User> userList = new List<User>();


            
            string getUserQuery = "Select * from Users where ID="+id;



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
        private void Btn1_Click(object sender,ContentDialogButtonClickEventArgs e)
        {
            this.Title = "Benutzer anlegen";
            tbID.Text = "-1";
            id = "-1";
            tbFirstname.Text = String.Empty;
            tbLastname.Text = String.Empty;
            tbUsername.Text = String.Empty;
            
            pbPassword.Password = String.Empty;
            tblPassword.Visibility = Visibility.Visible;
            pbPassword.Visibility = Visibility.Visible;
            pbPasswordRepeat.Visibility = Visibility.Visible;
            tblPasswordRepeat.Visibility = Visibility.Visible;
            
            tbRole.SelectedIndex = 1;
            tbEMail.Text = String.Empty;
            cbIsActive.IsChecked = false;
         e.Cancel = true;
            // Close the dialog
            // this.Hide();
            
             
            
           
            
        }

        private void Btn2_Click(object sender, ContentDialogButtonClickEventArgs e)
        {

                if (id == "-1")
                {
                password = pbPassword.Password;
                query = "Insert into Users (Firstname, Lastname, Username, Password, Role, EMail, IsActive, LastLogin, Counter) Values (@Firstname, @Lastname, @Username, @Password, @Role, @Email, @IsActive, @LastLogin, @Counter)";
                }
                else
                {
                pbPasswordRepeat.Password = pbPassword.Password;
                query = "Update Users set Firstname=@Firstname, Lastname=@Lastname, Username=@Username, Password=@Password, Role=@Role, EMail=@Email, IsActive=@IsActive, Counter=@Counter where ID=" + Int32.Parse(id);
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                    if (!pbPassword.Password.Equals(pbPasswordRepeat.Password))
                    {
                        MessageBox.DisplayDialog("Fehler","Passwortwort stimmt nicht überein!");
                    }
                    else
                    {
                        command.Connection = connection;            // <== lacking

                        command.CommandText = query;
                        command.Parameters.AddWithValue("@Firstname", tbFirstname.Text);
                        command.Parameters.AddWithValue("@Lastname", tbLastname.Text);
                        command.Parameters.AddWithValue("@Username", tbUsername.Text);
                        command.Parameters.AddWithValue("@Password", Encryption.Encrypt(password));
                        command.Parameters.AddWithValue("@Role", tbRole.SelectedItem);
                        command.Parameters.AddWithValue("@EMail", tbEMail.Text);
                        command.Parameters.AddWithValue("@IsActive", cbIsActive.IsChecked);
                        command.Parameters.AddWithValue("@Counter", 0);
                        if (id == "-1")
                        {
                            command.Parameters.AddWithValue("@LastLogin", DateTime.Now);
                        }
                        try
                        {
                            connection.Open();
                            int recordsAffected = command.ExecuteNonQuery();
                            if (recordsAffected >= 1)
                            {
                                if (query.StartsWith("U"))
                                    MessageBox.DisplayDialog("Fehler","Daten erfolgreich geändert!");
                                if (query.StartsWith("I"))
                                    MessageBox.DisplayDialog("Fehler","Daten erfolgreich eingefügt!");
                            }
                            else
                                MessageBox.DisplayDialog("Fehler","Fehler beim ändern/einfügen des Datensatzes.");
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.DisplayDialog("Fehler",ex.Message);
                        }
                        finally
                        {
                            connection.Close();

                        }
                    }
                    }
                    e.Cancel = true;
                }
            

            // Close the dialog
            
        }

        private void Btn3_Click(object sender, ContentDialogButtonClickEventArgs e)
        {
            id = "0";
            // Close the dialog

            this.Hide();
        }
        private void CDialog_Closing(object sender, ContentDialogClosingEventArgs args)
        {
            ((Window.Current.Content as Frame).Content as MainPage).contentFrame.Navigate(typeof(UserAccount));

















        }
    }
}
