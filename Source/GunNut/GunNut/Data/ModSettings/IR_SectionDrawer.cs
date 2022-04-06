using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GunNut
{
    internal abstract class IR_SectionDrawer
    {
        protected const int buttonHeight = 40;
        protected const int buttonWidth = 180;
        protected const int mediumButtonWidth = 120;
        protected const int smallButtonWidth = 90;
        protected const int tinyButtonWidth = 45;
        protected const int weaponScale = 3;

        protected const int sliderWidth = 16;

        protected IR_SectionDrawer_Coordinator parent;

        protected Rect myRect;

        public IR_SectionDrawer(IR_SectionDrawer_Coordinator _parent)
        {
            parent = _parent;
        }

        public abstract void Draw(Rect rect);
    }
}
