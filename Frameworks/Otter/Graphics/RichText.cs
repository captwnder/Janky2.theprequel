using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Otter {
    /// <summary>
    /// Graphic that renders text with some more options than normal Text.
    /// </summary>
    /// <example>
    /// richText.String = "Hello, {color:f00}this text is red!{clear} {shake:4}Shaking text!";
    /// <code>
    /// Commands:
    ///     {clear} - Clear all styles and reset back to normal, white text.
    ///     {style:name} - Apply the style 'name' to text.  Create styles with AddStyle().
    ///     {color:fff} - Colors text. Strings of 3, 4, 6, or 8 hex digits allowed.
    ///     {color0:fff} - Colors the top left corner of characters.  Strings of 3, 4, 6, or 8 hex digits allowed.
    ///     {color1:fff} - Colors the top right corner of characters.  Strings of 3, 4, 6, or 8 hex digits allowed.
    ///     {color2:fff} - Colors the bottom right corner of characters.  Strings of 3, 4, 6, or 8 hex digits allowed.
    ///     {color3:fff} - Colors the bottom left corner of characters.  Strings of 3, 4, 6, or 8 hex digits allowed.
    ///     {colorShadow:fff} - Colors text shadow. Strings of 3, 4, 6, or 8 hex digits allowed.
    ///     {colorOutline:fff} - Colors text outline. Strings of 3, 4, 6, or 8 hex digits allowed.
    ///     {shadowX:0} - Set the drop shadow of the text on the X axis.
    ///     {shadowY:0} - Set the drop shadow of the text on the Y axis.
    ///     {shadow:0} - Set the drop shadow of the text on the X and Y axes.
    ///     {outline:0} - Set the outline thickness on text.
    ///     {shakeX:0} - Shake the text on the X axis with a float range.
    ///     {shakeY:0} - Shake the text on the Y axis with a float range.
    ///     {shake:0} - Shake the text on the X and Y axes with a float range.
    ///     {waveAmpX:0} - Wave the text on the X axis with a float range.
    ///     {waveAmpY:0} - Wave the text on the Y axis with a float range.
    ///     {waveAmp:0} - Wave the text on the X and Y axes with a float range.
    ///     {waveRateX:0} - Set the wave speed for the X axis.
    ///     {waveRateY:0} - Set the wave speed for the Y axis.
    ///     {waveRate:0} - Set the wave speed for the X and Y axes.
    ///     {waveOffsetX:0} - Set the wave offset for the X axis.
    ///     {waveOffsetY:0} - Set the wave offset for the Y axis.
    ///     {waveOffset:0} - Set the wave offset for the X and Y axes.
    ///     {offset:0} - Set the offset rate for characters.
    /// </code>
    /// </example>
    public class RichText : Image {

        List<RichTextCharacter> chars = new List<RichTextCharacter>();
        List<uint> glyphs = new List<uint>();

        /// <summary>
        /// The alignment of the text.  Left, Right, or Center.
        /// </summary>
        public TextAlign TextAlign = TextAlign.Left;

        /// <summary>
        /// The character used to mark an opening of a command.
        /// </summary>
        public char CommandOpen = '{';

        /// <summary>
        /// The character used to mark the closing of a command.
        /// </summary>
        public char CommandClose = '}';

        /// <summary>
        /// The character used to separate the command with the command value.
        /// </summary>
        public char CommandDelim = ':';

        VertexArray vertices;

        SFML.Graphics.Font font;

        int charSize = 16;

        public bool Monospaced {
            get { return MonospaceWidth > 0; }
        }

        public int MonospaceWidth = -1;

        Color currentCharColor = Color.White;
        Color currentCharColor0 = Color.White;
        Color currentCharColor1 = Color.White;
        Color currentCharColor2 = Color.White;
        Color currentCharColor3 = Color.White;

        Color currentShadowColor = Color.Black;
        Color currentOutlineColor = Color.White;

        float currentSineAmpX = 0;
        float currentSineAmpY = 0;
        float currentSineRateX = 1;
        float currentSineRateY = 1;
        float currentSineOffsetX = 0;
        float currentSineOffsetY = 0;
        float currentOffsetAmount = 10;
        float currentShadowX = 0;
        float currentShadowY = 0;
        float currentOutlineThickness = 0;
        bool currentBold = false;

        float currentShakeX = 0;
        float currentShakeY = 0;

        public float DefaultSineAmpX = 0;
        public float DefaultSineAmpY = 0;
        public float DefaultSineRateX = 1;
        public float DefaultSineRateY = 1;
        public float DefaultSineOffsetX = 0;
        public float DefaultSineOffsetY = 0;
        public float DefaultOffsetAmount = 10;
        public float DefaultShadowX = 0;
        public float DefaultShadowY = 0;
        public float DefaultOutlineThickness = 0;
        public float DefaultShakeX = 0;
        public float DefaultShakeY = 0;
        public Color DefaultCharColor = Color.White;
        public Color DefaultCharColor0 = Color.White;
        public Color DefaultCharColor1 = Color.White;
        public Color DefaultCharColor2 = Color.White;
        public Color DefaultCharColor3 = Color.White;
        public Color DefaultShadowColor = Color.Black;
        public Color DefaultOutlineColor = Color.White;

        public float LineHeight = 1;
        public float LetterSpacing = 1;

        int totalHeight = 0;

        static Dictionary<string, string> styles = new Dictionary<string, string>();

        float timer = 0;

        public int TextWidth {
            get { return textWidth; }
            set {
                textWidth = value;
                Refresh();
            }
        }
        public int TextHeight {
            get { return textHeight; }
            set {
                textHeight = value;
                Refresh();
            }
        }

        int textWidth = -1;
        int textHeight = -1;

        /// <summary>
        /// The line spacing between each vertical line.
        /// </summary>
        public int LineSpacing {
            get { return font.GetLineSpacing((uint)charSize); }
        }

        public bool WordWrap {
            get { return wordWrap; }
            set {
                wordWrap = value;
                Refresh();
            }
        }

        bool wordWrap = false;

        /// <summary>
        /// Create a new RichText object.
        /// </summary>
        /// <param name="str">The string to display. This can include commands to alter text.</param>
        /// <param name="font">The font to use (path to ttf)</param>
        /// <param name="size">The font size to use.</param>
        /// <param name="textWidth">The width of the text box.</param>
        /// <param name="textHeight">The height of the text box.</param>
        public RichText(string str = "", string font = "", int size = 16, int textWidth = -1, int textHeight = -1) : base() {
            if (font == "") {
                font = "Assets/CONSOLA.TTF";
            }
            this.font = Fonts.Load(font);
            charSize = size;
            String = str;
            Texture = new Texture(this.font.GetTexture((uint)size));
            TextWidth = textWidth;
            TextHeight = textHeight;
        }

        public RichText(string str = "", int size = 16) : this(str, "", size) { }
        public RichText(int size = 16) : this("", size) { }

        /// <summary>
        /// Add a global style to RichText objects.  The style will not be updated unless Refresh() is
        /// called on the objects.
        /// </summary>
        /// <example>
        /// RichText.AddStyle("important","color:f00,waveAmpY:2,waveRate:2");
        /// </example>
        /// <param name="name">The name of the style.</param>
        /// <param name="content">The properties to set using commas as a delim character.</param>
        static public void AddStyle(string name, string content) {
            if (styles.ContainsKey(name)) {
                styles[name] = content;
                return;
            }
            styles.Add(name, content);
        }

        /// <summary>
        /// Removes a style from all RichText objects.
        /// </summary>
        /// <param name="name">The name of the style to remove.</param>
        static public void RemoveStyle(string name) {
            styles.Remove(name);
        }

        /// <summary>
        /// Remove all styles from RichText objects.
        /// </summary>
        static public void ClearStyles() {
            styles.Clear();
        }

        /// <summary>
        /// True of the width was not manually set.
        /// </summary>
        public bool AutoWidth {
            get { return TextWidth < 0; }
        }

        /// <summary>
        /// True if the height was not manually set.
        /// </summary>
        public bool AutoHeight {
            get { return TextHeight < 0; }
        }

        /// <summary>
        /// The string to display stripped of all commands.
        /// </summary>
        public string CleanString {
            get {
                var str = "";
                foreach (var c in chars) {
                    str += c.Character.ToString();
                }
                return str;
            }
        }

        int Advance(Glyph glyph) {
            if (Monospaced) return MonospaceWidth;
            return glyph.Advance;
        }

        /// <summary>
        /// Insert new lines into a string to prepare it for word wrapping with this object's width.
        /// This function will not wrap text if AutoWidth is true!
        /// </summary>
        /// <param name="str">The string to wrap.</param>
        /// <returns>The wrapped string.</returns>
        public string PreWrap(string str) {
            if (AutoWidth) return str;

            var finalStr = str;

            var writingText = true;

            int pixels = 0;
            int lastSpaceIndex = 0;

            for (var i = 0; i < str.Length; i++) {
                var c = str[i];
                var glyph = Glyph(c);

                if (c == CommandOpen) {
                    var cmdEnd = str.IndexOf(CommandClose, i + 1);
                    if (cmdEnd >= 0) {
                        writingText = false;
                    }
                }
                if (!writingText) {
                    if (c == CommandClose) {
                        writingText = true;
                    }
                }
                else if (writingText) {
                    if (c == '\t') {
                        pixels += Advance(glyph) * 4;
                    }
                    else if (c == '\n') {
                        pixels = 0;
                    }
                    else {
                        pixels += Advance(glyph);

                        if (c == ' ') {
                            lastSpaceIndex = i;
                        }
                        if (pixels > TextWidth) {
                            StringBuilder sb = new StringBuilder(finalStr);

                            if (lastSpaceIndex < sb.Length) {
                                sb[lastSpaceIndex] = '\n';
                            }
                            var charBeforeSpace = sb[lastSpaceIndex - 1];
                            finalStr = sb.ToString();
                            //i = lastSpaceIndex;
                            pixels = 0;
                        }
                    }
                }
            }

            return finalStr;
        }

        Glyph Glyph(uint charCode) {
            var g = font.GetGlyph(charCode, (uint)charSize, currentBold);

            //update otter texture because SFML font texture updates
            if (!glyphs.Contains(charCode)) {
                Texture = new Texture(font.GetTexture((uint)charSize));
                glyphs.Add(charCode);
            }

            return g;
        }

        /// <summary>
        /// The pixel width of the longest line in the displayed string.
        /// </summary>
        public int LongestLine {
            get {
                var lines = CleanString.Split('\n');
                int longest = 0;
                for (int i = 0; i < NumLines; i++) {
                    int pixels = LineLength(i);
                    longest = Math.Max(longest, pixels);
                }

                return longest;
            }
        }

        /// <summary>
        /// The line length in pixels of a specific line.
        /// </summary>
        /// <param name="lineNumber">The line number to check.</param>
        /// <returns>The length of the line in pixels.</returns>
        public int LineLength(int lineNumber) {
            if (lineNumber < 0 || lineNumber >= NumLines) throw new ArgumentOutOfRangeException("Line doesn't exist in string!");

            var line = Lines[lineNumber];
            int pixels = 0;
            foreach (var c in line) {
                var glyph = Glyph(c);
                if (c == '\t') {
                    pixels += (int)Math.Ceiling(Advance(glyph) * 3 * LetterSpacing);
                }
                pixels += (int)Math.Ceiling(Advance(glyph) * LetterSpacing);
            }
            return pixels;
        }

        /// <summary>
        /// The displayed string broken up into an array by lines.
        /// </summary>
        public string[] Lines {
            get {
                return CleanString.Split('\n');
            }
        }

        /// <summary>
        /// The total number of lines in the displayed string.
        /// </summary>
        public int NumLines {
            get { return Lines.Length; }
        }

        string textString;

        /// <summary>
        /// The string to display.  This string can contain commands to alter the text dynamically.
        /// </summary>
       
        public string String {
            get {
                return textString;
            }
            set {
                textString = value;
                Refresh();
            }
        }

        void ApplyCommand(string command, string args) {
            switch (command) {
                case "color":
                    currentCharColor = new Color(args);
                    break;
                case "color0":
                    currentCharColor0 = new Color(args);
                    break;
                case "color1":
                    currentCharColor1 = new Color(args);
                    break;
                case "color2":
                    currentCharColor2 = new Color(args);
                    break;
                case "color3":
                    currentCharColor3 = new Color(args);
                    break;
                case "colorShadow":
                    currentShadowColor = new Color(args);
                    break;
                case "colorOutline":
                    currentOutlineColor = new Color(args);
                    break;
                case "outline":
                    currentOutlineThickness = float.Parse(args);
                    break;
                case "shakeX":
                    currentShakeX = float.Parse(args);
                    break;
                case "shakeY":
                    currentShakeY = float.Parse(args);
                    break;
                case "shake":
                    currentShakeX = float.Parse(args);
                    currentShakeY = float.Parse(args);
                    break;
                case "waveAmpX":
                    currentSineAmpX = float.Parse(args);
                    break;
                case "waveAmpY":
                    currentSineAmpY = float.Parse(args);
                    break;
                case "waveAmp":
                    currentSineAmpX = float.Parse(args);
                    currentSineAmpY = float.Parse(args);
                    break;
                case "waveRateX":
                    currentSineRateX = float.Parse(args);
                    break;
                case "waveRateY":
                    currentSineRateY = float.Parse(args);
                    break;
                case "waveRate":
                    currentSineRateX = float.Parse(args);
                    currentSineRateY = float.Parse(args);
                    break;
                case "waveOffsetX":
                    currentSineOffsetX = float.Parse(args);
                    break;
                case "waveOffsetY":
                    currentSineOffsetY = float.Parse(args);
                    break;
                case "waveOffset":
                    currentSineOffsetX = float.Parse(args);
                    currentSineOffsetY = float.Parse(args);
                    break;
                case "shadowX":
                    currentShadowX = float.Parse(args);
                    break;
                case "shadowY":
                    currentShadowY = float.Parse(args);
                    break;
                case "shadow":
                    currentShadowX = float.Parse(args);
                    currentShadowY = float.Parse(args);
                    break;
                case "offset":
                    currentOffsetAmount = float.Parse(args);
                    break;
                case "bold":
                    currentBold = int.Parse(args) > 0;
                    break;
            }
        }

        /// <summary>
        /// Refresh the display string re-applying commands.
        /// </summary>
        public void Refresh() {
            chars.Clear();
            Clear();

            var writingText = true;

            //auto word wrap string on input, before parsing?
            if (!AutoWidth && WordWrap) {
                textString = PreWrap(textString);
            }

            writingText = true;

            //create the set of chars with properties and parse commands
            for (var i = 0; i < textString.Length; i++) {
                var c = textString[i];
                if (c == CommandOpen) {
                    //scan for commandclose
                    var cmdEnd = textString.IndexOf(CommandClose, i + 1);
                    if (cmdEnd >= 0) {
                        //only continue of command close character is found
                        writingText = false;
                        var cmd = textString.Substring(i + 1, cmdEnd - i - 1);
                        var cmdSplit = cmd.Split(CommandDelim);
                        var command = cmdSplit[0];
                        if (command == "clear") {
                            Clear();
                        }
                        else if (command == "style") {
                            var args = cmdSplit[1];
                            if (styles.ContainsKey(args)) {
                                var stylestring = styles[args];

                                var styleSplit = stylestring.Split(',');
                                foreach (var str in styleSplit) {
                                    var styleStrSplit = str.Split(CommandDelim);

                                    ApplyCommand(styleStrSplit[0], styleStrSplit[1]);
                                }
                            }
                        }
                        else {
                            ApplyCommand(command, cmdSplit[1]);
                        }

                        continue;
                    }
                }
                if (c == CommandClose) {
                    writingText = true;
                    continue;
                }
                if (writingText) {
                    var rtchar = new RichTextCharacter(c, i) {
                        SineAmpX = currentSineAmpX,
                        SineAmpY = currentSineAmpY,
                        SineRateX = currentSineRateX,
                        SineRateY = currentSineRateY,
                        SineOffsetX = currentSineOffsetX,
                        SineOffsetY = currentSineOffsetY,
                        OffsetAmount = currentOffsetAmount,
                        ShadowX = currentShadowX,
                        ShadowY = currentShadowY,
                        ShadowColor = currentShadowColor,
                        OutlineThickness = currentOutlineThickness,
                        OutlineColor = currentOutlineColor,
                        Color = currentCharColor,
                        Color0 = currentCharColor0,
                        Color1 = currentCharColor1,
                        Color2 = currentCharColor2,
                        Color3 = currentCharColor3,
                        ShakeX = currentShakeX,
                        ShakeY = currentShakeY,
                        Timer = timer
                    };

                    chars.Add(rtchar);
                }
            }

            totalHeight = (int)Math.Ceiling(NumLines * font.GetLineSpacing((uint)charSize) * LineHeight);

            UpdateGeometry();
        }

        void Clear() {
            currentCharColor = DefaultCharColor;
            currentCharColor0 = DefaultCharColor0;
            currentCharColor1 = DefaultCharColor1;
            currentCharColor2 = DefaultCharColor2;
            currentCharColor3 = DefaultCharColor3;
            currentShadowColor = DefaultShadowColor;
            currentOutlineColor = DefaultOutlineColor;
            currentOutlineThickness = DefaultOutlineThickness;
            currentShadowX = DefaultShadowX;
            currentShadowY = DefaultShadowY;
            currentShakeX = DefaultShakeX;
            currentShakeY = DefaultShakeY;
            currentSineAmpX = DefaultSineAmpX;
            currentSineAmpY = DefaultSineAmpY;
            currentSineOffsetX = DefaultSineOffsetX;
            currentSineOffsetY = DefaultSineOffsetY;
            currentSineRateX = DefaultSineRateX;
            currentSineRateY = DefaultSineRateY;
            currentOffsetAmount = DefaultOffsetAmount;
        }

        void UpdateGeometry() {

            vertices = new VertexArray(PrimitiveType.Quads);

            float nextX = 0;
            float nextY = 0;

            int currentLine = 0;

            nextX = LineStartPosition(currentLine);
            nextY += charSize;

            for (var i = 0; i < chars.Count; i++) {
                var c = chars[i].Character;
                var glyph = Glyph(c);
                var rect = glyph.TextureRect;
                var bounds = glyph.Bounds;


                if (c == '\t') {
                    nextX += Advance(glyph) * 4 * LetterSpacing;
                }
                else if (c == '\n') {
                    nextY += font.GetLineSpacing((uint)charSize) * LineHeight;
                    currentLine++;
                    nextX = LineStartPosition(currentLine);
                }
                else if (c == '\v') {
                    nextY += font.GetLineSpacing((uint)charSize) * 4 * LineHeight;
                    currentLine += 4;
                    nextX = LineStartPosition(currentLine);
                }
                else {
                    var cx = chars[i].OffsetX;
                    var cy = chars[i].OffsetY;

                    //draw shadow

                    Color nextColor;

                    if (chars[i].ShadowX != 0 || chars[i].ShadowY != 0) {
                        var shadowx = cx + chars[i].ShadowX;
                        var shadowy = cy + chars[i].ShadowY;

                        nextColor = chars[i].ShadowColor * Color;

                        vertices.Append(shadowx + nextX + bounds.Left, shadowy + nextY + bounds.Top, nextColor, rect.Left, rect.Top);
                        vertices.Append(shadowx + nextX + bounds.Left + bounds.Width, shadowy + nextY + bounds.Top, nextColor, rect.Left + rect.Width, rect.Top);
                        vertices.Append(shadowx + nextX + bounds.Left + bounds.Width, shadowy + nextY + bounds.Top + bounds.Height, nextColor, rect.Left + rect.Width, rect.Top + rect.Height);
                        vertices.Append(shadowx + nextX + bounds.Left, shadowy + nextY + bounds.Top + bounds.Height, nextColor, rect.Left, rect.Top + rect.Height);
                    }
                    //draw outline
                    if (chars[i].OutlineThickness > 0) {
                        var outline = chars[i].OutlineThickness;
                        nextColor = chars[i].OutlineColor * Color;

                        for (float o = 0; o < outline; o += outline * 0.5f) {
                            for (float r = 0; r < 360; r += 45) {
                                var outlinex = Util.PolarX(r, o) + cx;
                                var outliney = Util.PolarY(r, o) + cy;

                                vertices.Append(outlinex + nextX + bounds.Left, outliney + nextY + bounds.Top, nextColor, rect.Left, rect.Top);
                                vertices.Append(outlinex + nextX + bounds.Left + bounds.Width, outliney + nextY + bounds.Top, nextColor, rect.Left + rect.Width, rect.Top);
                                vertices.Append(outlinex + nextX + bounds.Left + bounds.Width, outliney + nextY + bounds.Top + bounds.Height, nextColor, rect.Left + rect.Width, rect.Top + rect.Height);
                                vertices.Append(outlinex + nextX + bounds.Left, outliney + nextY + bounds.Top + bounds.Height, nextColor, rect.Left, rect.Top + rect.Height);
                            }
                        }
                    }

                    //draw character

                    nextColor = chars[i].Color.Copy() * Color;
                    nextColor *= chars[i].Color0;
                    
                    vertices.Append(cx + nextX + bounds.Left, cy + nextY + bounds.Top, nextColor, rect.Left, rect.Top);

                    nextColor = chars[i].Color.Copy() * Color;
                    nextColor *= chars[i].Color1;

                    vertices.Append(cx + nextX + bounds.Left + bounds.Width, cy + nextY + bounds.Top, nextColor, rect.Left + rect.Width, rect.Top);

                    nextColor = chars[i].Color.Copy() * Color;
                    nextColor *= chars[i].Color2;

                    vertices.Append(cx + nextX + bounds.Left + bounds.Width, cy + nextY + bounds.Top + bounds.Height, nextColor, rect.Left + rect.Width, rect.Top + rect.Height);

                    nextColor = chars[i].Color.Copy() * Color;
                    nextColor *= chars[i].Color3;

                    vertices.Append(cx + nextX + bounds.Left, cy + nextY + bounds.Top + bounds.Height, nextColor, rect.Left, rect.Top + rect.Height);

                    nextX += Advance(glyph) * LetterSpacing;
                }
            }

            DrawableSource = vertices;

            if (AutoWidth) {
                Width = LongestLine;
            }
            else {
                Width = TextWidth;
            }

            if (AutoHeight) {
                Height = totalHeight;
            }
            else {
                Height = TextHeight;
            }
        }

        int LineStartPosition(int lineNumber) {
            int lineStart = 0;
            int lineLength = LineLength(lineNumber);
            switch (TextAlign) {
                case TextAlign.Left:
                    lineStart = 0;
                    break;
                case TextAlign.Center:
                    lineStart = (Width - lineLength) / 2;
                    break;
                case TextAlign.Right:
                    lineStart = Width - lineLength;
                    break;
            }
            return lineStart;
        }

        public override void Update() {
            base.Update();

            timer += Game.Instance.DeltaTime;

            foreach (var c in chars) {
                c.Update();
            }

            UpdateGeometry();
        }

        bool debugRendering = false;
        public override void Render(float x = 0, float y = 0) {
            if (debugRendering) {
                Draw.Rectangle(X + x, Y + y, Width, Height, Color.None, Color.Red, 1);
            }

            if (DrawableSource != null) {
                base.Render(x, y);
            }
        }
    }

    public enum TextAlign {
        Left,
        Right,
        Center
    }

    /// <summary>
    /// Internal class for managing characters in RichText.
    /// </summary>
    class RichTextCharacter {

        //base color, 4 corner colors, sine rate, sine amplitude, sine offset, shake x, shake y
        public Color Color = Color.White;

        public Color Color0 = Color.White;
        public Color Color1 = Color.White;
        public Color Color2 = Color.White;
        public Color Color3 = Color.White;

        public Color ShadowColor = Color.Black;
        public Color OutlineColor = Color.White;

        public char Character;

        public float Timer = 0;

        public float SineAmpX = 0;
        public float SineAmpY = 0;
        public float SineRateX = 1;
        public float SineRateY = 1;
        public float SineOffsetX = 0;
        public float SineOffsetY = 0;
        public float CharOffset = 0;
        public float OffsetAmount = 10;
        public float ShadowX = 0;
        public float ShadowY = 0;
        public float OutlineThickness = 0;

        public float ShakeX = 0;
        public float ShakeY = 0;

        float finalShakeX = 0;
        float finalShakeY = 0;
        float finalSinX = 0;
        float finalSinY = 0;

        public bool Bold = false;

        public RichTextCharacter(char character, int charOffset = 0) {
            Character = character;
            CharOffset = charOffset;
        }

        public void Update() {
            Timer += Game.Instance.DeltaTime;

            finalShakeX = Rand.Float(-ShakeX, ShakeX);
            finalShakeY = Rand.Float(-ShakeY, ShakeY);
            finalSinX = Util.SinScale((Timer + SineOffsetX - CharOffset * OffsetAmount) * SineRateX, -SineAmpX, SineAmpX);
            finalSinY = Util.SinScale((Timer + SineOffsetY - CharOffset * OffsetAmount) * SineRateY, -SineAmpY, SineAmpY); 
        }

        public float OffsetX {
            get {
                return finalShakeX + finalSinX;
            }
        }

        public float OffsetY {
            get {
                return finalShakeY + finalSinY;
            }
        }

        internal void Append(VertexArray vertices, float x, float y) {
            var col = new Color(Color0);
            col.R *= Color.R;
            col.G *= Color.G;
            col.B *= Color.B;
            col.A *= Color.A;
        }
    }
}
