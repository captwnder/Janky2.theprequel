using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML;
using SFML.Graphics;
using SFML.Window;

namespace Otter {
    /// <summary>
    /// Graphic that represents a render target.  By default the game uses a master surface to
    /// render the game to the window.  Be aware of graphics card limiations of render textures when
    /// creating surfaces.
    /// </summary>
    public class Surface : Image {

        internal RenderTexture target;

        /// <summary>
        /// The color that the surface will fill with at the start of each render.
        /// </summary>
        public Color FillColor;

        RenderStates states;

        /// <summary>
        /// The reference to the Game using this Surface (if it is the main Surface the game is rendering to!)
        /// </summary>
        public Game Game;

        /// <summary>
        /// Determines if the Surface will automatically clear at the start of the next render cycle.
        /// </summary>
        public bool AutoClear = true;

        float cameraX, cameraY, cameraAngle, cameraZoom = 1f;

        RectangleShape fill;

        List<Shader> shaders = new List<Shader>();

        RenderTexture
            postProcessA,
            postProcessB;

        /// <summary>
        /// Creates a surface with a specified size.  Surfaces are used as drawing targets.  Be aware
        /// of video card limitations when using a lot of surfaces!
        /// </summary>
        /// <param name="width">The width of the surface to create.</param>
        /// <param name="height">The height of the surface to create.</param>
        /// <param name="color">The default fill color of the surface.</param>
        public Surface(int width, int height, Color color) {
            if (width < 0) throw new ArgumentException("Width must be greater than 0.");
            if (height < 0) throw new ArgumentException("Height must be greater than 0.");

            FillColor = color;
            Width = width;
            Height = height;
            target = new RenderTexture((uint)Width, (uint)Height);

            Sprite = new Sprite(target.Texture);
            //Texture = new Texture(target.Texture);
            IsSprite = true;
            fill = new RectangleShape(new Vector2f(Width, Height));
            DrawableSource = Sprite;
            Clear();
        }

        public Surface(int width, int height) : this(width, height, Color.None) { }
        public Surface(int size) : this(size, size) { }
        public Surface(int size, Color color) : this(size, size, color) { }

        internal void Draw(Drawable drawable) {
            RenderTarget.Draw(drawable);
        }

        internal void Draw(Drawable drawable, RenderStates states) {
            RenderTarget.Draw(drawable, states);
        }

        /// <summary>
        /// Add a shader to be drawn on the surface.  If "Shader" is set, the shader list is ignored.
        /// </summary>
        /// <param name="shader"></param>
        public void AddShader(Shader shader) {
            shaders.Add(shader);
            UpdateShader();
        }

        /// <summary>
        /// Remove a shader from the surface.
        /// </summary>
        /// <param name="shader"></param>
        public void RemoveShader(Shader shader) {
            shaders.Remove(shader);
            UpdateShader();
        }

        /// <summary>
        /// Remove the top most shader on the list of shaders.
        /// </summary>
        /// <returns>The shader removed.</returns>
        public Shader PopShader() {
            if (shaders.Count == 0) return null;

            var shader = shaders[shaders.Count - 1];
            RemoveShader(shader);
            return shader;
        }

        /// <summary>
        /// Remove all shaders from the surface.
        /// </summary>
        public void ClearShaders() {
            shaders.Clear();
            UpdateShader();
        }

        /// <summary>
        /// Replace all shaders with a single shader.  This will be ignored if "Shader" is set.
        /// </summary>
        /// <param name="shader"></param>
        public void SetShader(Shader shader) {
            shaders.Clear();
            shaders.Add(shader);
            UpdateShader();
        }

        void UpdateShader() {
            if (shaders.Count < 2) {
                if (postProcessA != null) {
                    postProcessA.Dispose();
                    postProcessA = null;
                }
                if (postProcessB != null) {
                    postProcessB.Dispose();
                    postProcessB = null;
                }
            }
            else if (shaders.Count == 2) {
                if (postProcessA == null) {
                    postProcessA = new RenderTexture((uint)Width, (uint)Height);
                }
                if (postProcessB != null) {
                    postProcessB.Dispose();
                    postProcessB = null;
                }
            }
            else if (shaders.Count > 2) {
                if (postProcessA == null) {
                    postProcessA = new RenderTexture((uint)Width, (uint)Height);
                }
                if (postProcessB == null) {
                    postProcessB = new RenderTexture((uint)Width, (uint)Height);
                }
            }
        }

        /// <summary>
        /// Draws a graphic to this surface.
        /// </summary>
        /// <param name="graphic"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Draw(Graphic graphic, float x = 0, float y = 0) {
            Surface tempSurface = Otter.Draw.Target;
            Otter.Draw.SetTarget(this);
            graphic.Render(x, y);
            Otter.Draw.SetTarget(tempSurface);
        }

        void Display() {
            target.Display();
        }

        /// <summary>
        /// Fills the surface with the specified color.
        /// </summary>
        /// <param name="color"></param>
        public void Fill(Color color) {
            fill.Size = new Vector2f(Width, Height);
            fill.FillColor = color.SFMLColor;
            fill.Position = new Vector2f(CameraX, CameraY);
            target.Draw(fill);
        }

        /// <summary>
        /// Clears the surface with the fill color.
        /// </summary>
        public void Clear() {
            target.Clear(FillColor.SFMLColor);
        }

        /// <summary>
        /// Clears the surface with a specified color.
        /// </summary>
        /// <param name="color"></param>
        public void Clear(Color color) {
            target.Clear(color.SFMLColor);
        }

        /// <summary>
        /// Determines the pixel smoothing for the surface.
        /// </summary>
        public override bool Smooth {
            get {
                return target.Smooth;
            }
            set {
                target.Smooth = value;
            }
        }

        public override void Render(float x = 0, float y = 0) {
            Display();

            base.Render(x, y);

            if (AutoClear) Clear();
        }

        protected override void RenderImage(float x = 0, float y = 0) {
            Drawable drawable = RenderShaders();
            base.RenderImage(x, y);
        }

        /// <summary>
        /// Draw the surface directly to the game window.  This will refresh the view,
        /// and Display the surface, as well as clear it if AutoClear is true.
        /// </summary>
        /// <param name="game"></param>
        public void DrawToWindow(Game game) {
            RefreshView();

            Display();

            Drawable drawable = RenderShaders();

            game.Window.Draw(drawable, states);

            if (AutoClear) Clear(FillColor);
        }

        /// <summary>
        /// This goes through all the shaders and applies them between two buffers, and
        /// eventually spits out the final drawable object.
        /// </summary>
        /// <returns></returns>
        Drawable RenderShaders() {
            Drawable drawable = Sprite;
            
            states = new RenderStates(target.Texture);
            states.Transform.Rotate(angle, originX, originY);
            states.Transform.Translate(new Vector2f(X - originX, Y - originY));
            states.Transform.Scale(scale.X, scale.Y, originX, originY);
            states.BlendMode = (SFML.Graphics.BlendMode)Blend;
            if (Shader != null) {
                states.Shader = Shader.shader;
            }
            else {
                if (shaders.Count == 1) {
                    states.Shader = shaders[0].shader;
                }
                if (shaders.Count == 2) {
                    states = RenderStates.Default;
                    states.Shader = shaders[0].shader;

                    Game.Instance.RenderCount++;
                    postProcessA.Draw(DrawableSource, states);
                    postProcessA.Display();

                    states.Shader = shaders[1].shader;

                    drawable = new Sprite(postProcessA.Texture);

                    states.Transform.Rotate(angle, originX, originY);
                    states.Transform.Translate(new Vector2f(X - originX, Y - originY));
                    states.Transform.Scale(scale.X, scale.Y, originX, originY);
                }
                if (shaders.Count > 2) {
                    states = RenderStates.Default;
                    RenderTexture nextRt, currentRt;
                    nextRt = postProcessB;
                    currentRt = postProcessA;

                    states.Shader = shaders[0].shader;

                    Game.Instance.RenderCount++;
                    postProcessA.Draw(DrawableSource, states);
                    postProcessA.Display();

                    for (int i = 1; i < shaders.Count - 1; i++) {
                        states = RenderStates.Default;
                        states.Shader = shaders[i].shader;

                        Game.Instance.RenderCount++;
                        nextRt.Draw(new Sprite(currentRt.Texture), states);
                        nextRt.Display();

                        nextRt = nextRt == postProcessA ? postProcessB : postProcessA;
                        currentRt = currentRt == postProcessA ? postProcessB : postProcessA;
                    }

                    drawable = new Sprite(currentRt.Texture);
                    currentRt.Display();
                    states.Shader = shaders[shaders.Count - 1].shader;
                    states.Transform.Rotate(angle, originX, originY);
                    states.Transform.Translate(new Vector2f(X - originX, Y - originY));
                    states.Transform.Scale(scale.X, scale.Y, originX, originY);
                }
            }

            return drawable;
        }

        public void DrawToWindow() {
            DrawToWindow(Game);
        }

        internal RenderTarget RenderTarget {
            get { return target; }
        }

        public void SetView(float x, float y, float angle = 0, float zoom = 1f) {
            cameraX = x;
            cameraY = y;
            cameraAngle = angle;
            cameraZoom = zoom;
            RefreshView();
        }

        void RefreshView() {
            View v = new View(new FloatRect(cameraX, cameraY, Width, Height));
            
            v.Rotation = -cameraAngle;
            v.Zoom(1/cameraZoom);
            RenderTarget.SetView(v);
        }

        /// <summary>
        /// Returns a texture by getting the current render texture. I don't know if this works right yet.
        /// </summary>
        /// <returns></returns>
        public Texture GetTexture() {
            return new Texture(target.Texture);
        }

        /// <summary>
        /// Save the current texture to a file. The supported image formats are bmp, png, tga and jpg.
        /// </summary>
        /// <param name="path">The file path to save to. The type of image is deduced from the extension.</param>
        public void SaveToFile(string path) {
            var img = target.Texture.CopyToImage();
            img.SaveToFile(path);
        }

        /// <summary>
        /// The camera X for the view of the surface.
        /// Note: For the game's main surface, this is controlled by the active scene.
        /// </summary>
        public float CameraX {
            set {
                cameraX = value;
                RefreshView();
            }
            get {
                return cameraX;
            }
        }

        /// <summary>
        /// The camera Y for the view of the surface.
        /// Note: For the game's main surface, this is controlled by the active scene.
        /// </summary>
        public float CameraY {
            set {
                cameraY = value;
                RefreshView();
            }
            get {
                return cameraY;
            }
        }

        /// <summary>
        /// The camera angle for the view of the surface.
        /// Note: For the game's main surface, this is controlled by the active scene.
        /// </summary>
        public float CameraAngle {
            set {
                cameraAngle = value;
                RefreshView();
            }
            get {
                return cameraAngle;
            }
        }

        /// <summary>
        /// The camera zoom for the view of the surface.
        /// Note: For the game's main surface, this is controlled by the active scene.
        /// </summary>
        public float CameraZoom {
            set {
                cameraZoom = value;
                if (cameraZoom <= 0) { cameraZoom = 0.0001f; } //dont be divin' by zero ya hear?
                RefreshView();
            }
            get {
                return cameraZoom;
            }
        }

        /// <summary>
        /// Matches the view of the surface to the same view of a Scene.
        /// </summary>
        /// <param name="scene"></param>
        public void CameraScene(Scene scene) {
            SetView(scene.CameraX + X, scene.CameraY + Y, scene.CameraAngle, scene.CameraZoom);
        }



    }
}
