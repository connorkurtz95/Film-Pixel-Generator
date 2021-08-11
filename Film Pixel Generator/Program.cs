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
        public static string extension = "png";
        public static Random rand = new Random();
        static void Main()
        {
            LikeItsWinter();

            Console.WriteLine("done");
            Console.ReadLine();
        }
        static void LikeItsWinter()
        {
            string unprocessedFolder1 = "Clip1Unprocessed", processedFolder1 = "Clip1Processed", unprocessedFolder2 = "Clip2Unprocessed", processedFolder2 = "Clip2Processed", unprocessedFolder3 = "Clip3Unprocessed", processedFolder3 = "Clip3Processed",
                unprocessedFolder4 = "Clip4Unprocessed", processedFolder4 = "Clip4Processed", unprocessedFolder5 = "Clip5Unprocessed", processedFolder5 = "Clip5Processed", unprocessedFolder6 = "Clip6Unprocessed", processedFolder6 = "Clip6Processed",
                unprocessedFolder7 = "Clip7Unprocessed", processedFolder7 = "Clip7Processed", unprocessedFolder8 = "Clip8Unprocessed", processedFolder8 = "Clip8Processed", unprocessedFolder9 = "Clip9Unprocessed", processedFolder9 = "Clip9Processed";

            LikeItsWinterProcess(ProcessNames.Horizontal, unprocessedFolder1, processedFolder1, shiftValue: 1);
            LikeItsWinterProcess(ProcessNames.Horizontal, unprocessedFolder2, processedFolder2, shiftValue: 2);
            LikeItsWinterProcess(ProcessNames.Horizontal, unprocessedFolder3, processedFolder3, shiftValue: 4);
            LikeItsWinterProcess(ProcessNames.Horizontal, unprocessedFolder4, processedFolder4, shiftValue: 8);
            LikeItsWinterProcess(ProcessNames.Horizontal, unprocessedFolder5, processedFolder5, shiftValue: 16);
            LikeItsWinterProcess(ProcessNames.Horizontal, unprocessedFolder6, processedFolder6, shiftValue: 32);

            LikeItsWinterProcess(ProcessNames.Vertical, unprocessedFolder7, processedFolder7, shiftValue: 32);
            LikeItsWinterProcess(ProcessNames.Vertical, unprocessedFolder8, processedFolder8, shiftValue: 8);
            LikeItsWinterProcess(ProcessNames.Vertical, unprocessedFolder9, processedFolder9, shiftValue: 2);
        }

        static void LikeItsWinterProcess(string process, string unprocessedFolder, string processedFolder, double shiftChance = 0.5, int shiftValue = 1)
        {
            List<ImageReference> unprocessedImages = GetImages(unprocessedFolder);

            int count = 0;

            foreach (ImageReference imageReference in unprocessedImages)
            {
                count++;

                Image<Rgba32> image = imageReference.Load();

                if (process == ProcessNames.Horizontal)
                {
                    image = HorizontalLineProcess(image, shiftChance, shiftValue);
                }
                else if(process == ProcessNames.Vertical)
                {
                    image = VerticalLineProcess(image, shiftChance, shiftValue);
                }

                SaveImage(image, processedFolder, extension, count, unprocessedImages.Count);
            }
        }

        static List<ImageReference> GetImages(string folder, bool getBrightness = false)
        {
            string[] fileList = Directory.GetFiles(folder);

            List<ImageReference> images = new List<ImageReference>();

            int count = 1;

            foreach (string fileName in fileList)
            {
                Console.WriteLine("Getting " + fileName + " - " + count + " / " + fileList.Length);
                images.Add(new ImageReference(fileName, getBrightness));
                count++;
            }

            return images;
        }

        static Image<Rgba32> HorizontalLineProcess(Image<Rgba32> unprocessedImage, double shiftChance = 0.5, int shiftValue = 1)
        {
            for (int x = 0; x < unprocessedImage.Width - shiftValue * 2; x+= shiftValue)
            {
                if(rand.NextDouble() < shiftChance)
                {
                    for (int x2 = x; x2 <= x + shiftValue; x2++)
                    {
                        for (int y = 0; y < unprocessedImage.Height; y++)
                        {
                            Rgba32 passoverColour = unprocessedImage[x2, y];
                            unprocessedImage[x2, y] = unprocessedImage[x2 + shiftValue, y];
                            unprocessedImage[x2 + shiftValue, y] = passoverColour;
                        }
                    }
                }
            }

            return unprocessedImage;
        }

        static Image<Rgba32> VerticalLineProcess(Image<Rgba32> unprocessedImage, double shiftChance = 0.5, int shiftValue = 1)
        {
            for (int y = 0; y < unprocessedImage.Height - shiftValue * 2; y+= shiftValue)
            {
                if (rand.NextDouble() < shiftChance)
                {
                    for (int y2 = y; y2 <= y + shiftValue; y2++)
                    {
                        for (int x = 0; x < unprocessedImage.Width; x++)
                        {
                            Rgba32 passoverColour = unprocessedImage[x, y2];
                            unprocessedImage[x, y2] = unprocessedImage[x, y2 + shiftValue];
                            unprocessedImage[x, y2 + shiftValue] = passoverColour;
                        }
                    }
                }
            }

            return unprocessedImage;
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

        static void SaveImage(Image<Rgba32> image, string folder, string extension, int count, int total)
        {
            string location = folder + "\\" + folder + count + "." + extension;

            Console.WriteLine("Saving " + location + " - " + count + " / " + total);

            Directory.CreateDirectory(folder);
            image.Save(location);

            image.Dispose();
        }
    }

    class ImageReference
    {
        public string imageLocation;
        public long brightness;

        public ImageReference(string imageLocation, bool getBrightness = false)
        {
            this.imageLocation = imageLocation;

            if (getBrightness)
            {
                FindBrightness();
            }
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
    class ProcessNames
    {
        public static string Horizontal = "Horizontal";
        public static string Vertical = "Vertical";
    }
}
