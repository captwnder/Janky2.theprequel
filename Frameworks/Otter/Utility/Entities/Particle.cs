using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Entity that is a quick way to make a particle.  Has lots of parameters that can be set, so
    /// use this with that constructor where you do { } and put a bunch of properties inside.
    /// </summary>
    public class Particle : Entity {

        public ImageSet Image;

        public static int DefaultLifeSpan = 60;

        #region Initial Properties

        public float SpeedX = 0;
        public float SpeedY = 0;

        public float XOffset = 0;
        public float YOffset = 0;

        public float ScaleX = 1;
        public float ScaleY = 1;
        public float Angle = 0;

        public float Alpha = 1;

        public float ColorR = 1;
        public float ColorG = 1;
        public float ColorB = 1;

        Color color;
        bool hasColor;
        public Color Color {
            set { color = value; hasColor = true; }
            get { return color; }
        }
        float colorLerp;


        public int Delay = 0;

        public List<int> Frames;
        bool useFrameList = false;

        /// <summary>
        /// How many steps the particle should move by its speed when first created.
        /// </summary>
        public int AdvanceSteps = 0;

        public bool FlipX = false;
        public bool FlipY = false;

        float speedLen;
        bool hasSpeedLen;
        /// <summary>
        /// The magnitude of the particle's movement.  Overrides SpeedX and SpeedY.
        /// </summary>
        public float SpeedLen {
            set { speedLen = value; hasSpeedLen = true; }
            get { return speedLen; }
        }

        float speedDir;
        bool hasSpeedDir;
        /// <summary>
        /// The direction of the particle's movement.  Overrides SpeedX and SpeedY.
        /// </summary>
        public float SpeedDir {
            set { speedDir = value; hasSpeedDir = true; }
            get { return speedDir; }
        }

        /// <summary>
        /// Determines if the image angle should be locked to the direction of the particle's movement.
        /// </summary>
        public bool MotionAngle = false;

        public BlendMode Blend = BlendMode.Alpha;

        int frameCount;
        bool hasFrameCount;
        public int FrameCount {
            set { frameCount = value; hasFrameCount = true; }
            get { return frameCount; }
        }

        public int FrameOffset = 0;

        public int Loops = 0;

        #endregion

        #region Final Properties

        float finalSpeedX;
        bool hasFinalSpeedX;
        public float FinalSpeedX {
            set { finalSpeedX = value; hasFinalSpeedX = true; }
            get { return finalSpeedX; }
        }

        float finalSpeedY;
        bool hasFinalSpeedY;
        public float FinalSpeedY {
            set { finalSpeedY = value; hasFinalSpeedY = true; }
            get { return finalSpeedY; }
        }

        float finalScaleX;
        bool hasFinalScaleX;
        public float FinalScaleX {
            set { finalScaleX = value; hasFinalScaleX = true; }
            get { return finalScaleX; }
        }
        float finalScaleY;
        bool hasFinalScaleY;
        public float FinalScaleY {
            set { finalScaleY = value; hasFinalScaleY = true; }
            get { return finalScaleY; }
        }

        float finalAngle;
        bool hasFinalAngle;
        public float FinalAngle {
            set { finalAngle = value; hasFinalAngle = true; }
            get { return finalAngle; }
        }

        float finalX;
        bool hasFinalX;
        public float FinalX {
            set { finalX = value; hasFinalX = true; }
            get { return finalX; }
        }

        float finalY;
        bool hasFinalY;
        public float FinalY {
            set { finalY = value; hasFinalY = true; }
            get { return finalY; }
        }

        float finalAlpha;
        bool hasFinalAlpha;
        public float FinalAlpha {
            set { finalAlpha = value; hasFinalAlpha = true; }
            get { return finalAlpha; }
        }

        float finalColorR;
        bool hasFinalColorR;
        public float FinalColorR {
            set { finalColorR = value; hasFinalColorR = true; }
            get { return finalColorR; }
        }

        float finalColorG;
        bool hasFinalColorG;
        public float FinalColorG {
            set { finalColorG = value; hasFinalColorG = true; }
            get { return finalColorG; }
        }

        float finalColorB;
        bool hasFinalColorB;
        public float FinalColorB {
            set { finalColorB = value; hasFinalColorB = true; }
            get { return finalColorB; }
        }

        Color finalColor;
        bool hasFinalColor;
        public Color FinalColor {
            set { finalColor = value; hasFinalColor = true; }
            get { return finalColor; }
        }

        float finalSpeedLen;
        bool hasFinalSpeedLen;
        public float FinalSpeedLen {
            set { finalSpeedLen = value; hasFinalSpeedLen = true; }
            get { return finalSpeedLen; }
        }

        float finalSpeedDir;
        bool hasFinalSpeedDir;
        public float FinalSpeedDir {
            set { finalSpeedDir = value; hasFinalSpeedDir = true; }
            get { return finalSpeedDir; }
        }
        #endregion

        /// <summary>
        /// Determines if the ScaleY will always be locked to the ScaleX.
        /// </summary>
        public bool LockScaleRatio = false;

        public bool CenterOrigin = true;

        public float OriginX {
            get { return originX; }
            set {
                originX = value;
                useOrigin = true;
            }
        }
        float originX;

        public float OriginY {
            get { return originY; }
            set {
                originY = value;
                useOrigin = true;
            }
        }
        float originY;

        bool useOrigin;
        public bool Animate;

        bool useSpeedXY = true;
        bool tweenPosition = false;

        float xpos, ypos;

        int delayTimer = 0;

        public Particle(float x, float y, string source, int width, int height) : base(x, y) {
            Image = new ImageSet(source, width, height);
            
        }

        public override void Added() {
            base.Added();

            if (Delay == 0) Start();
        }

        public void Start() {
            if (LifeSpan == 0) {
                LifeSpan = DefaultLifeSpan;
            }

            if (Frames != null) {
                useFrameList = true;
                FrameCount = Frames.Count;
            }

            if (FrameCount > 0) {
                Animate = true;
            }

            Tweener.CancelAndComplete();

            if (hasSpeedLen || hasSpeedDir) {
                useSpeedXY = false;
            }
            if (hasFinalX || hasFinalY) {
                tweenPosition = true;
            }

            if (!tweenPosition) {
                if (useSpeedXY) {
                    if (!hasFinalSpeedX) {
                        FinalSpeedX = SpeedX;
                    }
                    if (!hasFinalSpeedY) {
                        FinalSpeedY = SpeedY;
                    }
                    SpeedLen = 0;
                    FinalSpeedLen = 0;
                    SpeedDir = 0;
                    FinalSpeedDir = 0;
                }
                else {
                    FinalSpeedX = 0;
                    FinalSpeedY = 0;
                    if (!hasFinalSpeedLen) {
                        FinalSpeedLen = SpeedLen;
                    }
                    if (!hasFinalSpeedDir) {
                        FinalSpeedDir = SpeedDir;
                    }
                }
                FinalX = 0;
                FinalY = 0;
            }
            else {
                xpos = X;
                ypos = Y;
                if (!hasFinalX) {
                    FinalX = X;
                }
                if (!hasFinalY) {
                    FinalY = Y;
                }
            }
            if (!hasFinalScaleX) {
                FinalScaleX = ScaleX;
            }
            if (!hasFinalScaleY) {
                FinalScaleY = ScaleY;
            }
            if (!hasFinalAngle) {
                FinalAngle = Angle;
            }
            if (!hasFinalAlpha) {
                FinalAlpha = Alpha;
            }
            if (!hasFinalColorR) {
                FinalColorR = ColorR;
            }
            if (!hasFinalColorG) {
                FinalColorG = ColorG;
            }
            if (!hasFinalColorB) {
                FinalColorB = ColorB;
            }

            Tween(this, new {
                SpeedX = FinalSpeedX,
                SpeedY = FinalSpeedY,
                ScaleX = FinalScaleX,
                ScaleY = FinalScaleY,
                Angle = FinalAngle,
                Alpha = FinalAlpha,
                ColorR = FinalColorR,
                ColorG = FinalColorG,
                ColorB = FinalColorB,
                SpeedLen = FinalSpeedLen,
                SpeedDir = FinalSpeedDir,
                xpos = FinalX,
                ypos = FinalY,
                colorLerp = 1
            }, LifeSpan);

            X += XOffset;
            Y += YOffset;

            AddGraphic(Image);

            if (useOrigin) {
                Image.OriginX = originX;
                Image.OriginY = originY;
            }
            else {
                if (CenterOrigin) Image.CenterOrigin();
            }

            for (var i = 0; i < AdvanceSteps; i++) {
                if (!useSpeedXY) {
                    SpeedX = Util.PolarX(SpeedDir, SpeedLen);
                    SpeedY = Util.PolarY(SpeedDir, SpeedLen);
                }
                X += SpeedX;
                Y += SpeedY;
            }

            Image.Visible = false;
        }

        public override void Update() {
            base.Update();

            // Handle delay
            if (delayTimer < Delay) {
                delayTimer++;
                Timer = 0;
                if (delayTimer == Delay) {
                    Start();
                }
                return;
            }

            Image.Visible = true;

            if (!tweenPosition) {
                if (!useSpeedXY) {
                    SpeedX = Util.PolarX(SpeedDir, SpeedLen);
                    SpeedY = Util.PolarY(SpeedDir, SpeedLen);
                }

                X += SpeedX;
                Y += SpeedY;
            }
            else {
                X = xpos;
                Y = ypos;
            }

            int endFrame;
            if (hasFrameCount) {
                endFrame = FrameOffset + FrameCount;
            }
            else {
                endFrame = Image.Frames;
            }

            if (Animate) {
                var playCount = Loops + 1;
                var frameIndex = (int)Util.ScaleClamp(Timer, 0, LifeSpan, 0, FrameCount * playCount);
                frameIndex %= FrameCount;
                if (useFrameList) {
                    Image.Frame = Frames[frameIndex];
                }
                else {
                    Image.Frame = frameIndex + FrameOffset;
                }
            }
            else {
                Image.Frame = 0;
            }

            Image.ScaleX = ScaleX;
            if (LockScaleRatio) {
                Image.ScaleY = ScaleX;
            }
            else {
                Image.ScaleY = ScaleY;
            }

            if (MotionAngle) {
                Image.Angle = Util.Angle(SpeedX, SpeedY);
            }
            else {
                Image.Angle = Angle;
            }
            Image.Blend = Blend;

            if (hasColor) {
                if (hasFinalColor) {
                    Image.Color = Util.LerpColor(Color, FinalColor, colorLerp);
                }
                else {
                    Image.Color = Color;
                }
                Image.Alpha = Alpha;
            }
            else {
                Image.Color = new Color(ColorR, ColorG, ColorB, Alpha);
            }

            Image.FlippedX = FlipX;
            Image.FlippedY = FlipY;
        }

        public override void Removed() {
            base.Removed();

            Timer = 0;
            ClearGraphics();
        }
    }
}
