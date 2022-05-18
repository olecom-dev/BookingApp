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
    public sealed partial class NewTableDialog : ContentDialog
    {
        public NewTableDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string InsertNewTable = "Insert into Tables (TableNumber, Location) Values (@TableNumber, @Location)";
            using (SqlConnection conn = new SqlConnection((App.Current as App).ConnectionString))
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
                        cmd.CommandText = InsertNewTable;

                        cmd.Parameters.AddWithValue("@TableNumber", tbTableNumber.Text);

                        cmd.Parameters.AddWithValue("@Location", tbTableLocation.Text);


                        try
                        {
                            cmd.ExecuteNonQuery();

                        }
                        catch (SqlException eSql)
                        {
                            MessageBox.DisplayDialog("Fehler", eSql.Message);
                        }
                    }
                }
                conn.Close();
        
                
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e) { }

    }
}
