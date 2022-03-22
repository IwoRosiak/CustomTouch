using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace GunNut
{
    [StaticConstructorOnStartup]
    public static class IR_Init
    {
        static IR_Init()
        {
            IR_Settings.Initialise();
        }
    }
}