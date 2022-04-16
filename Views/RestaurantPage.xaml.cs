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
using System.Timers;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace BookingApp.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class RestaurantPage : Page
    {
        List<RestaurantBooking> restaurantBookings = new List<RestaurantBooking>();
        List<Category> cats = new List<Category>();
        List<Products> prods = new List<Products>();
        List<string> productCodes = new List<string>();
        List<Table> tables = new List<Table>();
        List<string> tab = new List<string>();
        int count = 1;
     
        public string ConnectionString = (App.Current as App).ConnectionString;
        public RestaurantPage()
        {
            this.InitializeComponent();
            cats = GetCategorys(ConnectionString);
            tables = GetTables(ConnectionString, null);
            cats.Insert(0, new Category { CategoryID = 0, CategoryName = "Alle Kategorien" });
            cboxCategorys.ItemsSource = cats;
            cboxCategorys.DisplayMemberPath = "CategoryName";
            cboxCategorys.SelectedValuePath = "CategoryID";
            prods = GetProducts(ConnectionString, 0);
            productCodes = prods.Select(t => t.ProductCode).ToList();
            tab = tables.Select(t => t.TableNumber).ToList();
            txtAutoComplete.ItemsSource = productCodes;
            txtAutoTables.ItemsSource = tab;
            tbCount.Text = count.ToString();


            ProductsList.ItemsSource = prods;
            TablesList.ItemsSource = tables;
            //     ProductsList.SelectedIndex = 0;
        }
        private void TablesList_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            int index = tables.IndexOf((Table)TablesList.SelectedItem);
            txtAutoTables.Text = tables[index].TableNumber;
            tbTableID.Text = tables[index].TableID.ToString();
            tbTableNumber.Text = tables[index].TableNumber;
            tbTableLocation.Text = tables[index].Location;
        }
        private void RestaurantPage_Loaded(object sender, RoutedEventArgs e)

        {

            //btnOptions.Focus(FocusState.Programmatic);

            tbTableNumber.Focus(FocusState.Keyboard);

        }
        private void ProductsList_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {

            int index = prods.IndexOf((Products)ProductsList.SelectedItem);
            if (index >= 0)
            {
                tbProductID.Text = prods[index].ProductID.ToString();
                tbProductName.Text = prods[index].ProductName;
                txtAutoComplete.Text = prods[index].ProductCode;
                tbProductCode.Text = prods[index].ProductCode;
                tbProductPrice.Text = prods[index].ProductPrice.ToString();
                tbProductDescription.Text = prods[index].ProductDescription;
            }
        }
        private void CboxCategorys_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {

            ProductsList.ItemsSource = prods.Where(t => t.CategoryID.Equals(cboxCategorys.SelectedValue));
            // ProductsList.SelectedIndex = 0;
        }
        private void TxtAutoComplete_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.CheckCurrent())
            {

                var search_term = txtAutoComplete.Text;
                var results = productCodes.Where(i => i.StartsWith(search_term)).ToList();
                txtAutoComplete.ItemsSource = results;


            }
        }

        private void TxtAutoComplete_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {


            txtAutoComplete.Text = args.SelectedItem as string;
            var z = prods.Where(t => t.ProductCode.Equals(args.SelectedItem)).ToList();
            tbProductID.Text = z[0].ProductID.ToString();
            tbProductName.Text = z[0].ProductName;
            tbProductPrice.Text = z[0].ProductPrice.ToString();
            tbProductDescription.Text = z[0].ProductDescription;

        }
        private void TxtAutoComplete_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {

            var search_term = args.QueryText;
            var results = productCodes.Where(i => i.StartsWith(search_term)).ToList();
            txtAutoComplete.ItemsSource = results;
            //   txtAutoComplete.DisplayMemberPath = "ProductCode";
            txtAutoComplete.IsSuggestionListOpen = true;
        }
        private void TxtAutoSuggestBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var z = prods.Where(t => t.ProductCode.Equals(txtAutoComplete.Text)).ToList();

                tbProductID.Text = z[0].ProductID.ToString();
            }
        }
        private void TxtAutoTables_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.CheckCurrent())
            {

                var search_term = txtAutoComplete.Text;
                var results = tab.Where(i => i.StartsWith(search_term)).ToList();
                txtAutoComplete.ItemsSource = results;


            }
        }
        private void TxtAutoTables_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {


            txtAutoTables.Text = args.SelectedItem as string;
            var z = tables.Where(t => t.TableNumber.Equals(args.SelectedItem)).ToList();
            tbTableID.Text = z[0].TableID.ToString();
            tbTableLocation.Text = z[0].Location;

        }
        private void TxtAutoTables_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {

            var search_term = args.QueryText;
            var results = tab.Where(i => i.StartsWith(search_term)).ToList();
            txtAutoComplete.ItemsSource = results;
            //   txtAutoComplete.DisplayMemberPath = "ProductCode";
            txtAutoComplete.IsSuggestionListOpen = true;
        }
        public List<Products> GetProducts(string connectionString, int CategoryID)
        {
            string getProductsQuery;
            if (CategoryID == 0)
            {
                getProductsQuery = "Select ProductID, ProductCode, ProductName, ProductDescription, ProductPrice, Products.CategoryID, Category.CategoryName from Products inner join Category on Products.CategoryID = Category.CategoryID";
            }
            else
            {
                getProductsQuery = "Select ProductID, ProductCode, ProductName, ProductDescription, ProductPrice, Products.CategoryID, Category.CategoryName from Products inner join" +
                    " Category on Products.CategoryID = Category.CategoryID where Category.CategoryID=@CategoryID";
            }
            var products = new List<Products>();
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
                        cmd.CommandText = getProductsQuery;
                        cmd.Parameters.AddWithValue("@CategoryID", CategoryID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var product = new Products
                                {
                                    ProductID = reader.GetInt32(0),
                                    ProductCode = reader.GetString(1),
                                    ProductName = reader.GetString(2),
                                    ProductDescription = reader.GetString(3),
                                    ProductPrice = reader.GetDecimal(4),
                                    CategoryID = reader.GetInt32(5),
                                    CategoryName = reader.GetString(6)

                                };
                                products.Add(product);
                            }
                            return products;

                        }
                    }

                }


            }

            return null;

        }
        private List<Category> GetCategorys(string connectionString)
        {
            List<Category> categorys = new List<Category>();
            string query = "Select * from Category";
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
                        cmd.CommandText = query;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var category = new Category
                                {
                                    CategoryID = reader.GetInt32(0),
                                    CategoryName = reader.GetString(1),


                                };
                                categorys.Add(category);
                            }
                            return categorys;
                        }
                    }

                }

            }
            return null;
        }
        private List<Table> GetTables(string connectionString, string tableNumber)
        {
            string query;
            List<Table> tables = new List<Table>();
            if (String.IsNullOrEmpty(tableNumber))
            {
                query = "Select * from Tables";
            }
            else
            {
                query = "Select * from Tables where TableNumber=@TableNumber;";

            }
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

                        if (!String.IsNullOrEmpty(tableNumber))
                        {
                            cmd.Parameters.AddWithValue("@TableNUmber", tableNumber);
                        }
                        cmd.CommandText = query;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var table = new Table
                                {
                                    TableID = reader.GetInt32(0),
                                    TableNumber = reader.GetString(1),
                                    Location = reader.GetString(2)

                                };
                                tables.Add(table);
                            }
                            return tables;
                        }
                    }

                }

            }
            return null;
        }


        private  void BtnBook_Click(object sender, RoutedEventArgs args)
        {
            
            var product = prods.Where(t => t.ProductCode.Equals(tbProductCode.Text)).ToList();
            if (product.Count > 0) {
                RestaurantBooking res = new RestaurantBooking();
                res.Price = product[0].ProductPrice;
                res.Multiplicator = Int32.Parse(tbCount.Text);
                res.BookingTime = DateTime.Now;
                res.BookingCode = product[0].ProductCode;
                res.TableNumber = tbTableNumber.Text;
                tbPrice.Text = res.PriceOverall.ToString();
                restaurantBookings.Add(res);
                ClearTextBoxes();
            }
       
          

        }
        private void TbCount_TextChanged(object sender, TextChangedEventArgs args)
        {
            var product = prods.Where(t => t.ProductCode.Equals(tbProductCode.Text)).ToList();
            if (product.Count > 0)
            {
                tbPrice.Text = (product[0].ProductPrice * Int32.Parse(tbCount.Text)).ToString();
            }
            }
        private void TbProductCode_TextChanged(object sender, TextChangedEventArgs args)
        {
            var product = prods.Where(t => t.ProductCode.Equals(tbProductCode.Text)).ToList();
            if (product.Count > 0)
            {
                tbPrice.Text = (product[0].ProductPrice * Int32.Parse(tbCount.Text)).ToString();
            }
        }
        private   void ClearTextBoxes()
        {
            
            tbProductCode.Text = "";
            tbCount.Text = "1";
            tbPrice.Text = "";
            
            
        }
    }
}
