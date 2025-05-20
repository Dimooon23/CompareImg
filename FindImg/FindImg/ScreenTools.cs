using System;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace FindImg
{
    public static class ScreenTools
    {
        public static Bitmap Screenshot()
        {
            Bitmap captureBitmap;

            int screenWidth = Convert.ToInt32(SystemParameters.FullPrimaryScreenWidth);
            int screenHeight = Convert.ToInt32(SystemParameters.FullPrimaryScreenHeight);

            // Crée un objet Bitmap pour stocker l'image capturée.
            captureBitmap = new Bitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb);

            try
            {
                // Crée un objet Graphics à partir du Bitmap, permettant de dessiner sur le Bitmap.
                Graphics captureGraphics = Graphics.FromImage(captureBitmap);
                    
                // Copie les pixels de l'écran vers le Bitmap.
                // Point(0,0) est le coin supérieur gauche de l'écran.
                captureGraphics.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight));
                    
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la capture d'écran: {ex.Message}");
            }

            return captureBitmap;
        }

        public static void detectImage()
        {


        }


    }
}
