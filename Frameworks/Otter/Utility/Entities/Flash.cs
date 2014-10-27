using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Entity that acts as a screen flash.  Best used when using the constructor that allows for
    /// initial parameters be set:
    /// <example>
    /// Flash(Color.Red) { Alpha = 0.5, Blend = BlendMode.Add };
    /// </example>
    /// </summary>
    public class Flash : Entity {
        Image imgFlash;

        public static int DefaultLifeSpan = 60;

        public Color Color;
        public float Alpha = 1;
        public float FinalAlpha = 0;
        public BlendMode Blend = BlendMode.Alpha;

        float scale;
        bool hasScale;
        public float Scale {
            set {
                scale = value;
                hasScale = true;
            }
            get { return scale; }
        }

        public Flash(Color color) : base(0, 0) {
            Color = color;
        }

        public override void Added() {
            base.Added();

            if (LifeSpan == 0) {
                LifeSpan = DefaultLifeSpan;
            }

            imgFlash = Image.CreateRectangle(Game.Instance.Width, Game.Instance.Height, Color);
            imgFlash.Blend = Blend;
            imgFlash.Scroll = 0;
            imgFlash.CenterOriginZero();
            if (Surface != null) {
                imgFlash.Scale = 1 / Surface.CameraZoom;
            }
            else {
                imgFlash.Scale = 1 / Game.Surface.CameraZoom;
            }
            SetGraphic(imgFlash);
        }

        public override void Update() {
            base.Update();

            if (Surface != null) {
                imgFlash.Scale = 1 / Surface.CameraZoom;
            }
            else {
                imgFlash.Scale = 1 / Game.Surface.CameraZoom;
            }

            imgFlash.Alpha = Util.ScaleClamp(Timer, 0, LifeSpan, Alpha, FinalAlpha);
        }

        public override void Removed() {
            base.Removed();

            ClearGraphics();
        }
    }
}
