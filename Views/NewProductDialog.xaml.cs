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
    public sealed partial class NewProductDialog : ContentDialog
    {
        List<Category> categories = new List<Category>();
        string categoryName = "";
        public NewProductDialog()
        {
            this.InitializeComponent();
            categories = GetAllCategories((App.Current as App).ConnectionString);
            cmbProductCategory.ItemsSource = categories;
            cmbProductCategory.DisplayMemberPath = "CategoryName";
            cmbProductCategory.SelectedValuePath = "CategoryID";
            cmbProductCategory.TextSubmitted += CmbProductCategory_TextSubmitted;

        }
        private void CmbProductCategory_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs e)
        {
            sender.SelectedIndex = -1;
            categoryName = e.Text;


    
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string insertCategory = "Insert into Category (CategoryName) values (@CategoryName)";
            string insertProduct = "Insert into Products (ProductCode, ProductName, ProductDescription, ProductPrice, CategoryID) Values (@ProductCode, @ProductName, @ProductDescription, @ProductPrice, @CategoryID)";


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

                        cmd.CommandText = insertCategory;
                        cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                        if (cmbProductCategory.SelectedIndex == -1)
                        {
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.DisplayDialog("Fehler", ex.Message);
                            }
                        }
                    }
                    
                
            
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = insertProduct;
                            cmd.Parameters.AddWithValue("@CategoryID", cmbProductCategory.SelectedValue);
                            cmd.Parameters.AddWithValue("@ProductCode", tbProductCode.Text);
                            cmd.Parameters.AddWithValue("@ProductName", tbProductName.Text);
                            cmd.Parameters.AddWithValue("@ProductDescription", tbProductDescription.Text);
                            cmd.Parameters.AddWithValue("@ProductPrice", tbProductPrice.Text);
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.DisplayDialog("Fehler", ex.Message);
                            }
                        }

                    }

                    conn.Close();
                    cmbProductCategory.ItemsSource = GetAllCategories((App.Current as App).ConnectionString);
                    
                
                args.Cancel = true;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private List<Category> GetAllCategories(string connectionString)
        {
            List<Category> categories = new List<Category>();
            string getCategoriesQuery = "Select * From Category";
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
                        cmd.CommandText = getCategoriesQuery;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var userauth = new User();
                            while (reader.Read())
                            {
                                Category cat = new Category{
                                    CategoryID = reader.GetInt32(0),
                                    CategoryName = reader.GetString(1)
                                };
                                categories.Add(cat);
                            }
                            return categories;
                        }
                    }
                }
                conn.Close();
            }
           return null;
        }
    }
}
