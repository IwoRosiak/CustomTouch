using GunNut;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace CustomTouch
{
    public class IR_SlotsCords : IExposable
    {
        public List<IR_AttachmentTag> weaponTags;

        public Dictionary<IR_AttachmentType, Vector2> position;
        public Dictionary<IR_AttachmentType, bool> isEnabled;

        public Dictionary<IR_AttachmentType, bool> isFront;
        public Dictionary<IR_AttachmentType, float> size;

        public Dictionary<IR_AttachmentType, Rect> masks;

        public Vector2 GetPosition(IR_AttachmentType part)
        {
            return position[part]/10;
        }

        public void SetPosition(IR_AttachmentType part, Vector2 pos)
        {
            position[part] = pos*10;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref weaponTags, "weaponTags", LookMode.Def);
            Scribe_Collections.Look(ref position, "defaultPlacement", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref isEnabled, "defaultActive", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref isFront, "isFront", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref size, "size", LookMode.Value, LookMode.Value);


            if (Scribe.mode == LoadSaveMode.Saving)
            {
                Dictionary<IR_AttachmentType, string> masksString = new Dictionary<IR_AttachmentType, string>(); 

                foreach (var pair in masks)
                {
                    masksString.Add(pair.Key,  "(" + pair.Value.x.ToString() + "," + pair.Value.y.ToString() + "," + pair.Value.width.ToString() + "," + pair.Value.height + ")");;
                }

                Scribe_Collections.Look<IR_AttachmentType, string>(ref masksString, "masksString", LookMode.Value, LookMode.Value);
            }

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Dictionary<IR_AttachmentType, string> masksString = new Dictionary<IR_AttachmentType, string>();

                Scribe_Collections.Look<IR_AttachmentType, string>(ref masksString, "masksString", LookMode.Value, LookMode.Value);


                masks = new Dictionary<IR_AttachmentType, Rect>();

                foreach (var pair in masksString)
                {
                    masks.Add(pair.Key, ParseHelper.FromStringRect(pair.Value));
                    Log.Message(pair.Value);
                }
            }


        }


    }
}