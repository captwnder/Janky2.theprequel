using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML;
using SFML.Graphics;
using SFML.Window;

namespace Otter {
    /// <summary>
    /// Graphic type that is used to represent a static image.
    /// </summary>
    public class Image : Graphic {
        protected Shape Shape;
        protected Sprite Sprite;

        protected bool IsSprite;

        internal bool isCircle = false;

        protected IntRect atlasRect = new IntRect(0, 0, 0, 0);

        protected Vector2f scale = new Vector2f(1f, 1f);
        protected float alpha = 1;
        protected Color color = Color.White;

        protected IntRect ClipRect;

        internal IntRect SourceRect;

        public Texture Texture;

        protected Transformable TransformSource;
        protected Drawable DrawableSource;

        public float ShakeX = 0;
        public float ShakeY = 0;

        public float Shake {
            set { ShakeX = value; ShakeY = value; }
        }

        /// <summary>
        /// The amount of points to use when rendering a circle shape.
        /// </summary>
        public static int CirclePointCount = 24;

        protected bool NeedsUpdate = true;

        /// <summary>
        /// The rectangle bounds of the image.
        /// </summary>
        public Rectangle Rect {
            get { return new Rectangle((int)(X - OriginX), (int)(Y - OriginY), Width, Height); }
        }

        /// <summary>
        /// Determines if the image should be rendered multiple times.
        /// </summary>
        public Repeat Repeat = Repeat.None;

        protected float
            originX = 0,
            originY = 0,
            angle = 0;

        protected float
            RepeatSizeX = 0,
            RepeatSizeY = 0;

        protected bool
            flipX = false,
            flipY = false;

        bool shook;

        float shakeX, shakeY;

        /// <summary>
        /// Creates a new image using a texture from an Atlas.
        /// </summary>
        /// <param name="atlas">The atlas to search.</param>
        /// <param name="source">The name of the texture in the atlas.</param>
        /// <param name="clip">The clipping rectangle to use.</param>
        public Image(Atlas atlas, string source, Rectangle clip = new Rectangle()) {
            Init(source, clip, atlas);
        }

        /// <summary>
        /// Creates an image using a Texture.
        /// </summary>
        /// <param name="texture">The texture to use for the image.</param>
        public Image(Texture texture, Rectangle clip = new Rectangle()) {
            Texture = texture;
            if (clip.Width == 0 || clip.Height == 0) {
                SourceRect = new IntRect(0, 0, (int)Texture.Width, (int)Texture.Height);
            }
            else {
                SourceRect = new IntRect(clip.Left, clip.Top, clip.Width, clip.Height);
            }

            IsSprite = true;
            Sprite = new Sprite(Texture.SFMLTexture);
            Sprite.TextureRect = SourceRect;
            UpdateTexture();

            Width = SourceRect.Width;
            Height = SourceRect.Height;

            TransformSource = Sprite;
            DrawableSource = Sprite;

            Smooth = Game.Instance.SmoothAll;
        }

        public override void Update() {
            base.Update();

            shakeX = Rand.Float(-ShakeX * 0.5f, ShakeX * 0.5f);
            shakeY = Rand.Float(-ShakeY * 0.5f, ShakeY * 0.5f);
        }

        /// <summary>
        /// Creates a new image using a texture from a file.
        /// </summary>
        /// <param name="source">The filepath of the texture.  If source file isn't found, attempt to locate the file in the Game's default atlas.</param>
        /// <param name="clip">Where to clip the texture.</param>
        public Image(string source = null, Rectangle clip = new Rectangle()) {
            if (Textures.Exists(source) || source == null) {
                Init(source, clip);
            }
            else {
                source = source.Replace(".png", "");
                Init(source, clip, Game.Instance.Atlas);
            }

        }

        /// <summary>
        /// Creates a new image from a file path.
        /// </summary>
        /// <param name="source">The file path to load from.  Will load from a cache if the file has been used before.</param>
        public Image(string source) : this(source, new Rectangle()) { }

        void Init(string source, Rectangle clip, Atlas atlas = null) {
            if (atlas == null && source != null) {
                //if no source found, try to load it from the atlas
                if (!Textures.Exists(source)) {
                    atlas = Game.Instance.Atlas;
                    source = source.Replace(".png", "");
                }
            }

            if (source == null) {
                IsSprite = false;

                Width = 0;
                Height = 0;
            }
            else {
                IsSprite = true;

                if (atlas != null) {
                    LoadTexture(atlas, source);
                }
                else {
                    LoadTexture(source, clip);
                }

                Sprite = new Sprite(Texture.SFMLTexture);
                Sprite.TextureRect = SourceRect;
                UpdateTexture();

                Width = SourceRect.Width;
                Height = SourceRect.Height;

                TransformSource = Sprite;
                DrawableSource = Sprite;
            }

            Blend = BlendMode.Alpha;

            Name = "Image";

        }

        void LoadTexture(string source, Rectangle clip) {
            if (clip.Width == 0 || clip.Height == 0) {
                Texture = new Texture(source);
                SourceRect = new IntRect(0, 0, (int)Texture.Width, (int)Texture.Height);
            }
            else {
                Texture = new Texture(source);
                SourceRect = new IntRect(clip.Left, clip.Top, clip.Width, clip.Height);
            }
        }

        void LoadTexture(Atlas atlas, string source) {
            var a = atlas.GetTexture(source);
            atlasRect.Left = a.X;
            atlasRect.Top = a.Y;
            atlasRect.Width = a.Width;
            atlasRect.Height = a.Height;
            Texture = a.Texture;
            SourceRect = atlasRect;
        }

        /// <summary>
        /// Creates a simple rectangle.
        /// </summary>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <returns>A new image containing the rectangle.</returns>
        static public Image CreateRectangle(int width, int height, Color color) {
            Image i = new Image();

            i.Shape = new RectangleShape(new Vector2f(width, height));
            i.Shape.FillColor = color.SFMLColor;

            i.Color = color;

            i.Width = width;
            i.Height = height;

            i.TransformSource = i.Shape;
            i.DrawableSource = i.Shape;

            return i;
        }

        /// <summary>
        /// Creates a simple rectangle the size of the active Game window.
        /// </summary>
        /// <param name="color">The color of the rectangle.</param>
        /// <returns>A new image containing the rectangle.</returns>
        static public Image CreateRectangle(Color color) {
            int w = Game.Instance.Width;
            int h = Game.Instance.Height;
            return Image.CreateRectangle(w, h, color);
        }

        /// <summary>
        /// Creates a simple black rectangle the size of the active Game window.
        /// </summary>
        /// <returns>A new image containing the rectangle.</returns>
        static public Image CreateRectangle() {
            return Image.CreateRectangle(Color.Black);
        }

        /// <summary>
        /// Creates a simple rectangle.
        /// </summary>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <returns>A new image containing the rectangle.</returns>
        static public Image CreateRectangle(int width, int height) {
            return CreateRectangle(width, height, Color.White);
        }

        /// <summary>
        /// Creates a simple rectangle.
        /// </summary>
        /// <param name="size">The width and height of the rectangle.</param>
        /// <returns>A new image containing the rectangle.</returns>
        static public Image CreateRectangle(int size) {
            return CreateRectangle(size, size);
        }

        /// <summary>
        /// Creates a simple rectangle.
        /// </summary>
        /// <param name="size">The width and height of the rectangle.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <returns>A new image containing the rectangle.</returns>
        static public Image CreateRectangle(int size, Color color) {
            return CreateRectangle(size, size, color);
        }

        /// <summary>
        /// Create a simple circle image.
        /// </summary>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="color">The color of the circle.</param>
        /// <returns>A new image containing the circle.</returns>
        static public Image CreateCircle(int radius, Color color) {
            Image i = new Image();

            i.isCircle = true;

            i.Shape = new CircleShape(radius);
            i.Shape.FillColor = color.SFMLColor;

            (i.Shape as CircleShape).SetPointCount((uint)CirclePointCount);

            i.Width = radius * 2;
            i.Height = radius * 2;

            i.TransformSource = i.Shape;
            i.DrawableSource = i.Shape;

            return i;
        }


        /// <summary>
        /// Create a simple white circle image.
        /// </summary>
        /// <param name="radius">The radius of the circle.</param>
        /// <returns>A new image containing the circle.</returns>
        static public Image CreateCircle(int radius) {
            return CreateCircle(radius, Color.White);
        }

        /// <summary>
        /// Updates the texture.  Usually this is called automatically, but if the Texture has
        /// been changed in some way that the Image doesn't know about, this will have to be
        /// called manually after the changes.
        /// </summary>
        public void UpdateTexture() {
            color.image = this; //this is a fix to allow the color to be set easier

            if (IsSprite) {
                int top, left, width, height;
                if (flipX) {
                    left = SourceRect.Left + SourceRect.Width;
                    width = -SourceRect.Width;
                }
                else {
                    left = SourceRect.Left;
                    width = SourceRect.Width;
                }

                if (flipY) {
                    top = SourceRect.Top + SourceRect.Height;
                    height = -SourceRect.Height;
                }
                else {
                    top = SourceRect.Top;
                    height = SourceRect.Height;
                }

                Texture.Update();
                Sprite.Texture = Texture.SFMLTexture;
                Sprite.TextureRect = new IntRect(left, top, width, height);
            }
        }

        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="x">the x offset to draw the image from</param>
        /// <param name="y">the y offset to draw the image from</param>
        public override void Render(float x = 0, float y = 0) {
            if (!Visible) return;

            float renderX = X + x + shakeX, renderY = Y + y + shakeY;

            if (ScrollX != 1) {
                renderX = X + Draw.Target.CameraX * (1 - ScrollX) + x;
            }
            if (ScrollY != 1) {
                renderY = Y + Draw.Target.CameraY * (1 - ScrollY) + y;
            }

            float
                screenX = renderX - Draw.Target.CameraX,
                screenY = renderY - Draw.Target.CameraY;

            float
                zoom = Draw.Target.CameraZoom,
                w = Draw.Target.Width + OriginX,
                h = Draw.Target.Height + OriginY;

            float
                repeatLeft = Draw.Target.CameraX - (w / zoom - w) * 0.5f,
                repeatTop = Draw.Target.CameraY - (h / zoom - h) * 0.5f,
                repeatRight = Draw.Target.CameraX + (w / zoom + w) * 0.5f,
                repeatBottom = Draw.Target.CameraY + (h / zoom + w) * 0.5f;

            RepeatSizeX = repeatRight - repeatLeft + OriginX;
            RepeatSizeY = repeatBottom - repeatTop + OriginY;

            switch (Repeat) {
                case Repeat.None:
                    RenderImage(renderX, renderY);
                    break;
                case Repeat.X:
                    while (renderX > repeatLeft) {
                        renderX -= ScaledWidth;
                    }

                    while (renderX < repeatRight) {
                        RenderImage(renderX, renderY);
                        renderX += ScaledWidth;
                    }

                    break;

                case Repeat.Y:
                    while (renderY > repeatTop) {
                        renderY -= ScaledHeight;
                    }

                    while (renderY < repeatBottom) {
                        RenderImage(renderX, renderY);
                        renderY += Height;
                    }
                    break;

                case Repeat.XY:
                    float startX = renderX;
                    while (renderY > repeatTop) {
                        renderY -= ScaledHeight;
                    }

                    while (renderY < repeatBottom) {
                        while (renderX > repeatLeft) {
                            renderX -= ScaledWidth;
                        }

                        while (renderX < repeatRight) {
                            RenderImage(renderX, renderY);
                            renderX += ScaledWidth;
                        }
                        renderY += ScaledHeight;

                    }

                    break;
            }       
        }

        protected virtual void RenderImage(float x = 0, float y = 0) {
            RenderStates = Game.Instance.renderStates;
            if (Texture != null) {
                if (RenderStates.Texture != Texture.SFMLTexture) {
                    RenderStates.Texture = Texture.SFMLTexture;
                }
            }
            RenderStates.Transform.Translate(new Vector2f(x - originX, y - originY));
            RenderStates.Transform.Rotate(-angle, originX, originY);
            RenderStates.Transform.Scale(scale.X, scale.Y, originX, originY);
            if (RenderStates.BlendMode != (SFML.Graphics.BlendMode)Blend) {
                RenderStates.BlendMode = (SFML.Graphics.BlendMode)Blend;
            }
            if (Shader != null) {
                RenderStates.Shader = Shader.shader;
            }

            if (DrawableSource == null) {
                throw new NullReferenceException("Image DrawableSource is null!");
            }
            Draw.Drawable(DrawableSource, RenderStates);
        }

        /// <summary>
        /// The origin in which the image will be drawn relative to the X coordinate.
        /// </summary>
        public float OriginX {
            get {
                return originX;
            }
            set {
                originX = value;
            }
        }

        /// <summary>
        /// The origin in which the image will be drawn relative to the Y coordinate.
        /// </summary>
        public float OriginY {
            get {
                return originY;
            }
            set {
                originY = value;
            }
        }

        /// <summary>
        /// Flip the texture coordinates on the X axis.
        /// Only applies to textured images!
        /// </summary>
        public bool FlippedX {
            get { return flipX; }
            set {
                flipX = value;
                UpdateTexture();
            }
        }

        /// <summary>
        /// Flip the texture coordinates on the Y axis.
        /// Only applies to textured images!
        /// </summary>
        public bool FlippedY {
            get { return flipY; }
            set {
                flipY = value;
                UpdateTexture();
            }
        }

        /// <summary>
        /// Centers the image origin.
        /// </summary>
        public virtual void CenterOrigin() {
            OriginX = HalfWidth;
            OriginY = HalfHeight;
        }

        /// <summary>
        /// Centers the image origin while retaining its relative position.
        /// </summary>
        public virtual void CenterOriginZero() {
            float ox, oy;
            ox = OriginX;
            oy = OriginY;
            CenterOrigin();
            ox = OriginX - ox;
            oy = OriginY - oy;
            X += ox;
            Y += oy;
        }

        /// <summary>
        /// Half of the width.
        /// </summary>
        public float HalfWidth { get { return Width / 2f; } }

        /// <summary>
        /// Half of the height.
        /// </summary>
        public float HalfHeight { get { return Height / 2f; } }

        /// <summary>
        /// The rotation of the image.
        /// </summary>
        public float Angle {
            get {
                return angle;
            }
            set {
                angle = value;
            }
        }

        /// <summary>
        /// The X scale of the image.
        /// </summary>
        public float ScaleX {
            get {
                return scale.X;
            }
            set {
                scale = new Vector2f(value, scale.Y);
            }
        }

        /// <summary>
        /// The Y scale of the image.
        /// </summary>
        public float ScaleY {
            get {
                return scale.Y;
            }
            set {
                scale = new Vector2f(scale.X, value);
            }
        }

        /// <summary>
        /// Sets both the ScaleX and ScaleY at the same time.  A shortcut property.
        /// </summary>
        public float Scale {
            set { ScaleX = value; ScaleY = value; }
        }

        public float Left {
            get { return X - OriginX; }
        }

        public float Top {
            get { return Y - OriginY; }
        }

        public float Right {
            get { return Left + Width; }
        }

        public float Bottom {
            get { return Top + Height; }
        }

        /// <summary>
        /// The tint color of the image, or color of the shape.
        /// </summary>
        public Color Color {
            set {
                color = value;
                NeedsUpdate = true;
                UpdateColor();
                color.image = this;
            }
            get {
                return color;

                //probably dont need this anymore
                if (DrawableSource is SFML.Graphics.Text) {
                    return new Color((DrawableSource as SFML.Graphics.Text).Color);
                }
                else if (IsSprite) {
                    return new Color(Sprite.Color);
                }
                else {
                    if (Shape != null) {
                        return new Color(Shape.FillColor);
                    }
                    return color;
                }
            }
        }

        void UpdateColor() {
            if (DrawableSource is SFML.Graphics.Text) {
                (DrawableSource as SFML.Graphics.Text).Color = color.SFMLColor;
            }
            else if (IsSprite) {
                Sprite.Color = color.SFMLColor;
            }
            else {
                if (Shape != null) {
                    Shape.FillColor = color.SFMLColor;
                }
            }
        }

        /// <summary>
        /// The alpha or transparency of the image or shape. This just affects the A value of the Color.
        /// </summary>
        public float Alpha {
            set {
                alpha = Util.Clamp(value, 0, 1);
                color.A = Util.Clamp(value, 0, 1);

                if (DrawableSource is SFML.Graphics.Text) {
                    SFML.Graphics.Text temptext = DrawableSource as SFML.Graphics.Text;
                    temptext.Color = new SFML.Graphics.Color(temptext.Color.R, temptext.Color.G, temptext.Color.B, (byte)(alpha * 255f));
                }
                else if (IsSprite) {
                    Sprite.Color = new SFML.Graphics.Color(Sprite.Color.R, Sprite.Color.G, Sprite.Color.B, (byte)(alpha * 255f));
                }
                else {
                    if (Shape != null) {
                        Shape.FillColor = new SFML.Graphics.Color(Shape.FillColor.R, Shape.FillColor.G, Shape.FillColor.B, (byte)(alpha * 255f));
                    }
                }
                
                NeedsUpdate = true;
            }
            get {
                return alpha;

                //might not need any of this
                if (DrawableSource is SFML.Graphics.Text) {
                    return (float)(DrawableSource as SFML.Graphics.Text).Color.A / 255f;
                }
                else if (IsSprite) {
                    return (float)(Sprite.Color.A) / 255f;
                }
                else {
                    if (Shape != null) {
                        return (float)(Shape.FillColor.A) / 255f;
                    }

                    return color.A;
                }
            }
        }

        /// <summary>
        /// The width in pixels of the image after applying the X scale.
        /// </summary>
        public float ScaledWidth {
            get {
                return Width * ScaleX;
            }
            set {
                ScaleX = value / Width;
            }
        }

        /// <summary>
        /// The height in pixels of the image after applying the Y scale.
        /// </summary>
        public float ScaledHeight {
            get {
                return Height * ScaleY;
            }
            set {
                ScaleY = value / Height;
            }
        }

        /// <summary>
        /// Smooth the texture of a sprite image while scaling it.
        /// </summary>
        public virtual bool Smooth {
            get {
                if (Texture != null) return Texture.Smooth;
                return false;
            }
            set {
                if (Texture != null) Texture.Smooth = value;
            }
        }

        /// <summary>
        /// The outline color of the image (only applies to circles and rectangles.)
        /// </summary>
        public virtual Color OutlineColor {
            set {
                if (!IsSprite) {
                    Shape.OutlineColor = value.SFMLColor;
                }
            }
            get {
                if (!IsSprite) {
                    return new Color(Shape.OutlineColor);
                }
                return null;
            }
        }

        /// <summary>
        /// The outline thickness of the image (only applies to circles and rectangles.)
        /// </summary>
        public virtual float OutlineThickness {
            set {
                if (!IsSprite) {
                    Shape.OutlineThickness = value;
                }
            }
            get {
                if (!IsSprite) {
                    return Shape.OutlineThickness;
                }
                return 0;
            }
        }

        /// <summary>
        /// Called by ImageBatch, add itself to an image batch in the proper way (different for different Images)
        /// </summary>
        internal void Batched(VertexArray vertices) {
            
            var point = new Vector2(-OriginX, -OriginY);
            point.X *= ScaleX;
            point.Y *= ScaleY;
            point = Util.Rotate(point, angle * Util.DEG_TO_RAD);

            point.X += X;
            point.Y += Y;
            
            vertices.Append((float)point.X, (float)point.Y, Color, SourceRect.Left, SourceRect.Top);
            
            point = new Vector2(Width-OriginX, -OriginY);
            point.X *= ScaleX;
            point.Y *= ScaleY;
            point = Util.Rotate(point, angle * Util.DEG_TO_RAD);

            point.X += X;
            point.Y += Y;
            
            vertices.Append((float)point.X, (float)point.Y, Color, SourceRect.Left + SourceRect.Width, SourceRect.Top);
            
            point = new Vector2(Width-OriginX, Height-OriginY);
            point.X *= ScaleX;
            point.Y *= ScaleY;
            point = Util.Rotate(point, angle * Util.DEG_TO_RAD);

            point.X += X;
            point.Y += Y;
            
            vertices.Append((float)point.X, (float)point.Y, Color, SourceRect.Left + SourceRect.Width, SourceRect.Top + SourceRect.Height);
            
            point = new Vector2(-OriginX, Height-OriginY);
            point.X *= ScaleX;
            point.Y *= ScaleY;
            point = Util.Rotate(point, angle * Util.DEG_TO_RAD);

            point.X += X;
            point.Y += Y;
            
            vertices.Append((float)point.X, (float)point.Y, Color, SourceRect.Left, SourceRect.Top + SourceRect.Height);
        }
    }
}
