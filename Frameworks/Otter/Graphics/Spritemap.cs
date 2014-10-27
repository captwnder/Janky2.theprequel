using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using System.Xml;

namespace Otter {
    /// <summary>
    /// Graphic that is used for an animated sprite sheet.
    /// </summary>
    /// <typeparam name="TAnimType"></typeparam>
    public class Spritemap<TAnimType> : Image {

        int textureWidth;
        int textureHeight;

        int columns, rows;

        /// <summary>
        /// The playback speed of all animations.  
        /// </summary>
        public float Speed = 1f;

        /// <summary>
        /// The current buffered animation.
        /// </summary>
        public TAnimType BufferedAnimation { get; private set; }

        /// <summary>
        /// Determines if the sprite is playing animations.
        /// </summary>
        public bool Active { get; private set; }

        /// <summary>
        /// Determines if the sprite is advancing its current animation.
        /// </summary>
        public bool Paused { get; private set; }

        public Dictionary<TAnimType, Anim> Anims { get; private set; }

        public TAnimType CurrentAnim {
            get;
            private set;
        }

        /// <summary>
        /// Create a new spritemap from a file path.
        /// </summary>
        /// <param name="source">The file path or name of the image on the game's default atlas.</param>
        /// <param name="width">The width of the animation.</param>
        /// <param name="height">The height of the animation.</param>
        public Spritemap(string source, int width, int height) : base(source) {
            Anims = new Dictionary<TAnimType, Anim>();

            textureWidth = (int)Sprite.Texture.Size.X;
            textureHeight = (int)Sprite.Texture.Size.Y;
            columns = (int)Math.Ceiling((float)textureWidth / width);
            rows = (int)Math.Ceiling((float)textureHeight / height);
            Width = width;
            Height = height;
            UpdateSourceRect(0);
        }

        /// <summary>
        /// Create a new spritemap from an atlas.
        /// </summary>
        /// <param name="atlas">The atlas to load the texture from.</param>
        /// <param name="source">The name of the image to load from the atlas.</param>
        /// <param name="width">The width of the animation.</param>
        /// <param name="height">The height of the animation.</param>
        public Spritemap(Atlas atlas, string source, int width, int height) : base(atlas, source) {
            Anims = new Dictionary<TAnimType, Anim>();

            textureWidth = Sprite.TextureRect.Width;
            textureHeight = Sprite.TextureRect.Height;
            atlasRect.Left = atlas[source].X;
            atlasRect.Top = atlas[source].Y;
            columns = (int)Math.Ceiling((float)textureWidth / width);
            rows = (int)Math.Ceiling((float)textureHeight / height);
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Add an animation to the list of Anims.
        /// </summary>
        /// <param name="a">The key to reference this animation.</param>
        /// <param name="anim">The anim value.</param>
        public void Add(TAnimType a, Anim anim) {
            Anims.Add(a, anim);
            CurrentAnim = a;
        }

        /// <summary>
        /// Adds an animation using a string for frames and a single value for frame delay.
        /// </summary>
        /// <param name="a">The key to store the animation with.</param>
        /// <param name="frames">The frames of the animation from the sprite sheet.  Example: "1, 3, 1, 2, 4"</param>
        /// <param name="framedelays">The delay between advancing to the next frame.</param>
        /// <param name="delim">The delimiter to parse the string with.</param>
        /// <returns>The added animation.</returns>
        public Anim Add(TAnimType a, string frames, int framedelays, string delim = ",") {
            var anim = new Anim(frames, framedelays.ToString(), delim);
            Add(a, anim);
            return anim;
        }

        /// <summary>
        /// TODO: Add an animation from XML data.
        /// </summary>
        /// <param name="a">The key to store the animation with.</param>
        /// <param name="xml"></param>
        public Anim Add(TAnimType a, XmlElement xml) {

            return null;
        }

        /// <summary>
        /// Add an animation using a string for frames and a string for framedelays.
        /// </summary>
        /// <param name="a">The key to store the animation with.</param>
        /// <param name="frames">The frames of the animation from the sprite sheet.  Example: "1, 3, 1, 2, 4"</param>
        /// <param name="framedelays">The duration of time to show each frame.  Example: "10, 10, 5, 5, 50"</param>
        /// <param name="delim">The delimiter to parse the string with.</param>
        /// <returns>The added animation.</returns>
        public Anim Add(TAnimType a, string frames, string framedelays, string delim = ", ") {
            var anim = new Anim(frames, framedelays, delim);
            Add(a, anim);
            return anim;
        }

        /// <summary>
        /// Add an animation to the sprite.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <param name="frames">An array of the frames to display.</param>
        /// <param name="frameDelay">An array of durations for each frame.</param>
        /// <returns>The added animation.</returns>
        public Anim Add(TAnimType a, int[] frames, int[] frameDelay = null) {
            var anim = new Anim(frames, frameDelay);
            Add(a, anim);
            return anim;
        }

        /// <summary>
        /// Updates the animation.  Very important!
        /// </summary>
        public override void Update() {
            base.Update();

            if (!Active) return;

            if (!Anims.ContainsKey(CurrentAnim)) return;

            if (Paused) return;

            Anims[CurrentAnim].Update(Speed);

            UpdateSprite();
        }


        void UpdateSprite() {
            UpdateSourceRect(Anims[CurrentAnim].CurrentFrame);
            UpdateTexture();
        }

        /// <summary>
        /// Updates the internal source for the texture.
        /// </summary>
        /// <param name="frame">The frame in terms of the sprite sheet.</param>
        void UpdateSourceRect(int frame) {
            var top = (int)(Math.Floor((float)frame / columns) * Height) + atlasRect.Top;
            var left = (int)((frame % columns) * Width) + atlasRect.Left;

            SourceRect = new SFML.Graphics.IntRect(left, top, (int)Width, (int)Height);
        }

        /// <summary>
        /// Play the desired animation.
        /// </summary>
        /// <param name="a"></param>
        public void Play(TAnimType a) {
            Active = true;
            var pastAnim = CurrentAnim;
            CurrentAnim = a;
            if (Anims[CurrentAnim] != Anims[pastAnim]) {
                Anims[CurrentAnim].Reset();
            }
            UpdateSprite();
        }

        /// <summary>
        /// Buffers an animation but does not play it.  Call Play() with no arguments to play the buffered animation.
        /// </summary>
        /// <param name="a">The animation to buffer.</param>
        public void Buffer(TAnimType a) {
            BufferedAnimation = a;
        }

        /// <summary>
        /// Plays an animation.  If no animation is specified, play the buffered animation.
        /// </summary>
        public void Play() {
            if (BufferedAnimation != null) {
                Play(BufferedAnimation);
            }
            UpdateSprite();
        }

        /// <summary>
        /// Get the animation with a specific key.
        /// </summary>
        /// <param name="a">The key to search with.</param>
        /// <returns>The animation found.</returns>
        public Anim Anim(TAnimType a) {
            if (!Anims.ContainsKey(a)) return null;
            return Anims[a];
        }

        /// <summary>
        /// Pause the playback of the animation.
        /// </summary>
        public void Pause() {
            Paused = true;
        }

        /// <summary>
        /// Resume the animation from the current position.
        /// </summary>
        public void Resume() {
            Paused = false;
        }

        /// <summary>
        /// Stop playback.  This will reset the animation to the first frame.
        /// </summary>
        public void Stop() {
            Active = false;
            Anims[CurrentAnim].Stop();
        }

        /// <summary>
        /// The current frame of the animation on the sprite sheet.
        /// </summary>
        public int CurrentFrame {
            get {
                return Anims[CurrentAnim].CurrentFrame;
            }
            set {
                SetFrame(value);
            }
        }

        /// <summary>
        /// The current frame index of the animation, from 0 to frame count - 1.
        /// </summary>
        public int CurrentFrameIndex {
            get {
                return Anims[CurrentAnim].CurrentFrameIndex;
            }
            set {
                Anims[CurrentAnim].CurrentFrameIndex = value;
            }
        }

        /// <summary>
        /// Set the current animation to a specific frame.
        /// </summary>
        /// <param name="frame">The frame in terms of the animation.</param>
        public void SetFrame(int frame) {
            if (!Active) return;

            Anims[CurrentAnim].CurrentFrameIndex = frame;
            Anims[CurrentAnim].Reset();
        }

        /// <summary>
        /// Set the current animation to a specific frame and pause.
        /// </summary>
        /// <param name="frame">The frame in terms of the animation.</param>
        public void FreezeFrame(int frame) {
            if (!Active) return;

            Paused = true;
            Anims[CurrentAnim].CurrentFrameIndex = frame;

            UpdateSourceRect(Anims[CurrentAnim].CurrentFrame);
            UpdateTexture();
        }

        /// <summary>
        /// Set the sprite to a frame on the sprite sheet itself.
        /// This will disable the current animation!
        /// </summary>
        /// <param name="frame">The global frame in terms of the sprite sheet.</param>
        public void SetGlobalFrame(int frame) {
            Active = false;
            UpdateSourceRect(frame);
            UpdateTexture();
        }

        /// <summary>
        /// Resets the current animation back to the first frame.
        /// </summary>
        public void Reset() {
            Anims[CurrentAnim].Reset();
        }

        /// <summary>
        /// Clear the list of animations.
        /// </summary>
        public void Clear() {
            Anims.Clear();
        }

    }
}
