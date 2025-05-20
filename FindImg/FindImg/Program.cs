using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindImg
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DateTime timeStamp = DateTime.Now;

            Bitmap screenShot = ScreenTools.Screenshot();

            Console.WriteLine(screenShot.ToString());
            try
            {
                screenShot.Save("C:\\Captures\\capture" + timeStamp.Hour.ToString("D2") + timeStamp.Minute.ToString("D2") + ".jpg", ImageFormat.Jpeg);
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}
