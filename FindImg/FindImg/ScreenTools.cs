using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;


namespace FindImg
{
    public static class ScreenTools
    {
        public static Bitmap Screenshot()
        {
            int screenWidth = Convert.ToInt32(SystemParameters.FullPrimaryScreenWidth);
            int screenHeight = Convert.ToInt32(SystemParameters.FullPrimaryScreenHeight);

            Bitmap captureBitmap = null;

            try
            {
                captureBitmap = new Bitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb);
                using (Graphics captureGraphics = Graphics.FromImage(captureBitmap))
                {
                    captureGraphics.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la capture d'écran: {ex.Message}");
                return null;
            }

            return captureBitmap;
        }

        public static Point findTemplate(string templateImagePath)
        {
            //Point foundLocation = Point.Empty;

            //if (!File.Exists(templateImagePath))
            //{
            //    return Point.Empty;
            //}

            //Bitmap templateBitmap = null;
            //Bitmap screenBitmap = null;
            //Image<Gray, byte> templateImage = null;
            //Image<Gray, byte> modelImage = null;
            //Mat result = null;

            //double minVal = 0;
            //double maxVal = 0;
            //Point minLoc = new Point();
            //Point maxLoc = new Point();


            //try
            //{
            //    templateBitmap = new Bitmap(templateImagePath);
            //    //screenBitmap = Screenshot();
            //    screenBitmap = new Bitmap("../../pictures/template.jpg");

            //    templateImage = templateBitmap.ToImage<Gray, byte>();
            //    modelImage = screenBitmap.ToImage<Gray, byte>();


            //    result = new Mat(
            //        modelImage.Height - templateImage.Height + 1,
            //        modelImage.Width - templateImage.Width + 1,
            //        DepthType.Cv32F, 1);


            //    CvInvoke.MatchTemplate(modelImage, templateImage, result, TemplateMatchingType.CcoeffNormed);

            //    //Test
            //    CvInvoke.Normalize(result, result, 0, 255, NormType.MinMax, DepthType.Cv8U);
            //    CvInvoke.Imshow("Result Heatmap", result);
            //    CvInvoke.WaitKey(1);

            //    CvInvoke.MinMaxLoc(result, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

            //    if (maxVal >= 0.8)
            //    {
            //        foundLocation = maxLoc;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    foundLocation = Point.Empty;
            //    Console.WriteLine(ex.Message );
            //}
            //finally
            //{
            //    if (result != null) result.Dispose();
            //    if (modelImage != null) modelImage.Dispose();
            //    if (templateImage != null) templateImage.Dispose();
            //    if (screenBitmap != null) screenBitmap.Dispose();
            //    if (templateBitmap != null) templateBitmap.Dispose();
            //}

            //return foundLocation;



            Point foundLocation = Point.Empty;

            if (!File.Exists(templateImagePath))
            {
                return Point.Empty;
            }

            using (Bitmap templateBitmap = new Bitmap(templateImagePath))
            //using (Bitmap screenBitmap = new Bitmap("../../pictures/template.jpg"))
            using (Bitmap screenBitmap = Screenshot())
            {
                if (templateBitmap == null || screenBitmap == null)
                {
                    return Point.Empty;
                }

                if (templateBitmap.Width > screenBitmap.Width || templateBitmap.Height > screenBitmap.Height)
                {
                    return Point.Empty;
                }

                using (Image<Gray, byte> templateImage = templateBitmap.ToImage<Gray, byte>())
                using (Image<Gray, byte> modelImage = screenBitmap.ToImage<Gray, byte>())
                {
                    using (Mat result = new Mat(
                        modelImage.Height - templateImage.Height + 1,
                        modelImage.Width - templateImage.Width + 1,
                        DepthType.Cv32F, 1))
                    {
                        CvInvoke.MatchTemplate(modelImage, templateImage, result, TemplateMatchingType.CcoeffNormed);

                        //CvInvoke.Normalize(result, result, 0, 255, NormType.MinMax, DepthType.Cv8U);
                        //CvInvoke.Imshow("Result Heatmap", result);
                        //CvInvoke.WaitKey();

                        double minVal = 0;
                        double maxVal = 0;
                        Point minLoc = new Point();
                        Point maxLoc = new Point();

                        CvInvoke.MinMaxLoc(result, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

                        if (maxVal >= 0.8)
                        {
                            foundLocation = maxLoc;
                        }
                    }
                }
            }
            return foundLocation;

        }
    }
}