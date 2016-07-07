using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qube
{
    class QubeBase
    {
        #region type
        public static Color[] ColorList =
            {
                Color.Purple,
                Color.Red,
                Color.Green,
                Color.Yellow,
                Color.White
            };

        public static readonly int
            MaxColorIndex = 3;

        private Color color = new Color();
        #endregion

        public QubeBase(Color color)
        {
            this.color = color;
        }

        public Color Type
        {
            get { return color; }
        }

        public void SetQube(Color color)
        {
            this.color = color;
        }
    }
}
