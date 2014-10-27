using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace Otter {
    /// <summary>
    /// Collider that can use an image as a mask.  This is not recommended to use for most cases as it can
    /// be pretty expensive to process.  This is in active development and not quite finished yet.
    /// </summary>
    public class PixelCollider : Collider {

        SFML.Graphics.Image collideImage;
        SFML.Graphics.Texture texture;

        /// <summary>
        /// The amount of Alpha a pixel needs to exceed to register as a collision.
        /// If 0, any pixel with an alpha above 0 will register as collidable.
        /// </summary>
        public float Threshold = 0;

        /// <summary>
        /// Creates a pixel collider.
        /// </summary>
        /// <param name="source">The source image to create the collider from.</param>
        /// <param name="tags">The tags to register the collider with.</param>
        public PixelCollider(string source, params int[] tags) {
            texture = Textures.Load(source);
            collideImage = texture.CopyToImage();
            collideImage.SetPixel(4, 4, Color.Black.SFMLColor);
            
            Width = texture.Size.X;
            Height = texture.Size.Y;

            AddTag(tags);
        }

        /// <summary>
        /// The byte array of pixels.
        /// </summary>
        public byte[] Pixels {
            get { return collideImage.Pixels; }
        }

        /// <summary>
        /// Check if a pixel is collidable at x, y.
        /// </summary>
        /// <param name="x">The x coordinate of the pixel to check.</param>
        /// <param name="y">The y coordinate of the pixel to check.</param>
        /// <returns>True if the pixel collides.</returns>
        public bool PixelAt(int x, int y) {
            if (x < 0 || y < 0 || x > Width || y > Height) return false;

            if (collideImage.GetPixel((uint)x, (uint)y).A > Threshold) return true;
            return false;
        }

        /// <summary>
        /// Check if a pixel is collidable at x, y.
        /// </summary>
        /// <param name="x">The x coordinate of the pixel to check.</param>
        /// <param name="y">The y coordinate of the pixel to check.</param>
        /// <param name="threshold">The alpha threshold that should register a collision.</param>
        /// <returns>True if the pixel collides.</returns>
        public bool PixelAt(int x, int y, float threshold) {
            if (x < 0 || y < 0 || x > Width || y > Height) return false;

            if (collideImage.GetPixel((uint)x, (uint)y).A > threshold) return true;
            return false;
        }

        public bool PixelArea(int x, int y, int x2, int y2) {
            for (var i = x; i < x2; i++) {
                for (var j = y; j < y2; j++) {
                    if (PixelAt(i, j)) return true;
                }
            }
            return false;
        }

        public bool PixelArea(int x, int y, int x2, int y2, float threshold) {
            for (var i = x; i < x2; i++) {
                for (var j = y; j < y2; j++) {
                    if (PixelAt(i, j, threshold)) return true;
                }
            }
            return false;
        }
    }
}
