//=============================================================================
// ApplicationIcon.cs
//
// Created by Victor on 2019/09/13
//=============================================================================

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Edo.Graphics
{
    /// <summary>
    /// Helper for loading an icon for the GLFW window
    /// </summary>
    internal struct ApplicationIcon
    {
        internal int Width { get; }
        internal int Height { get; }
        internal byte[] Pixels { get; private set; }

        /// <summary>
        /// Creates a new bitmap based icon
        /// </summary>
        /// <param name="path">Icon file path</param>
        public ApplicationIcon(string path)
        {
            var bmp = new Bitmap(path);
            Width = bmp.Width;
            Height = bmp.Height;

            // Setup the bitmap
            var rect = new Rectangle(0, 0, Width, Height);
            var bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

            // Create icon
            var ptr = bmpData.Scan0;
            var bytes = Math.Abs(bmpData.Stride) * Height;
            Pixels = new byte[bytes];
            Marshal.Copy(ptr, Pixels, 0, bytes);
            ChangePixelFormat();
            
            bmp.Dispose();
        }

        public GLFW.Image ToImage()
        {
            var size = Width * Height * 4;
            var img = new GLFW.Image(Width, Height, Marshal.AllocHGlobal(size));

            Marshal.Copy(Pixels, 0, img.Pixels, Math.Min(size, Pixels.Length));
            return img;
        }

        /// <summary>
        /// Changes the pixel format from default bitmap to GLFW's expected format
        /// </summary>
        private void ChangePixelFormat()
        {
            // TODO: Is this performant enough? (do we care?)
            for (var i = 0; i < Pixels.Length; i += 4)
            {
                var r = Pixels[i];
                var g = Pixels[i + 1];
                var b = Pixels[i + 2];
                var a = Pixels[i + 3];

                Pixels[i] = b;
                Pixels[i + 1] = g;
                Pixels[i + 2] = r;
                Pixels[i + 3] = a;
            }
        }
    }
}