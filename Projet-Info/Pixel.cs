using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_Info
{
    class Pixel
    {
        #region Attributs
        private int x; //position X du pixel
        private int y; //position Y du pixel
        private int red; //valeur rouge du pixel
        private int green; //valeur verte du pixel
        private int blue; //valeur bleue du pixel
        #endregion

        #region Constructeurs
        /// <summary>
        /// Constructeur de la classe Pixel
        /// </summary>
        /// <param name="x">position X du pixel</param>
        /// <param name="y">position Y du pixel</param>
        /// <param name="R">valeur rouge du pixel</param>
        /// <param name="G">valeur verte du pixel</param>
        /// <param name="B">valeur bleue du pixel</param>
        public Pixel(int x,int y,int R,int G,int B)
        {
            this.x = x;
            this.y = y;
            red = R;
            green = G;
            blue = B;
        }
        #endregion

        #region Propriétés
        public int X
        {
            get { return x; }
        }
        public int Y
        {
            get { return y; }
        }
        public int Red
        {
            get { return red; }
            set { red = value; }
        }
        public int Green
        {
            get { return green; }
            set { green = value; }
        }
        public int Blue
        {
            get { return blue; }
            set { blue = value; }
        }
        #endregion
    }
}
