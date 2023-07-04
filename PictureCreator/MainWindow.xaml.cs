using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PictureCreator
{
    public partial class MainWindow : Window
    {
        private List<string> imagePaths = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadImagesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filePath in openFileDialog.FileNames)
                {
                    imagePaths.Add(filePath);
                }

                // Display the loaded images
                DisplayImages();
            }
        }

        private async void ProcessImagesButton_Click(object sender, RoutedEventArgs e)
        {
            List<Task> imageTasks = new List<Task>();

            foreach (string imagePath in imagePaths)
            {
                Task task = Task.Run(() => ProcessImage(imagePath));
                imageTasks.Add(task);
            }

            await Task.WhenAll(imageTasks);

            MessageBox.Show("Image processing completed!");

            // Display the processed images
            DisplayImages();
        }

        private void ProcessImage(string imagePath)
        {
            // Simulating image processing
            Thread.Sleep(2000);

            // Apply filters/effects to the image
            ApplyFilter(imagePath, FilterType.Grayscale);
            // Apply more filters/effects as needed

            // Save the processed image
            string processedImagePath = System.IO.Path.GetFileNameWithoutExtension(imagePath) + "_processed" + System.IO.Path.GetExtension(imagePath);
            string destinationPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), processedImagePath);

            Application.Current.Dispatcher.Invoke(() =>
            {
                File.Copy(imagePath, destinationPath);
            });
        }

        private void ApplyFilter(string imagePath, FilterType filterType)
        {
            // Load the image
            BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));

            // Create an Image control
            Image imageControl = new Image();
            imageControl.Width = 200;
            imageControl.Height = 200;
            imageControl.Source = bitmapImage;

            // Apply the filter to the image
            ApplyFilter(imageControl, filterType);

            // Update the source image with the modified image
            bitmapImage = (BitmapImage)imageControl.Source;
            bitmapImage.Freeze();
        }

        private void ApplyFilter(Image image, FilterType filterType)
        {
            // Convert Image to Bitmap
            BitmapSource bitmapSource = (BitmapSource)image.Source;

            // Create WriteableBitmap to modify the image
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);

            // Apply the filter to the image
            switch (filterType)
            {
                case FilterType.Grayscale:
                    ApplyGrayscaleFilter(writeableBitmap);
                    break;
                case FilterType.Sepia:
                    ApplySepiaFilter(writeableBitmap);
                    break;
                    // Add more cases for other filters/effects
            }

            // Update the image source with the modified image
            image.Source = writeableBitmap;
        }

        private void ApplyGrayscaleFilter(WriteableBitmap writeableBitmap)
        {
            // Get the width, height, and stride of the image
            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = writeableBitmap.BackBufferStride;

            // Define the pixel format
            PixelFormat pixelFormat = PixelFormats.Bgra32;
            int bytesPerPixel = (pixelFormat.BitsPerPixel + 7) / 8;

            // Create a buffer to hold the pixel data
            byte[] pixelBuffer = new byte[height * stride];

            // Copy the pixel data from the WriteableBitmap to the buffer
            writeableBitmap.CopyPixels(pixelBuffer, stride, 0);

            // Iterate over each pixel and apply the grayscale filter
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Calculate the index of the current pixel
                    int index = y * stride + x * bytesPerPixel;

                    // Get the RGB values of the pixel
                    byte blue = pixelBuffer[index];
                    byte green = pixelBuffer[index + 1];
                    byte red = pixelBuffer[index + 2];

                    // Calculate the grayscale value
                    byte grayscale = (byte)((red + green + blue) / 3);

                    // Set the RGB values of the pixel to the grayscale value
                    pixelBuffer[index] = grayscale;
                    pixelBuffer[index + 1] = grayscale;
                    pixelBuffer[index + 2] = grayscale;
                }
            }

            // Copy the modified pixel data back to the WriteableBitmap
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelBuffer, stride, 0);
        }

        private void ApplySepiaFilter(WriteableBitmap writeableBitmap)
        {
            // Get the width, height, and stride of the image
            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = writeableBitmap.BackBufferStride;

            // Define the pixel format
            PixelFormat pixelFormat = PixelFormats.Bgra32;
            int bytesPerPixel = (pixelFormat.BitsPerPixel + 7) / 8;

            // Create a buffer to hold the pixel data
            byte[] pixelBuffer = new byte[height * stride];

            // Copy the pixel data from the WriteableBitmap to the buffer
            writeableBitmap.CopyPixels(pixelBuffer, stride, 0);

            // Iterate over each pixel and apply the sepia filter
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Calculate the index of the current pixel
                    int index = y * stride + x * bytesPerPixel;

                    // Get the RGB values of the pixel
                    byte blue = pixelBuffer[index];
                    byte green = pixelBuffer[index + 1];
                    byte red = pixelBuffer[index + 2];

                    // Calculate the sepia values
                    byte sepiaBlue = (byte)(0.272 * red + 0.534 * green + 0.131 * blue);
                    byte sepiaGreen = (byte)(0.349 * red + 0.686 * green + 0.168 * blue);
                    byte sepiaRed = (byte)(0.393 * red + 0.769 * green + 0.189 * blue);

                    // Set the RGB values of the pixel to the sepia values
                    pixelBuffer[index] = sepiaBlue;
                    pixelBuffer[index + 1] = sepiaGreen;
                    pixelBuffer[index + 2] = sepiaRed;
                }
            }

            // Copy the modified pixel data back to the WriteableBitmap
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelBuffer, stride, 0);
        }

        private enum FilterType
        {
            Grayscale,
            Sepia
            // Add more filter types as needed
        }

        private void DisplayImages()
        {
            ImagesStackPanel.Children.Clear();

            foreach (string imagePath in imagePaths)
            {
                Image imageControl = new Image();
                imageControl.Width = 200;
                imageControl.Height = 200;
                imageControl.Source = new BitmapImage(new Uri(imagePath));

                ImagesStackPanel.Children.Add(imageControl);
            }
        }
    }
}
