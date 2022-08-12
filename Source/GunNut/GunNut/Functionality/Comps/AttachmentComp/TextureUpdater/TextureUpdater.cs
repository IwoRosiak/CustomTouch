using GunNut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomTouch.Functionality.Comps.AttachmentComp.TextureUpdater
{
    internal class TextureUpdater
    {
        public TextureUpdater(Material mat)
        {
            _mat = mat;
        }

        Material _mat;

        Texture2D _processedTexture;

        public Texture2D ProcessedTexture => _processedTexture;

        public void StartTextureProcessing(Texture texture)
        {
            Texture mainTexture = texture;

            int width = mainTexture.width;
            int width2 = mainTexture.width;

            _processedTexture = new Texture2D(width, width2, TextureFormat.ARGB32, false);

            RenderTexture renderTexture = new RenderTexture(width, width2, 0, (RenderTextureFormat)0, 1);
            Graphics.Blit(mainTexture, renderTexture, _mat, 0);
            RenderTexture.active = renderTexture;

            _processedTexture.ReadPixels(new Rect(0f, 0f, (float)width, (float)width2), 0, 0, false);
        }

        public void UpdateTextureBasedOnMasks(List<Rect> masks)
        {
            foreach (Rect mask in masks)
            {
                Log.Message("Mask dimensions X" + (int)(_processedTexture.width * mask.x) + " Y" + (int)(_processedTexture.height * mask.y) + " width " + _processedTexture.width * mask.width + " height " + _processedTexture.height * mask.height);
                int maskStartX = Mathf.FloorToInt(_processedTexture.width * mask.x);
                int maskStartY = Mathf.FloorToInt(_processedTexture.height * mask.y);
                int maskWidth = Mathf.FloorToInt(_processedTexture.width * mask.width);
                int maskHeight = Mathf.FloorToInt(_processedTexture.height * mask.height);


                for (int i = maskStartX; i < maskStartX+ maskWidth; i++)
                {
                    for (int j = maskStartY; j < maskStartY + maskHeight; j++)
                    {
                        _processedTexture.SetPixel(i, j, new Color(0, 0, 0, 0));
                    }
                }
            }
            
            _processedTexture.Apply();
        }

        public void UpdateTextureBasedOnAttachments()
        {

        }

        
    }
}
