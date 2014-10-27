using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Otter {
    /// <summary>
    /// Graphic type used to render a panel made up of 9 slices of an image. Handy for rendering panels
    /// with border graphics.
    /// </summary>
    public class NineSlice : Image {
        VertexArray vertices;

        string source;

        PanelType paneltype;

        int sliceX1, sliceX2, sliceY1, sliceY2;

        /// <summary>
        /// Draw the panel from the top left corner of the middle slice.
        /// </summary>
        public bool UseInsideOrigin;

        public PanelType PanelType {
            get {
                return paneltype;
            }
            set {
                paneltype = value;
                NeedsUpdate = true;
            }
        }

        public bool SnapWidth = false;
        public bool SnapHeight = false;

        public Rectangle PanelClip {
            set {
                panelClip = value;
                usePanelClip = true;
                NeedsUpdate = true;
            }
            get {
                return panelClip;
            }
        }

        public bool UsePanelClip {
            set { usePanelClip = value; }
            get { return usePanelClip; }
        }

        bool usePanelClip;
        Rectangle panelClip;

        /// <summary>
        /// Determines how the size of the panel will be adjusted when setting PanelWidth and PanelHeight.
        /// If set to All, the entire panel will be the width and height.
        /// If set to Inside, the inside of the panel will be the width and height.
        /// </summary>
        public PanelSizeMode PanelSizeMode = PanelSizeMode.All;

        int
            tWidth,
            tHeight;

        public NineSlice(string source, int width = 0, int height = 0, Atlas atlas = null) : base() {
            if (atlas != null) {
                var a = atlas.GetTexture(source);
                Texture = a.Texture;
                atlasRect.Left = a.X + a.FrameX;
                atlasRect.Top = a.Y + a.FrameY;
                atlasRect.Width = a.FrameWidth;
                atlasRect.Height = a.FrameHeight;
                tWidth = a.Width;
                tHeight = a.Height;
            }
            else {
                Texture = new Texture(source);
                tWidth = (int)Texture.Width;
                tHeight = (int)Texture.Height;
            }

            this.source = source;

            if (width == 0 || height == 0) {
                Width = tWidth;
                Height = tHeight;
            }
            else {
                Width = width;
                Height = height;
            }

            NeedsUpdate = true;
            UpdateSlices();
        }

        public NineSlice(Atlas atlas, string source, int width = 0, int height = 0) : this(source, width, height, atlas) { }

        public int TextureWidthThird { get { return tWidth / 3; } }
        public int TextureHeightThird { get { return tHeight / 3; } }

        void DrawQuad(VertexArray v, float x1, float y1, float x2, float y2, float u1, float v1, float u2, float v2) {
            float cx1 = x1, cx2 = x2, cy1 = y1, cy2 = y2;
            float cu1 = u1, cu2 = u2, cv1 = v1, cv2 = v2;

            if (usePanelClip) {
                cx1 = Util.Clamp(x1, PanelClip.Left, PanelClip.Right);
                cu1 = Util.ScaleClamp(cx1, x1, x2, u1, u2);

                cx2 = Util.Clamp(x2, PanelClip.Left, PanelClip.Right);
                cu2 = Util.ScaleClamp(cx2, x1, x2, u1, u2);

                cy1 = Util.Clamp(y1, PanelClip.Top, PanelClip.Bottom);
                cv1 = Util.ScaleClamp(cy1, y1, y2, v1, v2);

                cy2 = Util.Clamp(y2, PanelClip.Top, PanelClip.Bottom);
                cv2 = Util.ScaleClamp(cy2, y1, y2, v1, v2);
            }

            v.Append(cx1, cy1, color, cu1, cv1);
            v.Append(cx2, cy1, color, cu2, cv1);
            v.Append(cx2, cy2, color, cu2, cv2);
            v.Append(cx1, cy2, color, cu1, cv2);
        }

        void UpdateSlices() {
            if (!NeedsUpdate) return;

            var v = new VertexArray(PrimitiveType.Quads);

            var thirdWidth = tWidth / 3;
            var thirdHeight = tHeight / 3;
            
            if (PanelType == PanelType.Stretch) {
                //top
                DrawQuad(v,
                    thirdWidth,
                    0,
                    Width - thirdWidth,
                    thirdHeight,
                    atlasRect.Left + thirdWidth,
                    atlasRect.Top,
                    atlasRect.Left + thirdWidth * 2,
                    atlasRect.Top + thirdHeight);

                //left
                DrawQuad(v,
                    0,
                    thirdHeight,
                    thirdWidth,
                    Height - thirdHeight,
                    atlasRect.Left,
                    atlasRect.Top + thirdHeight,
                    atlasRect.Left + thirdWidth,
                    atlasRect.Top + thirdHeight * 2);

                //right
                DrawQuad(v,
                    Width - thirdWidth,
                    thirdHeight,
                    Width,
                    Height - thirdHeight,
                    atlasRect.Left + thirdWidth * 2,
                    atlasRect.Top + thirdHeight,
                    atlasRect.Left + thirdWidth * 3,
                    atlasRect.Top + thirdHeight * 2);

                //bottom
                DrawQuad(v,
                    thirdWidth,
                    Height - thirdHeight,
                    Width - thirdWidth,
                    Height,
                    atlasRect.Left + thirdWidth,
                    atlasRect.Top + thirdHeight * 2,
                    atlasRect.Left + thirdWidth * 2,
                    atlasRect.Top + thirdHeight * 3);

                //middle
                DrawQuad(v,
                    thirdWidth,
                    thirdHeight,
                    Width - thirdWidth,
                    Height - thirdHeight,
                    atlasRect.Left + thirdWidth,
                    atlasRect.Top + thirdHeight,
                    atlasRect.Left + thirdWidth * 2,
                    atlasRect.Top + thirdHeight * 2);
            }
            else {
                for (int xx = thirdWidth; xx < Width - thirdWidth; xx += thirdWidth) {
                    for (int yy = thirdHeight; yy < Height - thirdHeight; yy += thirdHeight) {
                        //middle
                        DrawQuad(v,
                            xx,
                            yy,
                            xx + thirdWidth,
                            yy + thirdHeight,
                            atlasRect.Left + thirdWidth,
                            atlasRect.Top + thirdHeight,
                            atlasRect.Left + thirdWidth * 2,
                            atlasRect.Top + thirdHeight * 2);

                        //left
                        DrawQuad(v,
                            0,
                            yy,
                            thirdWidth,
                            yy + thirdHeight,
                            atlasRect.Left,
                            atlasRect.Top + thirdHeight,
                            atlasRect.Left + thirdWidth,
                            atlasRect.Top + thirdHeight * 2);

                        //right
                        DrawQuad(v,
                            Width - thirdWidth,
                            yy,
                            Width,
                            yy + thirdHeight,
                            atlasRect.Left + thirdWidth * 2,
                            atlasRect.Top + thirdHeight,
                            atlasRect.Left + thirdWidth * 3,
                            atlasRect.Top + thirdHeight * 2);
                    }

                    //top
                    DrawQuad(v,
                        xx,
                        0,
                        xx + thirdWidth,
                        thirdHeight,
                        atlasRect.Left + thirdWidth,
                        atlasRect.Top,
                        atlasRect.Left + thirdWidth * 2,
                        atlasRect.Top + thirdHeight);

                    //bottom
                    DrawQuad(v,
                        xx,
                        Height - thirdHeight,
                        xx + thirdWidth,
                        Height,
                        atlasRect.Left + thirdWidth,
                        atlasRect.Top + thirdHeight * 2,
                        atlasRect.Left + thirdWidth * 2,
                        atlasRect.Top + thirdWidth * 3);
                }
            }

            //top left
            DrawQuad(v,
                0,
                0,
                thirdWidth,
                thirdHeight,
                atlasRect.Left,
                atlasRect.Top,
                atlasRect.Left + thirdWidth,
                atlasRect.Top + thirdHeight);

            //top right
            DrawQuad(v,
                Width - thirdWidth,
                0,
                Width,
                thirdHeight,
                atlasRect.Left + thirdWidth * 2,
                atlasRect.Top,
                atlasRect.Left + thirdWidth * 3,
                atlasRect.Top + thirdHeight);

            //bottom left
            DrawQuad(v,
                0,
                Height - thirdHeight,
                thirdWidth,
                Height,
                atlasRect.Left,
                atlasRect.Top + thirdHeight * 2,
                atlasRect.Left + thirdWidth,
                atlasRect.Top + thirdHeight * 3);

            //bottom right
            DrawQuad(v,
                Width - thirdWidth,
                Height - thirdHeight,
                Width,
                Height,
                atlasRect.Left + thirdWidth * 2,
                atlasRect.Top + thirdHeight * 2,
                atlasRect.Left + thirdWidth * 3,
                atlasRect.Top + thirdHeight * 3);

            vertices = v;

            DrawableSource = vertices;

            NeedsUpdate = false;
        }

        void Append(VertexArray v, float x, float y, float tx, float ty) {
            v.Append(new Vertex(new Vector2f(x, y), new Vector2f(tx, ty)));
        }

        public override void Update() {
            UpdateSlices();
        }

        public int PanelWidth {
            set {
                if (PanelSizeMode == PanelSizeMode.Inside) {
                    value += tWidth / 3 * 2;
                }

                if (SnapWidth) {
                    Width = (int)Util.SnapToGrid(value, tWidth / 3);
                }
                else {
                    Width = value;
                }
                if (Width < 1) Width = 1;
                NeedsUpdate = true;
            }
            get {
                if (PanelSizeMode == PanelSizeMode.Inside) {
                    return Width - tWidth / 3 * 2;
                }

                return Width;
            }
        }

        public int PanelHeight {
            set {
                if (PanelSizeMode == PanelSizeMode.Inside) {
                    value += tHeight / 3 * 2;
                }

                if (SnapWidth) {
                    Height = (int)Util.SnapToGrid(value, tHeight / 3);
                }
                else {
                    Height = value;
                }
                if (Height < 1) Height = 1;
                NeedsUpdate = true;
            }
            get {
                if (PanelSizeMode == PanelSizeMode.Inside) {
                    return Height - tHeight / 3 * 2;
                }

                return Height;
            }
        }

        public override void Render(float x = 0, float y = 0) {
            float ox = 0, oy = 0;

            if (UseInsideOrigin) {
                ox = tWidth / -3;
                oy = tHeight / -3;
            }
            
            base.Render(x + ox, y + oy);
        }
    }

    public enum PanelType {
        Stretch,
        Tile
    }

    public enum PanelSizeMode {
        All,
        Inside
    }
}
