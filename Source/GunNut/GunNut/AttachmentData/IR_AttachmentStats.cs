using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunNut
{
    public struct IR_AttachmentStats
    {
        public float accuracyTouch;
        public float accuracyShort;
        public float accuracyMedium;
        public float accuracyLong;

        public float damageMult;
        public float warmupMult;
        public float cooldownMult;

        public float nightvision;
        public float infravision;
        public float zoomvision;

        public float silencerEffect;

        public int burstShotsOffset;
        public float fireRateMult;


    }
}
