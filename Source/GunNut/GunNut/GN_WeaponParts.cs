using Verse;

namespace GunNut
{
    [StaticConstructorOnStartup]
    public static class GN_WeaponParts
    {
        public enum WeaponPart
        {
            scope,
            magazine,
            stock,
            barrel,
            barrelExt,
            receiver,
            grip,
            foregrip,
            undermount,
            debug
        }
    }
}