using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Projet_Info
{
    class Image
    {
        #region Attributs //commentés
        private string type = "bmp"; //type du fichier (par défaut bmp)
        private string name; //nom du fichier
        private int hauteur; //hauteur de l'image
        private int largeur; //largeure de l'image
        private long taille; //taille totale du fichier (image+padding+header)
        private long offSet; //taille de l'image seule
        private int nbsOctCouleur = 3; //nombre d'octet utilisé par pixel
        private Pixel[,] matriceImage; //matrice de pixel correspondant à l'image
        private int pading; //valeur du padding
        private static byte[] defaultHeader = { 66, 77, 230, 4, 0, 0, 0, 0, 0, 0, 54, 0, 0, 0, //header basique servant à la création d'image
            40, 0, 0, 0, 20, 0, 0, 0, 20, 0, 0, 0, 1, 0, 24, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private byte[] header = new byte[54]; //header vide, sera remplie par celui de l'image
        private bool utile = true;

        #endregion

        #region Static //commentées
        /// <summary>
        /// méthode de conversion du little endian aux entiers
        /// </summary>
        /// <param name="file">Tableau d'octet contenant les donnés</param>
        /// <param name="index">Index de départ de la conversion</param>
        /// <param name="taille">Nombre d'octet à convertir</param>
        /// <returns>La valeur entière correspondante</returns>
        public static int ConvertEndToInt(byte[] file, int index, int taille)
        {
            double memo = 0; //variable mémoire
            for (int i = 0; i < taille; i++)
            {
                memo += file[index + i] * Math.Pow(256, i); //calcul de la valeur
            }
            return Convert.ToInt32(memo); //Math.Pow renvoyant des double transformation obligée
        }

        /// <summary>
        /// méthode de conversion d'octet à carractère
        /// </summary>
        /// <param name="b">octet à convertir</param>
        /// <returns>le caractère correspondant</returns>
        public static char ConvertByteToChar(byte b)
        {
            return Convert.ToChar(b);
        }

        /// <summary>
        /// méthode de conversion d'entier à little endian (4 octet)
        /// </summary>
        /// <param name="a">valeur entière à convertir</param>
        /// <returns>tableau d'octet correspondant en little endian</returns>
        public static byte[] ConvertIntToByte(long a)
        {
            byte[] retour = new byte[4]; //initialise le tableau de retour
            for (int i = 0; i < 4; i++)
            {
                retour[i] = Convert.ToByte(a % 256); //calcul de la valeur i du tablaeu
                a = (a - retour[i]) / 256;
            }
            return retour;
        }

        /// <summary>
        /// permet de créer une image avec un cercle
        /// </summary>
        /// <param name="nom">nom du fichier créer</param>
        /// <param name="haut">hauteur de l'image</param>
        /// <param name="larg">largeur de l'image</param>
        /// <param name="posX">position X du centre du cercle</param>
        /// <param name="posY">position Y du centre du cercle</param>
        /// <param name="radius">Rayon du cercle</param>
        /// <returns>Image de taille hauteur largeur avec un cercle</returns>
        public static Image Cercle(string nom, int haut, int larg, int posX, int posY, int radius)
        {
            Image Retour = new Image(nom, haut, larg); //Création de l'image support
            bool flag = false; //flag de sécurité
            if (radius < 0) { Console.WriteLine("ERREUR RADIUS"); flag = true; } //test de sécurité
            if (flag) return Retour; //return de sécurité
            for (int i = 0; i < haut; i++)
            {
                for (int j = 0; j < larg; j++)
                {
                    double R = Math.Sqrt(Math.Pow((i - posX), 2) + Math.Pow((j - posY), 2)); //calcul de la distance au centre
                    if (R <= radius) //si le point appartient au disque
                    {
                        Retour.Matrice[i, j].Red = 0; //colorisation du Pixel
                        Retour.Matrice[i, j].Green = 128;
                        Retour.Matrice[i, j].Blue = 255;
                    }
                }
            }
            return Retour; //renvoie l'image modifié
        }

        /// <summary>
        /// Première version de la rosace
        /// </summary>
        /// <param name="nbs">Nombre de rosace à superposé</param>
        /// <returns>l'image avec les rosaces</returns>
        public static Image RosaceBeta(int nbs)
        {
            Image retour = new Image("retour", 1000, 1000); //Création de l'image support
            //Création d'un disque blanc sur fond noir
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    double R = Math.Sqrt(Math.Pow((i - 500), 2) + Math.Pow((j - 500), 2));
                    if (R <= 495)
                    {
                        retour.Matrice[i, j].Red = 0;
                        retour.Matrice[i, j].Green = 0;
                        retour.Matrice[i, j].Blue = 0;
                    }
                }
            }
            retour = retour.Negatif();

            //écriture des rosaces
            for (int k = 1; k <= nbs; k++)
            {
                for (int i = 0; i < 1000; i++)
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        double R1 = Math.Sqrt(Math.Pow(i - (500 + 500 * Math.Sin((1 * Math.PI / 3) + (Math.PI / 3) / k)), 2) + Math.Pow(j - (500 + 500 * Math.Cos((1 * Math.PI / 3) + (Math.PI / 3) / k)), 2)); //calcule de la distance aux diférent centre + phase entre les rosaces
                        double R2 = Math.Sqrt(Math.Pow(i - (500 + 500 * Math.Sin((2 * Math.PI / 3) + (Math.PI / 3) / k)), 2) + Math.Pow(j - (500 + 500 * Math.Cos((2 * Math.PI / 3) + (Math.PI / 3) / k)), 2));
                        double R3 = Math.Sqrt(Math.Pow(i - (500 + 500 * Math.Sin((3 * Math.PI / 3) + (Math.PI / 3) / k)), 2) + Math.Pow(j - (500 + 500 * Math.Cos((3 * Math.PI / 3) + (Math.PI / 3) / k)), 2));
                        double R4 = Math.Sqrt(Math.Pow(i - (500 + 500 * Math.Sin((4 * Math.PI / 3) + (Math.PI / 3) / k)), 2) + Math.Pow(j - (500 + 500 * Math.Cos((4 * Math.PI / 3) + (Math.PI / 3) / k)), 2));
                        double R5 = Math.Sqrt(Math.Pow(i - (500 + 500 * Math.Sin((5 * Math.PI / 3) + (Math.PI / 3) / k)), 2) + Math.Pow(j - (500 + 500 * Math.Cos((5 * Math.PI / 3) + (Math.PI / 3) / k)), 2));
                        double R6 = Math.Sqrt(Math.Pow(i - (500 + 500 * Math.Sin((6 * Math.PI / 3) + (Math.PI / 3) / k)), 2) + Math.Pow(j - (500 + 500 * Math.Cos((6 * Math.PI / 3) + (Math.PI / 3) / k)), 2));
                        if (retour.Matrice[i, j].Red != 0 && retour.Matrice[i, j].Green != 0 && retour.Matrice[i, j].Blue != 0) //si le Pixel n'est pas noir (permet de n'écrire que sur le disque blanc)
                        {
                            if ((495 <= R1 && R1 <= 500) || (495 <= R2 && R2 <= 500) || (495 <= R3 && R3 <= 500) || (495 <= R4 && R4 <= 500) || (495 <= R5 && R5 <= 500) || (495 <= R6 && R6 <= 500)) //si le pixel appartient à l'un des arcs de cercle
                            {
                                switch (k) //suivant l'itération de la rosace modification des couleurs
                                {
                                    case 1:
                                        retour.Matrice[i, j].Red = 0;
                                        break;
                                    case 2:
                                        retour.Matrice[i, j].Green = 0;
                                        break;
                                    case 3:
                                        retour.Matrice[i, j].Blue = 0;
                                        break;
                                    case 4:
                                        retour.Matrice[i, j].Red = 0;
                                        retour.Matrice[i, j].Green = 0;
                                        break;
                                    case 5:
                                        retour.Matrice[i, j].Red = 0;
                                        retour.Matrice[i, j].Blue = 0;
                                        break;
                                    case 6:
                                        retour.Matrice[i, j].Green = 0;
                                        retour.Matrice[i, j].Blue = 0;
                                        break;
                                    default: //default de sécurité
                                        retour.Matrice[i, j].Red = 0;
                                        retour.Matrice[i, j].Green = 0;
                                        retour.Matrice[i, j].Blue = 0;
                                        break;
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("itération " + k); //indiquateur de progression
            }
            return retour; //renvoie l'image 
        }

        /// <summary>
        /// Version finale de la méthode pour créer des rosaces
        /// </summary>
        /// <param name="nbs">Nombre de rosace à superposé</param>
        /// <returns>l'image avec les rosaces</returns>
        public static Image Rosace(int nbs)
        {
            Image retour = new Image("retour", 1000, 1000); //Création del'image support
            //création d'un fond noir
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    double R = Math.Sqrt(Math.Pow((i - 500), 2) + Math.Pow((j - 500), 2));
                    if (R >= 495)
                    {
                        retour.Matrice[i, j].Red = 0;
                        retour.Matrice[i, j].Green = 0;
                        retour.Matrice[i, j].Blue = 0;
                    }
                }
            }

            //écriture des rosaces
            for (int k = 1; k <= nbs; k++)
            {
                for (int i = 0; i < 1000; i++)
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        if (retour.Matrice[i, j].Red != 0 && retour.Matrice[i, j].Green != 0 && retour.Matrice[i, j].Blue != 0) //si le pixel n'est pas noir
                        {
                            double R1 = Math.Sqrt(Math.Pow(i - (500 + 500 * Math.Sin((1 * Math.PI / 3) + ((nbs - k) * Math.PI / 3) / nbs)), 2) + Math.Pow(j - (500 + 500 * Math.Cos((1 * Math.PI / 3) + ((nbs - k) * Math.PI / 3) / nbs)), 2)); //calcule de la distance aux diférent centre + phase entre les rosaces
                            double R2 = Math.Sqrt(Math.Pow(i - (500 + 500 * Math.Sin((2 * Math.PI / 3) + ((nbs - k) * Math.PI / 3) / nbs)), 2) + Math.Pow(j - (500 + 500 * Math.Cos((2 * Math.PI / 3) + ((nbs - k) * Math.PI / 3) / nbs)), 2));
                            double R3 = Math.Sqrt(Math.Pow(i - (500 + 500 * Math.Sin((3 * Math.PI / 3) + ((nbs - k) * Math.PI / 3) / nbs)), 2) + Math.Pow(j - (500 + 500 * Math.Cos((3 * Math.PI / 3) + ((nbs - k) * Math.PI / 3) / nbs)), 2));
                            double R4 = Math.Sqrt(Math.Pow(i - (500 + 500 * Math.Sin((4 * Math.PI / 3) + ((nbs - k) * Math.PI / 3) / nbs)), 2) + Math.Pow(j - (500 + 500 * Math.Cos((4 * Math.PI / 3) + ((nbs - k) * Math.PI / 3) / nbs)), 2));
                            double R5 = Math.Sqrt(Math.Pow(i - (500 + 500 * Math.Sin((5 * Math.PI / 3) + ((nbs - k) * Math.PI / 3) / nbs)), 2) + Math.Pow(j - (500 + 500 * Math.Cos((5 * Math.PI / 3) + ((nbs - k) * Math.PI / 3) / nbs)), 2));
                            double R6 = Math.Sqrt(Math.Pow(i - (500 + 500 * Math.Sin((6 * Math.PI / 3) + ((nbs - k) * Math.PI / 3) / nbs)), 2) + Math.Pow(j - (500 + 500 * Math.Cos((6 * Math.PI / 3) + ((nbs - k) * Math.PI / 3) / nbs)), 2));

                            if ((495 <= R1 && R1 <= 500) || (495 <= R2 && R2 <= 500) || (495 <= R3 && R3 <= 500) || (495 <= R4 && R4 <= 500) || (495 <= R5 && R5 <= 500) || (495 <= R6 && R6 <= 500)) //si le pixel appartient à l'un des arcs de cercle
                            {
                                switch (k % 12) //suivant l'itération en cours modification de la couleur
                                {
                                    case 0:
                                        retour.Matrice[i, j].Red = 0;
                                        break;
                                    case 1:
                                        retour.Matrice[i, j].Green = 0;
                                        break;
                                    case 2:
                                        retour.Matrice[i, j].Blue = 0;
                                        break;
                                    case 3:
                                        retour.Matrice[i, j].Red = 0;
                                        retour.Matrice[i, j].Green = 0;
                                        break;
                                    case 4:
                                        retour.Matrice[i, j].Red = 0;
                                        retour.Matrice[i, j].Blue = 0;
                                        break;
                                    case 5:
                                        retour.Matrice[i, j].Green = 0;
                                        retour.Matrice[i, j].Blue = 0;
                                        break;
                                    case 6:
                                        retour.Matrice[i, j].Red = 255;
                                        retour.Matrice[i, j].Green = 128;
                                        retour.Matrice[i, j].Blue = 0;
                                        break;
                                    case 7:
                                        retour.Matrice[i, j].Red = 255;
                                        retour.Matrice[i, j].Green = 0;
                                        retour.Matrice[i, j].Blue = 128;
                                        break;
                                    case 8:
                                        retour.Matrice[i, j].Red = 128;
                                        retour.Matrice[i, j].Green = 255;
                                        retour.Matrice[i, j].Blue = 0;
                                        break;
                                    case 9:
                                        retour.Matrice[i, j].Red = 0;
                                        retour.Matrice[i, j].Green = 255;
                                        retour.Matrice[i, j].Blue = 128;
                                        break;
                                    case 10:
                                        retour.Matrice[i, j].Red = 128;
                                        retour.Matrice[i, j].Green = 0;
                                        retour.Matrice[i, j].Blue = 255;
                                        break;
                                    case 11:
                                        retour.Matrice[i, j].Red = 0;
                                        retour.Matrice[i, j].Green = 128;
                                        retour.Matrice[i, j].Blue = 255;
                                        break;
                                    default: //default de sécurité
                                        retour.Matrice[i, j].Red = 0;
                                        retour.Matrice[i, j].Green = 0;
                                        retour.Matrice[i, j].Blue = 0;
                                        break;
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("itération " + k); //indiquateur de progression
            }
            return retour; //renvoie l'image 
        }

        /// <summary>
        /// Permet de créer un yin-yang
        /// </summary>
        /// <returns>Une image avec un yin-yang</returns>
        public static Image YinYang()
        {
            Image retour = new Image("retour", 1000, 1000); //Création de l'image support
            //création d'un fond gris uniforme
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    double R = Math.Sqrt(Math.Pow((i - 500), 2) + Math.Pow((j - 500), 2));
                    if (R > 500)
                    {
                        retour.Matrice[i, j].Red = 128;
                        retour.Matrice[i, j].Green = 128;
                        retour.Matrice[i, j].Blue = 128;
                    }
                }
            }
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    if (retour.Matrice[i, j].Red != 128) //si le pixel n'est pas gris
                    {
                        double Rn = Math.Sqrt(Math.Pow((i - 500), 2) + Math.Pow((j - 250), 2)); //calcul de la dstance au centre "noir"
                        double Rb = Math.Sqrt(Math.Pow((i - 500), 2) + Math.Pow((j - 750), 2)); //calcul de la dstance au centre "blanc"
                        //ATTENTION L'ORDRE DES IF NE DOIS PAS ÊTRE MODIFIE
                        if (i <= 500) //parti supérieur blanche
                        {
                            retour.Matrice[i, j].Red = 255;
                            retour.Matrice[i, j].Green = 255;
                            retour.Matrice[i, j].Blue = 255;
                        }
                        if (Rn <= 250) //grand cercle noir
                        {
                            retour.Matrice[i, j].Red = 0;
                            retour.Matrice[i, j].Green = 0;
                            retour.Matrice[i, j].Blue = 0;
                        }
                        if (i > 500) //parti inférieur noir
                        {
                            retour.Matrice[i, j].Red = 0;
                            retour.Matrice[i, j].Green = 0;
                            retour.Matrice[i, j].Blue = 0;
                        }
                        if (Rb <= 250) //grand cercle blanc
                        {
                            retour.Matrice[i, j].Red = 255;
                            retour.Matrice[i, j].Green = 255;
                            retour.Matrice[i, j].Blue = 255;
                        }
                        if (Rn < 75) //petit cercle blanc
                        {
                            retour.Matrice[i, j].Red = 255;
                            retour.Matrice[i, j].Green = 255;
                            retour.Matrice[i, j].Blue = 255;
                        }
                        if (Rb < 75) //petit cercle blanc
                        {
                            retour.Matrice[i, j].Red = 0;
                            retour.Matrice[i, j].Green = 0;
                            retour.Matrice[i, j].Blue = 0;
                        }
                    }
                }
            }
            return retour; //renvoie l'image
        }

        /// <summary>
        /// Permet de créer une palette de couleur
        /// </summary>
        /// <returns>Une image de palette</returns>
        public static Image Palette()
        {
            Image retour = new Image("retour", 256, 256); //Création de l'image support
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    retour.Matrice[i, j].Red = 255 - (i + j) / 2; //coloration des pixels
                    retour.Matrice[i, j].Blue = j;
                    retour.Matrice[i, j].Green = i;
                }
            }
            return retour; //renvoie la nouvelle image
        }

        /// <summary>
        /// Permet de créer une image repésentant une fractale de Julia
        /// </summary>
        /// <param name="ite_max">Nombre d'itération maximale, détermine la complxité de l'image final</param>
        /// <returns>L'image représentant la fractale de Julia</returns>
        public static Image Julia(int ite_max)
        {
            double x1 = -1, x2 = 1, zoom = 1000;
            double y1 = -1.2, y2 = 1.2;

            Image retour = new Image("retour_Julia", Convert.ToInt32((x2 - x1) * zoom), Convert.ToInt32((y2 - y1) * zoom));
            for (int i = 0; i < retour.Hauteur; i++)
            {
                for (int j = 0; j < retour.Largeur; j++)
                {
                    retour.Matrice[i, j] = new Pixel(i, j, 0, 0, 0);
                }
            }
            //retour = retour.Negatif();

            for (int i = 0; i < retour.Hauteur; i++)
            {
                for (int j = 0; j < retour.Largeur; j++)
                {
                    double c_r = 0.285, c_i = 0.01, Z_r = i / zoom + x1, Z_i = j / zoom + y1;
                    int ite = 0;
                    do
                    {
                        double tmp = Z_r;
                        Z_r = (Z_r * Z_r) - (Z_i * Z_i) + c_r;
                        Z_i = (2 * Z_i * tmp) + c_i;
                        ite++;
                    } while (((Z_r * Z_r) - (Z_i * Z_i)) < 4 && ite < ite_max);
                    if (ite == ite_max)
                    {
                        retour.Matrice[i, j].Red = 0;
                        retour.Matrice[i, j].Blue = 0;
                        retour.Matrice[i, j].Green = 0;
                    }
                    else
                    {
                        retour.Matrice[i, j].Red = ite * 0 / ite_max;
                        retour.Matrice[i, j].Blue = ite * 255 / ite_max;
                        retour.Matrice[i, j].Green = ite * 255 / ite_max;
                    }
                }
            }
            return retour;
        }

        public static Image Coeur()
        {
            Image retour = new Image("retour", 100, 100);
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    double x2 = Math.Pow(i - 50, 2);
                    double r3x2 = Math.Pow(x2, 1.0 / 3.0);
                    double y = j - 50;
                    double y2r3x2 = Math.Pow(y - r3x2, 2);
                    double R = x2 + y2r3x2;
                    double f = Math.Pow(i - 500, 2) + Math.Pow((j - 500) - Math.Pow(Math.Pow(i - 500, 2), 1 / 3), 2);
                    if (R <= 1000.0 && 950.0 <= R) retour.Matrice[i, j].Red = 0;
                }
            }
            return retour;
        }

        public static Image Spiral()//doesn't work yet
        {
            Image retour = new Image("retour_spi", 1000, 1000);
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    double R = Math.Sqrt(Math.Pow((i - 500), 2) + Math.Pow((j - 500), 2));
                    double r = R % 12;
                    double angle;
                    if (i == 750 && j == 433)
                    {; }
                    if (i < 500)
                    {
                        angle = Math.Asin(Convert.ToDouble(500 - i) / R);
                    }
                    else
                    {
                        angle = Math.Asin(Convert.ToDouble(500 - i) / R);
                    }
                    if (/*0 > r || r > 10 &&*/ angle <= Math.PI / 6.0)
                    {
                        retour.Matrice[i, j].Red = 0;
                    }

                }
            }
            return retour;
        }

        public static void TestAngle()
        {
            Image test = new Image("test", 100, 100);
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    double R = Math.Sqrt(Math.Pow((i - 50), 2) + Math.Pow((j - 50), 2));
                    if (i > 50 + 50 * Math.Cos(Math.PI / 3.0))
                    {
                        test.Matrice[i, j].Blue = 0;
                    }
                    if (i / R < 1)
                    {
                        test.Matrice[i, j].Green = 0;
                    }
                    if (j / R < 1)
                    {
                        test.Matrice[i, j].Red = 0;
                    }
                }
            }

            File.WriteAllBytes("testspi.bmp", test.ToByteArray());
        }

        static double[] equationDroite(int[] coord1, int[] coord2) //coordsous forme {j,i}
        {
            double[] facteur = { 1, 0, 0 };//{y,x,c}

            if (coord1[0] == coord2[0])
            {
                facteur[0] = 0;
                facteur[1] = -1;
                facteur[2] = coord1[0];
            }
            else
            {
                facteur[1] = Convert.ToDouble((coord1[1] - coord2[1])) / (coord1[0] - coord2[0]);
                facteur[2] = coord1[1] - facteur[1] * coord1[0];
            }
            return facteur;
        }

        public static Image triangle(int[] pts1, int[] pts2, int[] pts3)
        {
            Image renvoi = new Image("renvoie", 1000, 1000);
            double[] equa1 = equationDroite(pts1, pts2);
            double[] equa2 = equationDroite(pts1, pts3);
            double[] equa3 = equationDroite(pts2, pts3);
            for (int i = 0; i < renvoi.Largeur; i++)
            {
                for (int j = 0; j < renvoi.Largeur; j++)
                {
                    if (equa1[0] * i - equa1[1] * j - equa1[2] > 0)
                    {
                        renvoi.Matrice[i, j].Red = 0;
                    }
                    /*else
                    {
                        renvoi.Matrice[i, j].Red = 255;
                    }*/
                    else if (equa2[0] * i - equa2[1] * j - equa2[2] < 0)
                    {
                        renvoi.Matrice[i, j].Green = 0;
                    }
                    /*else
                    {
                        renvoi.Matrice[i, j].Red = 255;
                    }*/
                    else if (equa3[0] * i - equa3[1] * j - equa3[2] < 0)
                    {
                        renvoi.Matrice[i, j].Blue = 0;
                    }
                    else
                    {
                        renvoi.Matrice[i, j].Red = 255;
                        renvoi.Matrice[i, j].Green = 255;
                        renvoi.Matrice[i, j].Blue = 255;
                    }
                    /*renvoi.Matrice[i, j] = new Pixel(i, j, 255, 255, 255);
                    if (equa3[0] * i - equa3[1] * j - equa3[2] < 0 && equa2[0] * i - equa2[1] * j - equa2[2] > 0 && equa1[0] * i - equa1[1] * j - equa1[2] > 0)
                    {
                        renvoi.Matrice[i, j].Red = 0;
                    }*/
                    
                }
            }
            return renvoi;
        }

        #endregion

        #region Constructeurs //all done+commentaires
        /// <summary>
        /// Constructeur de la class Image à partir d'un fichier
        /// </summary>
        /// <param name="path">Chemin d'accès, relatif ou absolu, à l'image voulant être chargée</param>
        public Image(string path)//working
        {
            name = path;
            byte[] data = new byte[0];
            bool flag = true;
            try
            {
                data = File.ReadAllBytes(path); //récupération des données de l'image
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("ERREUR FICHIER NON TROUVE");
                flag = false;
                utile = false;
            }


            if (flag)
            {
                taille = data.Length; //récupération de la taille de l'image
                hauteur = ConvertEndToInt(data, 22, 4); //calcule de la hauteur
                largeur = ConvertEndToInt(data, 18, 4); //calcule de la largeure
                offSet = hauteur * largeur; //calucale offset
                if ((largeur * 3) % 4 != 0) { pading = 4 - ((largeur * 3) % 4); }//calcule du padding
                else pading = 0;
                matriceImage = new Pixel[hauteur, largeur]; //initialisation de la matrice
                int ind = 54;
                for (int i = 0; i < 54; i++)
                {
                    header[i] = data[i]; //initialisation du header
                }
                for (int i = hauteur - 1; 0 <= i; i--)
                {
                    for (int j = 0; j < largeur; j++)
                    {
                        matriceImage[i, j] = new Pixel(i, j, data[ind + 2], data[ind + 1], data[ind]); //création des pixel dans la matriceImage
                        ind += nbsOctCouleur;
                    }
                    ind += pading; //ajout du padding de fin de ligne
                }
            }
        }
        /// <summary>
        /// Construteur de la Classe Image à partir de valeur numérique
        /// </summary>
        /// <param name="Nom">Nom du nouveua ficher</param>
        /// <param name="Haut">Hauteur de la nouvelle image</param>
        /// <param name="Larg">Largeure de la nouvelle image</param>
        public Image(string Nom, int Haut, int Larg)//working
        {
            name = Nom;
            hauteur = Haut;
            largeur = Larg;
            if ((Larg * 3) % 4 != 0) { pading = 4 - ((Larg * 3) % 4); } //calcule du padding
            else pading = 0;
            offSet = Haut * Larg; //calcule de l'offset
            taille = Haut * Larg * 3 + 54 + Haut * pading; //calcule de la taille totale du ficher
            matriceImage = new Pixel[Haut, Larg]; //initialisation de la matrice de Pixel
            for (int i = 0; i < 54; i++)
            {
                header[i] = defaultHeader[i]; //initialisation du header du ficher à l'aide de celui de base
            }
            for (int i = 0; i < 4; i++)
            {
                header[2 + i] = ConvertIntToByte(taille)[i]; //remplace la taille du ficher dans le header
                header[22 + i] = ConvertIntToByte(Haut)[i]; //remplace la hauteur dans le header
                header[18 + i] = ConvertIntToByte(Larg)[i]; //remplace la largeure dans le header
            }
            for (int i = 0; i < Haut; i++)
            {
                for (int j = 0; j < Larg; j++)
                {
                    matriceImage[i, j] = new Pixel(i, j, 255, 255, 255); //initialise les Pixels de la matriceImage en blanc
                }
            }

        }
        #endregion

        #region Propriétés //all done
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Hauteur
        {
            get { return hauteur; }
            set { hauteur = value; }
        }
        public int Largeur
        {
            get { return largeur; }
            set { largeur = value; }
        }
        public long Taille
        {
            get { return taille; }
            set { taille = value; }
        }
        public long OffSet
        {
            get { return offSet; }
            set { offSet = value; }
        }
        public int NbsOctCouleur
        {
            get { return nbsOctCouleur; }
            set { nbsOctCouleur = value; }
        }
        public Pixel[,] Matrice
        {
            get { return matriceImage; }
            set { matriceImage = value; }
        }
        public byte[] Header
        {
            get { return header; }
            set { header = value; }
        }
        public bool Utile
        {
            get { return utile; }
            set { utile = value; }
        }
        #endregion 

        #region Méthodes //commentées
        /// <summary>
        /// Transforme une instance de la classe Image en tableau d'octet
        /// </summary>
        /// <returns>Le tableau d'entier correspondant à l'image</returns>
        public byte[] ToByteArray()//working
        {
            if (utile)
            {
                byte[] retour = new byte[taille]; //création d'un tableau vide
                for (int i = 0; i < 54; i++)
                {
                    retour[i] = header[i]; //remplisage du header
                }
                int ind = 54;
                for (int i = hauteur - 1; 0 <= i; i--)
                {
                    for (int j = 0; j < largeur; j++)
                    {
                        retour[ind] = Convert.ToByte(matriceImage[i, j].Blue); ind++; //remplie les valeurs bleues de l'image
                        retour[ind] = Convert.ToByte(matriceImage[i, j].Green); ind++; //remplie les valeurs vertes de l'image
                        retour[ind] = Convert.ToByte(matriceImage[i, j].Red); ind++; //remplie les valeurs rouges de l'image
                    }
                    ind += pading; //saute les octet de padding
                }
                return retour; //renvoie le tableau 
            }
            else
            {
                Console.WriteLine("ERREUR IMAGE NON DEFINIE");
                return null;
            }
        }

        /// <summary>
        /// Transforme une image en nuance de gris
        /// </summary>
        /// <returns>La nouvelle image en nuance de gris</returns>
        public Image Gris()//working
        {
            Image retour = new Image(this.name + "_Gris", this.hauteur, this.largeur); //crée une nouvelle Image ayant la même hauteur et largeur
            for (int i = 0; i < 54; i++)
            {
                retour.Header[i] = header[i]; //initialise le header de l'image retour
            }
            retour.Utile = this.utile;
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    int moyenne = (matriceImage[i, j].Red + matriceImage[i, j].Green + matriceImage[i, j].Blue) / 3; //calcule la moyenne de couleur par pixel
                    retour.Matrice[i, j].Red = moyenne; //injecte la valeur dans les pixel de la matrice de l'image retour
                    retour.Matrice[i, j].Green = moyenne;
                    retour.Matrice[i, j].Blue = moyenne;

                }
            }
            return retour; //renvoie l'image en nuance de gris
        }

        /// <summary>
        /// Transforme une image en noire et blanc
        /// </summary>
        /// <returns>La nouvelle image en noire et blanc</returns>
        public Image NoirBlanc()
        {
            Image retour = new Image(this.name + "_NoirBlanc", this.hauteur, this.largeur); //crée une nouvelle Image ayant la même hauteur et largeur
            for (int i = 0; i < 54; i++)
            {
                retour.Header[i] = header[i]; //initialise le header de l'image retour
            }
            retour.Utile = this.utile;
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    int moyenne = (matriceImage[i, j].Red + matriceImage[i, j].Green + matriceImage[i, j].Blue) / 3; //calcule la moyenne de couleur par pixel
                    if (moyenne < 128) //test si la moyenne est inférieure à 128 (càd 256/2)
                    {
                        retour.Matrice[i, j].Red = 0; //injecte des pixel noir dans la matrice de l'image retour
                        retour.Matrice[i, j].Green = 0;
                        retour.Matrice[i, j].Blue = 0;
                    }
                    else //sinon
                    {
                        retour.Matrice[i, j].Red = 255; //injecte des pixel blanc dans la matrice de l'image retour
                        retour.Matrice[i, j].Green = 255;
                        retour.Matrice[i, j].Blue = 255;
                    }
                }
            }
            return retour; //renvoie l'image en noire et blanc
        }

        /// <summary>
        /// Transforme une image en son négatif 
        /// </summary>
        /// <returns>La nouvelle image en négatif</returns>
        public Image Negatif() //working
        {
            Image retour = new Image(this.name + "_Nega", this.hauteur, this.largeur); //crée une nouvelle Image ayant la même hauteur et largeur
            for (int i = 0; i < 54; i++)
            {
                retour.Header[i] = header[i]; //initialise le header de l'image retour
            }
            retour.Utile = this.utile;
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    retour.Matrice[i, j].Red = 255 - this.matriceImage[i, j].Red; //injecte les pixel ayant les valeur inverse
                    retour.Matrice[i, j].Green = 255 - this.matriceImage[i, j].Green;
                    retour.Matrice[i, j].Blue = 255 - this.matriceImage[i, j].Blue;

                }
            }
            return retour; //renvoie l'image en négatif
        }

        #region Voisin 
        /// <summary>
        /// Renvoie le voisin Nord d'un pixel
        /// </summary>
        /// <param name="current">Correspond au pixel de référence</param>
        /// <returns>le voisin Nord (au desus)</returns>
        public Pixel VoisinNord(Pixel current)
        {
            if (current.X == 0) return new Pixel(0, 0, 0, 0, 0); //création d'un surbord noir autours de l'image
            return matriceImage[current.X - 1, current.Y]; //renvoie le pixel nord
        }

        /// <summary>
        /// Renvoie le voisin Nord-Est d'un pixel
        /// </summary>
        /// <param name="current">Correspond au pixel de référence</param>
        /// <returns>le voisin Nord-Est (coin supérieur droit)</returns>
        public Pixel VoisinNordEst(Pixel current)
        {
            if (current.X == 0 || current.Y == largeur - 1) return new Pixel(0, 0, 0, 0, 0); //création d'un surbord noir autours de l'image
            return matriceImage[current.X - 1, current.Y + 1];//renvoie le pixel nord-est
        }

        /// <summary>
        /// Renvoie le voisin Est d'un pixel
        /// </summary>
        /// <param name="current">Correspond au pixel de référence</param>
        /// <returns>le voisin Est (à droite)</returns>
        public Pixel VoisinEst(Pixel current)
        {
            if (current.Y == largeur - 1) return new Pixel(0, 0, 0, 0, 0); //création d'un surbord noir autours de l'image
            return matriceImage[current.X, current.Y + 1];//renvoie le pixel est
        }

        /// <summary>
        /// Renvoie le voisin Sud-Est d'un pixel
        /// </summary>
        /// <param name="current">Correspond au pixel de référence</param>
        /// <returns>le voisin Sud-Est (coin inférieur droit)</returns>
        public Pixel VoisinSudEst(Pixel current)
        {
            if (current.X == hauteur - 1 || current.Y == largeur - 1) return new Pixel(0, 0, 0, 0, 0); //création d'un surbord noir autours de l'image
            return matriceImage[current.X + 1, current.Y + 1];//renvoie le pixel sud-est
        }

        /// <summary>
        /// Renvoie le voisin Sud d'un pixel
        /// </summary>
        /// <param name="current">Correspond au pixel de référence</param>
        /// <returns>le voisin Sud (en dessous)</returns>
        public Pixel VoisinSud(Pixel current)
        {
            if (current.X == hauteur - 1) return new Pixel(0, 0, 0, 0, 0); //création d'un surbord noir autours de l'image
            return matriceImage[current.X + 1, current.Y];//renvoie le pixel sud
        }

        /// <summary>
        /// Renvoie le voisin Sud-Ouest d'un pixel
        /// </summary>
        /// <param name="current">Correspond au pixel de référence</param>
        /// <returns>le voisin Sud-Ouest(coin inférieure gauche)</returns>
        public Pixel VoisinSudOuest(Pixel current)
        {
            if (current.X == hauteur - 1 || current.Y == 0) return new Pixel(0, 0, 0, 0, 0); //création d'un surbord noir autours de l'image
            return matriceImage[current.X + 1, current.Y - 1];//renvoie le pixel sud-ouest
        }

        /// <summary>
        /// Renvoie le voisin Ouest d'un pixel
        /// </summary>
        /// <param name="current">Correspond au pixel de référence</param>
        /// <returns>le voisin Ouest (à gauche)</returns>
        public Pixel VoisinOuest(Pixel current)
        {
            if (current.Y == 0) return new Pixel(0, 0, 0, 0, 0); //création d'un surbord noir autours de l'image
            return matriceImage[current.X, current.Y - 1];//renvoie le pixel ouest
        }

        /// <summary>
        /// Renvoie le voisin Nord-Ouest d'un pixel
        /// </summary>
        /// <param name="current">Correspond au pixel de référence</param>
        /// <returns>le voisin Nord-Ouest (coin supérieur gauche)</returns>
        public Pixel VoisinNordOuest(Pixel current)
        {
            if (current.X == 0 || current.Y == 0) return new Pixel(0, 0, 0, 0, 0); //création d'un surbord noir autours de l'image
            return matriceImage[current.X - 1, current.Y - 1];//renvoie le pixel nord-ouest
        }
        #endregion

        /// <summary>
        /// Applique une matrice de convolution à l'image
        /// </summary>
        /// <param name="convolution">Matrice de Convolution</param>
        /// <param name="rapport">Rapport de division des résultat obtenue (par défaut à 9)</param>
        /// <returns>L'image ayant subie une Convolution</returns>
        public Image Convolution(int[,] convolution, int rapport = 9)
        {
            Image Retour = new Image(this.name + "_Convolution", this.hauteur, this.largeur); //crée une nouvelle Image ayant la même hauteur et largeur
            for (int i = 0; i < 54; i++)
            {
                Retour.header[i] = header[i]; //initialise le header de l'image retour
            }
            Retour.Utile = this.utile;
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    Pixel N = VoisinNord(matriceImage[i, j]); //aquisition des voisin du pixel actuel
                    Pixel NE = VoisinNordEst(matriceImage[i, j]);
                    Pixel E = VoisinEst(matriceImage[i, j]);
                    Pixel SE = VoisinSudEst(matriceImage[i, j]);
                    Pixel S = VoisinSud(matriceImage[i, j]);
                    Pixel SO = VoisinSudOuest(matriceImage[i, j]);
                    Pixel O = VoisinOuest(matriceImage[i, j]);
                    Pixel NO = VoisinNordOuest(matriceImage[i, j]);
                    int rouge = (convolution[0, 0] * NO.Red) + (convolution[0, 1] * N.Red) + (convolution[0, 2] * NE.Red)
                        + (convolution[1, 0] * O.Red) + (convolution[1, 1] * matriceImage[i, j].Red) + (convolution[1, 2] * E.Red)
                        + (convolution[2, 0] * SO.Red) + (convolution[2, 1] * S.Red) + (convolution[2, 2] * SE.Red); //calule de la valeur rouge du pixel Convolué
                    int vert = (convolution[0, 0] * NO.Green) + (convolution[0, 1] * N.Green) + (convolution[0, 2] * NE.Green)
                        + (convolution[1, 0] * O.Green) + (convolution[1, 1] * matriceImage[i, j].Green) + (convolution[1, 2] * E.Green)
                        + (convolution[2, 0] * SO.Green) + (convolution[2, 1] * S.Green) + (convolution[2, 2] * SE.Green); //calule de la valeur verte du pixel Convolué
                    int bleu = (convolution[0, 0] * NO.Blue) + (convolution[0, 1] * N.Blue) + (convolution[0, 2] * NE.Blue)
                        + (convolution[1, 0] * O.Blue) + (convolution[1, 1] * matriceImage[i, j].Blue) + (convolution[1, 2] * E.Blue)
                        + (convolution[2, 0] * SO.Blue) + (convolution[2, 1] * S.Blue) + (convolution[2, 2] * SE.Blue); //calule de la bleue rouge du pixel Convolué
                    Retour.Matrice[i, j].Red = Math.Abs(rouge / rapport); //apliquation de la valeur divisé par le rapport (la valeur absolue empéche les valuers négatives)
                    Retour.Matrice[i, j].Green = Math.Abs(vert / rapport);
                    Retour.Matrice[i, j].Blue = Math.Abs(bleu / rapport);
                }
            }
            return Retour; //Renvoie l'image Convoluée
        }

        #endregion
    }
}


