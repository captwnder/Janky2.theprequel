using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Graphic that renders part of a sprite sheet, but does not automatically animate it at all.
    /// </summary>
    public class ImageSet : Image {

        int textureWidth;
        int textureHeight;

        int rows;
        int columns;

        public int Frames { get; private set; }

        public int Columns { get { return columns; } }
        public int Rows { get { return rows; } }

        public ImageSet(string source, int width, int height) : base(source) {
            textureWidth = (int)Sprite.Texture.Size.X;
            textureHeight = (int)Sprite.Texture.Size.Y;

            Width = width;
            Height = height;

            columns = (int)Math.Ceiling((float)textureWidth / width);
            rows = (int)Math.Ceiling((float)textureHeight / height);

            Frames = columns * rows;

            UpdateSourceRect(0);
        }

        /// <summary>
        /// Updates the internal source for the texture.
        /// </summary>
        /// <param name="frame">The frame in terms of the sprite sheet.</param>
        void UpdateSourceRect(int frame) {
            var top = (int)(Math.Floor((float)frame / columns) * Height) + atlasRect.Top;
            var left = (int)((frame % columns) * Width) + atlasRect.Left;

            SourceRect = new SFML.Graphics.IntRect(left, top, (int)Width, (int)Height);

            UpdateTexture();
        }

        int frame = 0;

        /// <summary>
        /// The frame to render from the image set.
        /// </summary>
        public int Frame {
            get { return frame; }
            set {
                frame = (int)Util.Min(value, Frames);
                UpdateSourceRect(frame);
            }
        }
    }
}
