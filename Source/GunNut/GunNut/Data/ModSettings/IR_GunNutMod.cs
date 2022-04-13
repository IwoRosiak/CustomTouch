using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace GunNut
{
    internal class IR_GunNutMod : Mod
    {
        private IR_Settings settings;

        private IR_SectionDrawer_Coordinator drawerCoordinator;

        
        public IR_GunNutMod(ModContentPack content) : base(content)
        {
            LongEventHandler.ExecuteWhenFinished(() => { 
                
                settings = GetSettings<IR_Settings>();
                IR_Settings.Initialise();
                
                });

            //this.settings = GetSettings<IR_Settings>();

        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            if (drawerCoordinator == null)
            {
                drawerCoordinator = new IR_SectionDrawer_Coordinator(null);
            }

            drawerCoordinator.Draw(inRect);
        }         
        


        public override string SettingsCategory()
        {
            return "Custom Touch";
        }

    }
}