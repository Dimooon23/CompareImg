using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace FindImg
{
    internal class ScreenTool
    {
        private static int numberOfCaptures = 0;
        private static int timeStepInSeconds;

        public static void Screenshot()
        {
            int screenWidth = 1920; // Remplace par ta valeur
        int screenHeight = 1080;
        int numberOfCaptures = 0;
        string timeNow = DateTime.Now.ToString("yyyyMMddHHmmss");
        string path = "C:\\Captures\\";
        Directory.CreateDirectory(path);
        string fileName = $"capture{numberOfCaptures.ToString("D2")}{timeNow}.jpg";

        try
        {
            using (Bitmap captureBitmap = new Bitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb))
            {
                using (Graphics captureGraphics = Graphics.FromImage(captureBitmap))
                {
                    captureGraphics.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight));
                }

                captureBitmap.Save(path + fileName, ImageFormat.Jpeg);
                Console.WriteLine($"Capture d'écran sauvegardée sous : {path}{fileName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la capture d'écran: {ex.Message}");
        }
        }
    }
}
