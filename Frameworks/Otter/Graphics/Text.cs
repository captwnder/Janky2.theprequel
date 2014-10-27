using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML;
using SFML.Graphics;

namespace Otter {
    /// <summary>
    /// Graphic to use for displaying text in the game.
    /// </summary>
    public class Text : Image {

        SFML.Graphics.Text text;

        /// <summary>
        /// The color to use for the text shadow.
        /// </summary>
        public Color ShadowColor;
        Color outlineColor;

        /// <summary>
        /// Determines if the text's outline will be drawn using a sweeping method.  The sweeping
        /// method will make bigger outlines look better, but also result in more renders.
        /// </summary>
        public bool OutlineSweep = true;

        protected float OutlineDistance = 0;

        /// <summary>
        /// The X offset for drawing the text shadow.
        /// </summary>
        public float ShadowX = 0;

        /// <summary>
        /// The Y offset for drawing the text shadow.
        /// </summary>
        public float ShadowY = 0;

        /// <summary>
        /// The actual string of text to display.
        /// </summary>
        public string String {
            set {
                text.DisplayedString = value;
                Update();
            }
            get {
                return text.DisplayedString;
            }
        }

        /// <summary>
        /// The size of the font to use.
        /// </summary>
        public int FontSize {
            get { return (int)text.CharacterSize; }
            set { text.CharacterSize = (uint)value; }
        }

        /// <summary>
        /// The line spacing between each vertical line.
        /// </summary>
        public int LineSpacing {
            get { return text.Font.GetLineSpacing(text.CharacterSize); }
        }

        //fix for origin y because of weird stuff
        public override void CenterOrigin() {
            OriginX = HalfWidth;
            OriginY = (LineSpacing - (Height % LineSpacing)) + HalfHeight;
        }

        /// <summary>
        /// Shortcut to set both ShadowX and ShadowY.
        /// </summary>
        public float Shadow {
            set {
                ShadowX = value;
                ShadowY = value;
            }
        }

        /// <summary>
        /// Create a new text graphic object.
        /// </summary>
        /// <param name="str">The string to display.</param>
        /// <param name="font">The font to use (defaults to Consolas.)</param>
        /// <param name="size">The size of the font (defaults to 16.)</param>
        public Text(string str, string font = "", int size = 16) : base() {
            if (size < 0) throw new ArgumentException("Font size must be greater than 0.");

            if (font == "") {
                font = "Assets/CONSOLA.TTF";
            }

            text = new SFML.Graphics.Text(str, Fonts.Load(font), (uint)size);
            String = str;
            DrawableSource = text;
            TransformSource = text;
            Width = (int)text.GetLocalBounds().Width;
            Height = (int)text.GetLocalBounds().Height;
            Name = "Text";
        }

        public Text(string str, int size = 16) : this(str, "", size) { }
        public Text(int size = 16) : this("", "", size) { }

        public override void Update() {
            base.Update();
            Width = (int)text.GetLocalBounds().Width;
            Height = (int)text.GetLocalBounds().Height;
        }

        /// <summary>
        /// The text style of the text (bold, italic, etc)
        /// </summary>
        public TextStyle Style {
            set { text.Style = (SFML.Graphics.Text.Styles)value; }
            get { return (TextStyle)text.Style; }
        }

        /// <summary>
        /// The outline color of the text.
        /// </summary>
        public override Color OutlineColor {
            get {
                return outlineColor;
            }
            set {
                outlineColor = value;
            }
        }

        /// <summary>
        /// The thickness of the outline of the text.
        /// </summary>
        public override float OutlineThickness {
            get {
                return OutlineDistance;
            }
            set {
                OutlineDistance = value;
            }
        }

        public override void Render(float x, float y) {
            Color tempColor = Color;

            if (ShadowColor != null) {
                Color = ShadowColor;

                base.Render(x + ShadowX, y + ShadowY);

                Color = tempColor;
            }

            if (OutlineColor != null) {
                Color = OutlineColor;

                var outlineStep = OutlineThickness;
                if (OutlineSweep) {
                    outlineStep = OutlineThickness * 0.5f;
                }

                for (float o = 0; o < OutlineThickness; o += outlineStep) {
                    for (float r = 0; r < 360; r += 45) {
                        var outlinex = Util.PolarX(r, o);
                        var outliney = Util.PolarY(r, o);

                        base.Render(outlinex, outliney);
                    }
                }


                Color = tempColor;
            }

            base.Render(x, y);
        }
    }

    [Flags]
    public enum TextStyle {
        Regular,
        Bold,
        Italic,
        Underlined
    }
}
