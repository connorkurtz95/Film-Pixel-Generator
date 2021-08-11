using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Drawing;

namespace FilmPixelGenerator
{
    class Program
    {
        public static string extension = "png", unsortedFolder = "raw3", sortedFolder = "unraw3";
        static void Main()
        {
            List<ImageReference> unsortedImages = GetImages(unsortedFolder);//test
            List<ImageReference> sortedImages = SortImagesByBrightness(unsortedImages);

            SaveImages(sortedImages, sortedFolder, "sorted_image");

            Console.WriteLine("done");
            Console.ReadLine();
        }

        static List<ImageReference> GetImages(string folder)
        {
            string[] fileList = Directory.GetFiles(folder);

            List<ImageReference> images = new List<ImageReference>();

            int count = 1;

            foreach (string fileName in fileList)
            {
                Console.WriteLine("Processing " + fileName + " - " + count + " / " + fileList.Length);
                images.Add(new ImageReference(fileName));
                count++;
            }

            return images;
        }

        static List<ImageReference> SortImagesByBrightness(List<ImageReference> unsortedImages)
        {
            Console.WriteLine("Sorting Images");

            List<ImageReference> sortedImages = unsortedImages.OrderBy(image => image.brightness).ToList();

            return sortedImages;
        }

        static void SaveImages (List<ImageReference> images, string folder, string fileName, bool includeBlackFrames = false)
        {

            Directory.CreateDirectory(folder);

            int count = 1, blackCount = 0;

            foreach(ImageReference image in images)
            {
                if (includeBlackFrames || image.brightness > 0)
                {
                    Console.WriteLine("Saving " + fileName + " - " + count + " / " + images.Count() + " - Brightness: " + image.brightness);

                    image.Save(folder, fileName + count, extension);
                }
                else
                {
                    Console.WriteLine(fileName + " not saved");

                    blackCount++;
                }

                count++;
            }

            if(!includeBlackFrames)
            {
                Console.WriteLine(blackCount + " black frames ignored");
            }
        }
    }

    class ImageReference
    {
        public string imageLocation;
        public long brightness;

        public ImageReference(string imageLocation)
        {
            this.imageLocation = imageLocation;

            FindBrightness();
        }

        public void FindBrightness()
        {
            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(imageLocation))
            {
                brightness = 0;

                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        brightness += image[x, y].R + image[x, y].G + image[x, y].B;
                    }
                }

                image.Dispose();
            }
        }
        public void Save(string folder, string fileName, string extension)
        {
            Image<Rgba32> image = Flip(Load());

            string location = folder + "\\" + fileName + "." + extension;

            Console.WriteLine("Saving " + location);

            image.Save(location);

            image.Dispose();
        }

        public Image<Rgba32> Load()
        {
            Console.WriteLine("Loading " + imageLocation);
            return SixLabors.ImageSharp.Image.Load(imageLocation);
        }

        public Image<Rgba32> Flip(Image<Rgba32> image)
        {
            if (image.MetaData.ExifProfile != null && image.MetaData.ExifProfile.Values[3].ToString() != "Horizontal (normal)")
            {
                Console.WriteLine("Flipping " + imageLocation);
                Image<Rgba32> flippedImage = new Image<Rgba32>(image.Width, image.Height);

                for (int x = 0; x < flippedImage.Width; x++)
                {
                    for (int y = 0; y < flippedImage.Height; y++)
                    {
                        flippedImage[x, y] = image[x, y];
                    }
                }

                image = flippedImage;

                flippedImage.Dispose();// - uncomment if problems occur
            }

            return image;
        }
    }

    //class ImageReference
    //{
    //    public string imageLocation;
    //    public long brightness;

    //    public ImageReference(string imageLocation)
    //    {
    //        this.imageLocation = imageLocation;

    //        FindBrightness();
    //    }

    //    public void FindBrightness()
    //    {
    //        using (var image = new Bitmap(System.Drawing.Image.FromFile(imageLocation)))
    //        {
    //            brightness = 0;

    //            for (int x = 0; x < image.Width; x++)
    //            {
    //                for (int y = 0; y < image.Height; y++)
    //                {
    //                    brightness += image.GetPixel(x, y).R + image.GetPixel(x, y).G + image.GetPixel(x, y).B;
    //                }
    //            }

    //            image.Dispose();
    //        }
    //    }
    //    public void Save(string folder, string fileName, string extension)
    //    {
    //        string location = folder + "/" + fileName + "." + extension;

    //        Console.WriteLine("Saving " + location);

    //        using (var image = new Bitmap(System.Drawing.Image.FromFile(imageLocation)))
    //        {
    //            image.Save(location);

    //            image.Dispose();
    //        }
    //    }
    //}
}
