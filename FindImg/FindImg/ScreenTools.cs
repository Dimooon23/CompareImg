using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace FindImg
{
    public static class ScreenTools
    {
        public static void Screenshot()
        {

            try
            {
                int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                int screenHeight = Screen.PrimaryScreen.Bounds.Height;


                string path = "C:\\Captures\\";
                DateTime timeStamp = DateTime.Now;
                string timeNow = timeStamp.Hour.ToString("D2") + timeStamp.Minute.ToString("D2");


                System.IO.Directory.CreateDirectory(path);
                string fileName = $"capture{timeNow}.jpg";

                // Crée un objet Bitmap pour stocker l'image capturée.
                using (Bitmap captureBitmap = new Bitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb))
                {
                    // Crée un objet Graphics à partir du Bitmap, permettant de dessiner sur le Bitmap.
                    using (Graphics captureGraphics = Graphics.FromImage(captureBitmap))
                    {
                        // Copie les pixels de l'écran vers le Bitmap.
                        // Point(0,0) est le coin supérieur gauche de l'écran.
                        captureGraphics.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight));
                    }


                    captureBitmap.Save(path + fileName, ImageFormat.Jpeg);

                    Console.WriteLine($"Capture d'écran #{fileName} sauvegardée sous : {path}{fileName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la capture d'écran: {ex.Message}");
            }
        }
    }
}
