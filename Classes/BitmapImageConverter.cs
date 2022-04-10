using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace BookingApp.Classes
{
    public class BitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            byte[] imageBytes = (byte[])value;
            return ConvertByteToImage(imageBytes).Result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Convert byte[] to image
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> ConvertByteToImage(byte[] imageBytes)
        {
            BitmapImage image = new BitmapImage();
            using (InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(randomAccessStream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(imageBytes);
                    await writer.StoreAsync();
                    await image.SetSourceAsync(randomAccessStream);
                }

            }
            return image;
        }


        public static async Task<byte[]> ConvertImageToByte(StorageFile file)
        {
            using (var inputStream = await file.OpenSequentialReadAsync())
            {
                var readStream = inputStream.AsStreamForRead();

                var byteArray = new byte[readStream.Length];
                await readStream.ReadAsync(byteArray, 0, byteArray.Length);
                return byteArray;
            }

        }
    }
}
