using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Projet_Info
{
    class Program
    {
        /// <summary>
        /// Fonction de test fournie par le professeur
        /// </summary>
        /// <param name="file">tableau d'octet contenant l'image</param>
        static void Test(byte[] file)
        {
            Console.WriteLine("entête\n\n");
            for (int i = 0; i < 14; i++)
            {
                Console.Write(file[i] + " ");
            }
            Console.WriteLine("\n\nInfo\n\n");
            for (int i = 14; i < 54; i++)
            {
                Console.Write(file[i] + " ");
            }
            Console.WriteLine("\n\nIMAGE\n\n");
            for (int i = 54; i < file.Length; i++)
            {
                Console.Write(file[i] + "\t");
            }
            Console.WriteLine();
        }

        static int SaisieNombre()
        {
            int result;
            while (!int.TryParse(Console.ReadLine(), out result))
            { }
            return result;
        }

        /// <summary>
        /// Fonction Menu générale
        /// </summary>
        static void Menu()
        {
            //texte d'introduction
            Console.WriteLine("bienvenue dans ce programe de traitement d'image, que souhaitez vous faire?"
                + "\n\t1)Transformer une image en nuance gris"
                + "\n\t2)Transformer une image en noir et blanc"
                + "\n\t3)Appliquer une matrice de convolution à une image"
                + "\n\t4)Créer une image décrivant un cercle"
                + "\n\t5)Voir nos projets innovants");
            bool flagswitch = true; //flag pour les exo
            int exo = SaisieNombre();
            while (flagswitch)
            {
                switch (exo)
                {
                    case 1:
                        NuanceGris(); //menu nuance de gris
                        flagswitch = false;
                        break;
                    case 2:
                        NoirBlanc(); //menu noir et blanc
                        flagswitch = false;
                        break;
                    case 3:
                        Convolution(); //menu convolution
                        flagswitch = false;
                        break;
                    case 4:
                        Cercle(); //menu cercle
                        flagswitch = false;
                        break;
                    case 5:
                        Innovation(); //menu innovation
                        flagswitch = false;
                        break;
                    default: //pour toutes les autres réponses
                        Console.WriteLine("Je n'ai pas compris ce que vous vouliez faire, veuillez recommancer");
                        exo = SaisieNombre();
                        break;
                }
            }
        }

        /// <summary>
        /// Fonction menu pour la méthode nuance de gris
        /// </summary>
        static void NuanceGris()
        {
            Console.WriteLine("Veuillez entrez le chemin d'acces au ficher à modifier (attention ne pas oublier l'extension .bmp)"
                + "\nSi le chemin n'est pas absolu, le répertoire de départ est le même que celui du .exe");
            Image support = new Image(Console.ReadLine());
            Image Nuance = support.Gris();
            byte[] retour = Nuance.ToByteArray();
            if (retour != null)
            {
                Console.WriteLine("Sous quel chemin (et donc nom) voullez vous enregistrer votre nouvelle image (attention si le ficher existe déjà il sera réécrie)");
                File.WriteAllBytes(Console.ReadLine(), Nuance.ToByteArray());
            }
            else Console.WriteLine("ERREUR FICHIER NULL");
            Console.WriteLine("Oppération effectuer, passer une bonne journée");
        }

        /// <summary>
        /// Fonction menu pour la méthode noir et blanc
        /// </summary>
        static void NoirBlanc()
        {
            Console.WriteLine("Veuillez entrez le chemin d'acces au ficher à modifier (attention ne pas oublier l'extension .bmp)"
                + "\nSi le chemin n'est pas absolu, le répertoire de départ est le même que celui du .exe");
            Image support = new Image(Console.ReadLine());
            Image Noirblanc = support.NoirBlanc();
            byte[] retour = Noirblanc.ToByteArray();
            if (retour != null)
            {
                Console.WriteLine("Sous quel chemin (et donc nom) voullez vous enregistrer votre nouvelle image (attention si le ficher existe déjà il sera réécrie)");
                File.WriteAllBytes(Console.ReadLine(), Noirblanc.ToByteArray());
            }
            else Console.WriteLine("ERREUR FICHIER NULL");
            File.WriteAllBytes(Console.ReadLine(), Noirblanc.ToByteArray());
            Console.WriteLine("Oppération effectuer, passer une bonne journée");
        }

        /// <summary>
        /// Fonction menu pour la méthode Convolution
        /// </summary>
        static void Convolution()
        {
            Console.WriteLine("Veuillez entrez le chemin d'acces au ficher à modifier (attention ne pas oublier l'extension .bmp)"
                + "\nSi le chemin n'est pas absolu, le répertoire de départ est le même que celui du .exe");
            Image support = new Image(Console.ReadLine());
            int[,] conv = new int[3, 3];
            Console.WriteLine("veuillez entrer les facteurs de la matrices de convolution en partant de en haut à gauche");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    conv[i, j] = SaisieNombre();
                }
            }
            Console.WriteLine("Entrez le rapport de division pour la matrice de convolution (attention certaines valeurs peuvent dépaser 256 suivant ce rapport)");
            int rap = Convert.ToInt32(Console.ReadLine());
            Image Convo = support.Convolution(conv, rap);
            byte[] retour = Convo.ToByteArray();
            if (retour != null)
            {
                Console.WriteLine("Sous quel chemin (et donc nom) voullez vous enregistrer votre nouvelle image (attention si le ficher existe déjà il sera réécrie)");
                File.WriteAllBytes(Console.ReadLine(), Convo.ToByteArray());
            }
            else Console.WriteLine("ERREUR FICHIER NULL");
            File.WriteAllBytes(Console.ReadLine(), Convo.ToByteArray());
            Console.WriteLine("Oppération effectuer, passer une bonne journée");
        }

        /// <summary>
        /// Fonction menu pour la méthode Cercle
        /// </summary>
        static void Cercle()
        {
            Console.WriteLine("veuillez entrer la hauteur de votre image");
            int h = SaisieNombre();
            Console.WriteLine("veuillez entrer la largeure de votre image");
            int l = SaisieNombre();
            Console.WriteLine("veuillez entrer l'absice de votre cercle");
            int posx = SaisieNombre();
            Console.WriteLine("veuillez entrer l'ordonné de votre cercle");
            int posy = SaisieNombre();
            Console.WriteLine("Veuillez entrer le rayon de votre cercle");
            int r = SaisieNombre();
            Image cercle = Image.Cercle("cercle", h, l, posx, posy, r);
            Console.WriteLine("Sous quel chemin (et donc nom) voullez vous enregistrer votre nouvelle image (attention si le ficher existe déjà il sera réécrie)");
            File.WriteAllBytes(Console.ReadLine(), cercle.ToByteArray());
            Console.WriteLine("Oppération effectuer, passer une bonne journée");
        }

        /// <summary>
        /// Fonction menu pour les innovations
        /// </summary>
        static void Innovation()
        {
            Console.WriteLine("bienvenue dans la partie Innovation de ce programme, que souhaitez vous faire?"
                + "\n\t1)Transformer une image en négatif"
                + "\n\t2)Produire une palette de couleur"
                + "\n\t3)Produire une fractale de Julia"
                + "\n\t4)Créer une rosace"
                + "\n\t5)Créer une image Yin et Yang");
            bool flagswitch = true;
            int exo = SaisieNombre();
            while (flagswitch)
            {
                switch (exo)
                {
                    case 1:
                        Negatif();
                        flagswitch = false;
                        break;
                    case 2:
                        Palette();
                        flagswitch = false;
                        break;
                    case 3:
                        Julia();
                        flagswitch = false;
                        break;
                    case 4:
                        Rosace();
                        flagswitch = false;
                        break;
                    case 5:
                        YinYang();
                        flagswitch = false;
                        break;
                    default:
                        Console.WriteLine("Je n'ai pas compris ce que vous vouliez faire, veuillez recommancer");
                        exo = SaisieNombre();
                        break;
                }
            }
        }

        /// <summary>
        /// Fonction menu pour le Négatif
        /// </summary>
        static void Negatif()
        {
            Console.WriteLine("Veuillez entrez le chemin d'acces au ficher à modifier (attention ne pas oublier l'extension .bmp)"
                + "\nSi le chemin n'est pas absolu, le répertoire de départ est le même que celui du .exe");
            Image support = new Image(Console.ReadLine());
            Image Nega = support.Negatif();
            byte[] retour = Nega.ToByteArray();
            if (retour != null)
            {
                Console.WriteLine("Sous quel chemin (et donc nom) voullez vous enregistrer votre nouvelle image (attention si le ficher existe déjà il sera réécrie)");
                File.WriteAllBytes(Console.ReadLine(), Nega.ToByteArray());
            }
            else Console.WriteLine("ERREUR FICHIER NULL");
            File.WriteAllBytes(Console.ReadLine(), Nega.ToByteArray());
            Console.WriteLine("Oppération effectuer, passer une bonne journée");
        }

        /// <summary>
        /// Fonction menu pour les Rosaces
        /// </summary>
        static void Rosace()
        {
            Console.WriteLine("Combien de rosace voulez vous?");
            Image rose = Image.Rosace(SaisieNombre());
            Console.WriteLine("Sous quel chemin (et donc nom) voullez vous enregistrer votre nouvelle image (attention si le ficher existe déjà il sera réécrie)");
            File.WriteAllBytes(Console.ReadLine(), rose.ToByteArray());
            Console.WriteLine("Oppération effectuer, passer une bonne journée");
        }

        /// <summary>
        /// Fonction menu pour le Yin-Yang
        /// </summary>
        static void YinYang()
        {
            Image yinyang = Image.YinYang();
            Console.WriteLine("Sous quel chemin (et donc nom) voullez vous enregistrer votre nouvelle image (attention si le ficher existe déjà il sera réécrie)");
            File.WriteAllBytes(Console.ReadLine(), yinyang.ToByteArray());
            Console.WriteLine("Oppération effectuer, passer une bonne journée");
        }

        /// <summary>
        /// Fonction menu pour la Palette de couleur
        /// </summary>
        static void Palette()
        {
            Image palette = Image.Palette();
            Console.WriteLine("Sous quel chemin (et donc nom) voullez vous enregistrer votre nouvelle image (attention si le ficher existe déjà il sera réécrie)");
            File.WriteAllBytes(Console.ReadLine(), palette.ToByteArray());
            Console.WriteLine("Oppération effectuer, passer une bonne journée");
        }

        /// <summary>
        /// 
        /// </summary>
        static void Julia()
        {
            Console.WriteLine("Quel précision voulez vous ? (plus le nombre est petit plus la fractale sur complexe)?");
            Image julia = Image.Julia(SaisieNombre());
            Console.WriteLine("Sous quel chemin (et donc nom) voullez vous enregistrer votre nouvelle image (attention si le ficher existe déjà il sera réécrie)");
            File.WriteAllBytes(Console.ReadLine(), julia.ToByteArray());
            Console.WriteLine("Oppération effectuer, passer une bonne journée");
        }

        /// <summary>
        /// Fonction Pricipale du programe
        /// </summary>
        /// <param name="args">détermine l'ordre d'execusion si plusieur fonctions main sont active</param>
        static void Main(string[] args)
        {
            ConsoleKeyInfo cki; //déclare une variable de type ConsoleKeyInfo
            do
            {
                Console.Clear();
                Menu();
                Console.WriteLine("echape pour quitter");
                cki = Console.ReadKey();
            } while (cki.Key != ConsoleKey.Escape);//teste si la touche est différente de la touche Escape
            /*Image J = new Image("JF.bmp");
            int[,] a = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
            for (int i = 10; i < 5000000; i += 10)
            {
                J = J.Convolution(a);
            }*/
            //File.WriteAllBytes("CC.bmp", Image.Spiral().ToByteArray());
            //Image.TestAngle();
            /*int[] pts1 = { 100, 100 };
            int[] pts2 = { 200, 400 };
            int[] pts3 = { 500, 500 };
            File.WriteAllBytes("test.bmp", Image.triangle(pts1, pts2, pts3).ToByteArray());*/
            Console.Write("done");
        }
    }
}
