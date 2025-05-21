using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.IO;
using Emgu.CV.Util; 

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

        public enum MatchingMethodType
        {
            Sqdiff = 0,
            SqdiffNormed = 1,
            Ccorr = 2,
            CcorrNormed = 3,
            Ccoeff = 4,
            CcoeffNormed = 5
        }

        /// <summary>
        /// Détecte une image modèle dans un screenshot.
        /// </summary>
        /// <param name="templateImagePath">Le chemin complet vers l'image modèle à rechercher.</param>
        /// <param name="location">Un Point qui contiendra les coordonnées (X, Y) du coin supérieur gauche de la correspondance trouvée si l'image est détectée.</param>
        /// <param name="method">La méthode de correspondance à utiliser (par défaut : CcoeffNormed pour de bons résultats).</param>
        /// <returns>True si l'image a été trouvée et le seuil de correspondance est atteint, False sinon.</returns>
        public static bool DetectImage(string templateImagePath, out Point location, MatchingMethodType method = MatchingMethodType.CcoeffNormed)
        {
            location = Point.Empty; // Initialise la localisation à un point vide

            Console.WriteLine($"Démarrage de la détection d'image pour : {templateImagePath}");

            // 1. Prendre un screenshot
            Bitmap screenshotBitmap = Screenshot();
            if (screenshotBitmap == null)
            {
                Console.WriteLine("La capture d'écran a échoué. Impossible de procéder à la détection.");
                return false;
            }

            // 2. Charger l'image modèle
            if (!File.Exists(templateImagePath))
            {
                Console.WriteLine($"Erreur : Le fichier image modèle n'a pas été trouvé à : {templateImagePath}");
                screenshotBitmap.Dispose();
                return false;
            }
            Bitmap templateBitmap = null;
            try
            {
                templateBitmap = new Bitmap(templateImagePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement de l'image modèle '{templateImagePath}': {ex.Message}");
                screenshotBitmap.Dispose();
                return false;
            }

            // Déclarer les objets Mat en dehors du try/catch initial
            // pour garantir qu'ils sont à portée du finally pour Dispose().
            // Ils doivent être nullables ou initialisés pour la sécurité.
            Mat sceneImageMat = null;
            Mat templateImageMat = null;
            Mat result = null;

            try
            {
                // Convertir les Bitmap en objets Mat d'Emgu.CV
                // C'est la méthode recommandée pour une conversion fiable et directe.
                // Bitmap.ToImage<Bgr, byte>() crée un objet Image<Bgr, byte>,
                // et la propriété .Mat de cet objet est le Mat OpenCV souhaité.
                // Nous allons utiliser des using imbriqués pour gérer la durée de vie.
                sceneImageMat = (Image)screenshotBitmap;
                templateImageMat = (Image)templateBitmap;
                {
                    sceneImageMat = sceneImageEmgu.Mat;
                    templateImageMat = templateImageEmgu.Mat;

                    // S'assurer que l'image modèle n'est pas plus grande que l'image de scène
                    if (templateImageMat.Width > sceneImageMat.Width || templateImageMat.Height > sceneImageMat.Height)
                    {
                        Console.WriteLine("L'image modèle est plus grande que le screenshot. Impossible de procéder à la détection.");
                        return false;
                    }

                    // Créer un Mat pour stocker le résultat de la correspondance
                    int resultCols = sceneImageMat.Cols - templateImageMat.Cols + 1;
                    int resultRows = sceneImageMat.Rows - templateImageMat.Rows + 1;
                    result = new Mat(resultRows, resultCols, DepthType.Cv32F, 1);

                    Console.WriteLine($"Exécution de la correspondance de modèle avec la méthode : {method}...");

                    // Effectuer la correspondance de modèle
                    CvInvoke.MatchTemplate(sceneImageMat, templateImageMat, result, (TemplateMatchingType)method);

                    // Normaliser le résultat (facultatif mais recommandé pour certaines méthodes)
                    CvInvoke.Normalize(result, result, 0, 1, NormType.MinMax);

                    // Localiser le meilleur match avec minMaxLoc
                    double minVal = 0;
                    double maxVal = 0;
                    Point minLoc = new Point();
                    Point maxLoc = new Point();

                    CvInvoke.MinMaxLoc(result, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

                    // Pour SQDIFF et SQDIFF_NORMED, les meilleures correspondances sont les valeurs les plus basses.
                    // Pour toutes les autres méthodes, plus la valeur est élevée, mieux c'est.
                    if (method == MatchingMethodType.Sqdiff || method == MatchingMethodType.SqdiffNormed)
                    {
                        location = minLoc;
                        Console.WriteLine($"Meilleure correspondance (valeur minimale) : {minVal:F4} à X:{location.X}, Y:{location.Y}");
                        // Ajustez ce seuil selon vos besoins pour déterminer une correspondance "réussie"
                        if (minVal < 0.1) // Seuil plus bas = meilleure correspondance pour Sqdiff
                        {
                            Console.WriteLine("Correspondance significative trouvée !");
                            return true;
                        }
                    }
                    else
                    {
                        location = maxLoc;
                        Console.WriteLine($"Meilleure correspondance (valeur maximale) : {maxVal:F4} à X:{location.X}, Y:{location.Y}");
                        // Ajustez ce seuil selon vos besoins pour déterminer une correspondance "réussie"
                        if (maxVal > 0.8) // Seuil plus haut = meilleure correspondance pour Ccorr/Ccoeff
                        {
                            Console.WriteLine("Correspondance significative trouvée !");
                            return true;
                        }
                    }
                } // Fin des using (Image<Bgr, byte> ...)
                Console.WriteLine("Aucune correspondance significative trouvée. Le seuil de détection n'a pas été atteint.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur est survenue lors de la détection d'image : {ex.Message}");
                Console.WriteLine($"Détails : {ex.ToString()}");
                return false;
            }
            finally
            {
                // Libérer les ressources Bitmap dans tous les cas
                screenshotBitmap?.Dispose();
                templateBitmap?.Dispose();

                // Libérer les ressources Mat si elles ont été allouées
                // Attention: Les Mat créés par .ToImage() sont gérés par l'Image<TColor,TDepth> parente
                // Vous n'avez pas besoin de Dispose() explicitement les Mat s'ils proviennent directement de Image.Mat
                // MAIS si vous faites un .Clone() ou un new Mat(), il faut les disposer.
                // Dans ce code révisé, Image<TColor,TDepth> gère les Mat, donc pas de .Dispose() sur sceneImageMat/templateImageMat.
                // Seul le Mat 'result' doit être disposé car il est créé explicitement avec 'new Mat()'.
                result?.Dispose();
            }
        }
    }
}