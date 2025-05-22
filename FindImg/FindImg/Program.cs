using System;
using System.Drawing;
using System.IO;
using System.Timers;

namespace FindImg
{
    internal class Program
    {

        private static Timer _detectionTimer;
        private static string imagePath = "../../pictures/kabane.jpg";

        static void Main(string[] args)
        {
            Console.WriteLine("Application de détection d'image démarrée.");
            Console.WriteLine($"Image modèle: {imagePath}");
            Console.WriteLine("Appuyez sur 'Q' pour quitter l'application.");


            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"Erreur: Le fichier image modèle '{imagePath}' est introuvable. Veuillez vérifier le chemin.");
                Console.WriteLine("Appuyez sur n'importe quelle touche pour quitter.");
                Console.ReadKey();
                return;
            }

            //ScreenTools.findTemplate(imagePath);

            _detectionTimer = new Timer();
            _detectionTimer.Interval = 500;
            _detectionTimer.Elapsed += OnTimedEvent;
            _detectionTimer.AutoReset = true;
            _detectionTimer.Enabled = true;

            while (Console.ReadKey(true).Key != ConsoleKey.Q)
            {
                // Boucle vide, attend la touche 'Q'
            }


            _detectionTimer.Stop();
            _detectionTimer.Dispose();
            Console.WriteLine("\nApplication terminée.");
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Point points = ScreenTools.findTemplate(imagePath);

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);

            if (points != Point.Empty)
            {
                Console.WriteLine($"Image trouvée à X: {points.X}, Y: {points.Y}");
            }
            else
            {
                Console.WriteLine("Image non trouvée.");
            }
        }
    }
}
