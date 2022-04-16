using BookingApp.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace BookingApp.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    
    public sealed partial class NewDataPage : Page
    {
        public string ConnectionString = (App.Current as App).ConnectionString;
        StorageFile file = null;
        byte[] fileBytes = null;
        List<Rooms> listRooms = new List<Rooms>();
        public NewDataPage()
        {
            this.InitializeComponent();
        }
        private async void NewDataPage_Loaded(object sender, RoutedEventArgs e)
        {
            listRooms = await GetRooms();
            cboRooms.ItemsSource = listRooms;
            cboRooms.DisplayMemberPath = "RoomNumber";
            cboRooms.SelectedValuePath = "ID";
        }


        private void BtnSaveRoom_Click(object sender, RoutedEventArgs e)
        {
            string query;
            if(cboRooms.SelectedIndex > -1)
            {
                if (file != null)
                {
                    query = "Update Rooms set RoomNumber=@RoomNumber, NumberOfBeds=@NumberOfBeds, PricePerNight=@PricePerNight, RoomSize=@RoomSize, Image=@Image, Available=@Available,ImageUrl=@ImageURL where ID="+cboRooms.SelectedValue;
                }
                else
                {
                    query= "Update Rooms set RoomNumber=@RoomNumber, NumberOfBeds=@NumberOfBeds, PricePerNight=@PricePerNight, RoomSize=@RoomSize, Available=@Available where ID="+cboRooms.SelectedValue;
                }
            }
            else
            {
                query = "Insert into Rooms (RoomNumber, NumberOfBeds, PricePerNight, RoomSize, Image, Available, ImageUrl) values (@RoomNumber, @NumberOfBeds, @PricePerNight, @RoomSize, @Image, @Available, @ImageURL)";
            }
            if (fileBytes != null || tbRoomNumber.Text != "" || tbNumberOfBeds.Text != "" || tbPricePerNight.Text != "" || tbRoomSize.Text != "")
            {
                SqlConnection cn = new SqlConnection(ConnectionString);

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@RoomNumber", tbRoomNumber.Text);
                cmd.Parameters.AddWithValue("@NumberOfBeds", Int32.Parse(tbNumberOfBeds.Text));
                cmd.Parameters.AddWithValue("@PricePerNight", Decimal.Parse(tbPricePerNight.Text));
                cmd.Parameters.AddWithValue("@RoomSize", Int32.Parse(tbRoomSize.Text));
                cmd.Parameters.AddWithValue("@Available", true);
                if (file != null)
                {
                    cmd.Parameters.AddWithValue("@Image", fileBytes);
                    cmd.Parameters.AddWithValue("@ImageURL", file.Path);
                }
                
                try
                {
                    cn.Open();
                    int count = cmd.ExecuteNonQuery();
                    if (count > 0)
                    {
                        MessageBox.DisplayDialog("OK","Daten erfolgreich gespeichert");
                    }
                    else
                    {
                        MessageBox.DisplayDialog("Fehler","Fehler beim einfügen der Daten");
                    }
                }
                catch (SqlException eSql)
                {
                    MessageBox.DisplayDialog("Fehler", eSql.Message);
                }
                finally
                {
                    cn.Close();
                }
            }
            else
            {
                MessageBox.DisplayDialog("Fehler","Alle Felder müssen ausgefüllt werden!");
            }
            
        }
        private async void BtnNewPicture_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".gif");
            file = await openPicker.PickSingleFileAsync();


            if (file != null)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                var image = new BitmapImage();
                image.SetSource(stream);
                imgRoom.Source = image;
                fileBytes = await BitmapImageConverter.ConvertImageToByte(file);

            }
        }
        private async Task<List<Rooms>> GetRooms()
        {
            string getUserQuery = "Select * from Rooms;";
            var rooms = new List<Rooms>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
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
                        SqlDataReader result = cmd.ExecuteReader();

                        while (result.Read())
                        {
                            var room = new Rooms
                            {
                                ID = result.GetInt32(0),
                                RoomNumber = result.GetString(1),
                                NumberOfBeds = result.GetInt32(2),

                                PricePerNight = Math.Round(result.GetDecimal(3), 2),
                                RoomSize = result.GetString(4),
                                ImageUrl = result.GetString(5),
                                Available = result.GetBoolean(6),

                                SourceImage = (byte[])result[7],


                            };


                            rooms.Add(room);
                            foreach (var item in rooms)
                            {
                                room.Image = await BitmapImageConverter.ConvertByteToImage(room.SourceImage);
                            }
                        }
                        return rooms;



                    }

                }


            }




            return null;
        }
        private void CboRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboRooms.SelectedIndex > -1)
            {
                
                imgRoom.Source = listRooms[cboRooms.SelectedIndex].Image;
                tbRoomNumber.Text = listRooms[cboRooms.SelectedIndex].RoomNumber;
                tbNumberOfBeds.Text = listRooms[cboRooms.SelectedIndex].NumberOfBeds.ToString();
                tbRoomSize.Text = listRooms[cboRooms.SelectedIndex].RoomSize.ToString();
                tbPricePerNight.Text = listRooms[cboRooms.SelectedIndex].PricePerNight.ToString();
            }
            else
            {
                imgRoom.Source = new BitmapImage(new Uri(@"https://plchldr.co/i/400x280?bg=83A8C3&fc=000000&text=Bild" +" des" + " Zimmers"));
                tbNumberOfBeds.Text = String.Empty;
                tbRoomNumber.Text = String.Empty;
                tbPricePerNight.Text = String.Empty;
                tbRoomSize.Text = String.Empty;
            }
        }
        private void BtnNewRoom_Click(object sender, RoutedEventArgs e)
        {
            cboRooms.SelectedIndex = -1;
        }
    }
}
