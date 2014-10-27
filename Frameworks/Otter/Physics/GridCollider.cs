using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Grid Collider.  Can be mainly used to create collision to correspond to a Tilemap.
    /// </summary>
    public class GridCollider : Collider {

        List<bool> collisions = new List<bool>();

        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public int TileRows { get; private set; }
        public int TileColumns { get; private set; }

        public GridCollider(int width, int height, int tileWidth, int tileHeight, params int[] tags) {
            if (width < 0) throw new ArgumentOutOfRangeException("Width must be greater than 0.");
            if (height < 0) throw new ArgumentOutOfRangeException("Height must be greater than 0.");

            TileColumns = (int)Util.Ceil((float)width / tileWidth);
            TileRows = (int)Util.Ceil((float)height / tileHeight);
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            for (int i = 0; i < TileRows * TileColumns; i++) {
                collisions.Add(false);
            }

            AddTag(tags);
        }

        public override float Width {
            get { return TileColumns * TileWidth; }
        }

        public override float Height {
            get { return TileRows * TileHeight; }
        }

        public void SetTile(int x, int y, bool collidable = true) {
            if (x < 0 || y < 0) return;
            collisions[Util.OneDee((int)TileColumns, (int)x, (int)y)] = collidable;
        }

        public void SetRect(int x, int y, int width, int height, bool collidable = true) {
            for (int i = x; i < x + width; i++) {
                for (int j = y; j < y + height; j++) {
                    SetTile(i, j, collidable);
                }
            }
        }

        public bool GetTile(int x, int y) {
            if (x < 0 || y < 0) return false;
            var index = Util.OneDee((int)TileColumns, (int)x, (int)y);
            if (index >= collisions.Count) return false;
            return collisions[index];
        }

        public bool GetTileAtPosition(float x, float y) {
            var ox = x;
            var oy = y;

            x -= Left;
            y -= Top;

            x = (float)Math.Floor(x / TileWidth);
            y = (float)Math.Floor(y / TileHeight);

            return GetTile((int)x, (int)y);
        }

        /// <summary>
        /// Returns true if any tile in the rect is true.
        /// </summary>
        /// <param name="x">X of the rect in grid space.</param>
        /// <param name="y">Y of the rect in grid space.</param>
        /// <param name="width">Width of the rect in grid space.</param>
        /// <param name="height">Height of the rect in grid space.</param>
        /// <returns></returns>
        public bool GetRect(float x, float y, float x2, float y2, bool usingGrid = true) {
            //adjust for grid position
            x -= Left;
            x2 -= Left;
            y -= Top;
            y2 -= Top;

            if (!usingGrid) {
                x = (int)(Util.SnapToGrid(x, TileWidth) / TileWidth);
                y = (int)(Util.SnapToGrid(y, TileHeight) / TileHeight);
                x2 = (int)(Util.SnapToGrid(x2, TileWidth) / TileWidth);
                y2 = (int)(Util.SnapToGrid(y2, TileHeight) / TileHeight);
            }

            for (int i = (int)x; i <= x2; i++) {
                for (int j = (int)y; j <= y2; j++) {
                    if (GetTile(i, j)) {
                        return true;
                    }
                }
            }

            return false;
        }

        public int GridX(float x) {
            return (int)(Util.SnapToGrid(x, TileWidth) / TileWidth);
        }

        public int GridY(float y) {
            return (int)(Util.SnapToGrid(y, TileHeight) / TileHeight);
        }

        /// <summary>
        /// The area in tile size.
        /// </summary>
        public int TileArea {
            get { return TileRows * TileHeight; }
        }

        /// <summary>
        /// The area in pixels.
        /// </summary>
        public int Area {
            get { return (int)(Width * Height); }
        }

        public void LoadString(string source, string delim = "", char empty = '0', char filled = '1') {
            int xx = 0, yy = 0;

            for (int i = 0; i < source.Length; i++) {
                if (source[i] != empty && source[i] != filled) continue;

                if (xx == TileColumns) {
                    xx = 0;
                    yy++;
                }

                SetTile(xx, yy, source[i] == filled);

                xx++;
            }
        }

        public void LoadCSV(string source, string empty = "0", string filled = "1", char columnSep = ',', char rowSep = '\n') {
            string[] row = source.Split(rowSep);
            int rows = row.Length;
            string[] col;
            int cols;
            int x;
            int y;

            for (y = 0; y < rows; y++) {
                if (row[y] == "") {
                    continue;
                }

                col = row[y].Split(columnSep);
                cols = col.Length;
                for (x = 0; x < cols; x++) {
                    if (col[x].Equals("") || !col[x].Equals(filled)) {
                        continue;
                    }

                    SetTile(x, y, col[x].Equals(filled));
                }
            }
        }

        public override void Render() {
            base.Render();

            if (Entity == null) return;

            for (int i = 0; i < TileColumns; i++) {
                for (int j = 0; j < TileRows; j++) {
                    if (GetTile(i, j)) {
                        Draw.Rectangle(Left + i * TileWidth, Top + j * TileHeight, TileWidth, TileHeight, Color.None, Color.Red, 1f);
                    }
                }
            }
        }
    }
}
