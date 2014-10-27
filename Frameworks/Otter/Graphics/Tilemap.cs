using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML;
using SFML.Graphics;
using SFML.Window;
using System.Xml;

namespace Otter {
    /// <summary>
    /// Graphic used for loading and rendering a tilemap.  Renders tiles using a vertex array.
    /// Tilemaps have built in layers but they're still a work in progress and probably shouldn't be
    /// used yet.
    /// </summary>
    public class Tilemap : Image {

        VertexArray vertices;


        string source;

        /// <summary>
        /// The width in pixels of each tile.
        /// </summary>
        public int TileWidth { get; private set; }

        /// <summary>
        /// The height in pixels of each tile.
        /// </summary>
        public int TileHeight { get; private set; }

        /// <summary>
        /// The number of rows in the entire tilemap.
        /// </summary>
        public int TileRows { get; private set; }

        /// <summary>
        /// The number of columsn in the entire tilemap.
        /// </summary>
        public int TileColumns { get; private set; }

        /// <summary>
        /// Determines if the X and Y coordinates of tiles are interpreted as pixels or tile coords.
        /// </summary>
        public bool UsePositions = false;

        public SortedDictionary<int, List<TileInfo>> TileLayers { get; private set; }

        Dictionary<string, int> layerNames = new Dictionary<string, int>();

        /// <summary>
        /// The default layer name to use.
        /// </summary>
        public string DefaultLayerName = "base";

        int
            sourceColumns,
            sourceRows;
        
        /// <summary>
        /// Create a new tilemap.
        /// </summary>
        /// <param name="width">The width in pixels of the tilemap.</param>
        /// <param name="height">The height in pixels of the tilemap.</param>
        /// <param name="tileWidth">The width of each tile.</param>
        /// <param name="tileHeight">The height of each tile.</param>
        /// <param name="source">The file path to load the texture from, or the name of the texture in an atlas.</param>
        /// <param name="atlas">The atlas to use to load from.</param>
        public Tilemap(int width, int height, int tileWidth, int tileHeight, string source = "", Atlas atlas = null) : base() {
            if (width < 0) throw new ArgumentOutOfRangeException("Width must be greater than 0.");
            if (height < 0) throw new ArgumentOutOfRangeException("Height must be greater than 0.");

            TileLayers = new SortedDictionary<int, List<TileInfo>>();

            AddLayer(DefaultLayerName);

            TileWidth = tileWidth;
            TileHeight = tileHeight;

            TileColumns = (int)Util.Ceil((float)width / tileWidth);
            TileRows = (int)Util.Ceil((float)height / tileHeight);

            UpdateVertices();

            if (atlas == null && source != null) {
                //if no source found, try to load it from the atlas
                if (!Textures.Exists(source)) {
                    atlas = Game.Instance.Atlas;
                    source = source.Replace(".png", "");
                }
            }

            if (source != "") {
                if (atlas != null) {
                    var a = atlas.GetTexture(source);
                    Texture = a.Texture;
                    atlasRect.Left = a.X;
                    atlasRect.Top = a.Y;
                    atlasRect.Width = a.Width;
                    atlasRect.Height = a.Height;
                }
                else {
                    Texture = new Texture(source);
                    atlasRect.Width = (int)Texture.Width;
                    atlasRect.Height = (int)Texture.Height;
                }
                this.source = source;
                sourceColumns = (int)(atlasRect.Width / tileWidth);
                sourceRows = (int)(atlasRect.Height / tileHeight);
            }

            Width = width;
            Height = height;
        }

        public Tilemap(int size, int tileSize, string source = "", Atlas atlas = null) : this(size, size, tileSize, tileSize, source, atlas) { }

        /// <summary>
        /// Set a tile to a specific color.
        /// </summary>
        /// <param name="tileX">The tile's x coordinate on the map.</param>
        /// <param name="tileY">The tile's y coordinate on the map.</param>
        /// <param name="color">The tile's color.</param>
        public void SetTile(int tileX, int tileY, Color color, string layer = "") {
            if (layer == "") layer = DefaultLayerName;

            if (!UsePositions) {
                tileX *= TileWidth;
                tileY *= TileHeight;
            }

            var t = new TileInfo(tileX, tileY, -1, -1, TileWidth, TileHeight, color);
            TileLayers[layerNames[layer]].Add(t);

            NeedsUpdate = true;
        }

        /// <summary>
        /// Set a tile to a specific tile from the source texture.
        /// </summary>
        /// <param name="tileX">The tile's x coordinate on the map.</param>
        /// <param name="tileY">The tile's y coordinate on the map.</param>
        /// <param name="sourceX">The source X coordinate from the tile map in pixels.</param>
        /// <param name="sourceY">The source Y coordinate from the tile map in pixels.</param>
        public TileInfo SetTile(int tileX, int tileY, int sourceX, int sourceY, string layer = "") {
            sourceX += atlasRect.Left;
            sourceY += atlasRect.Top;

            if (layer == "") layer = DefaultLayerName;

            if (!UsePositions) {
                tileX *= TileWidth;
                tileY *= TileHeight;
            }

            var t = new TileInfo(tileX, tileY, sourceX, sourceY, TileWidth, TileHeight);

            TileLayers[layerNames[layer]].Add(t);

            NeedsUpdate = true;

            return t;
        }

        /// <summary>
        /// Load tile data from an XmlElement.
        /// </summary>
        /// <param name="e">An XmlElement containing attributes x, y, tx, and ty.</param>
        /// <returns>The TileInfo for the loaded tile.</returns>
        public TileInfo SetTile(XmlElement e) {
            int x, y, tx, ty;
            if (UsePositions) {
                x = e.AttributeInt("x") * TileWidth;
                y = e.AttributeInt("y") * TileHeight;
            }
            else {
                x = e.AttributeInt("x");
                y = e.AttributeInt("y");
            }
            tx = e.AttributeInt("tx") * TileWidth;
            ty = e.AttributeInt("ty") * TileHeight;
            return SetTile(x, y, tx, ty);
        }

        /// <summary>
        /// Set a tile on the tilemap to a specific tile.
        /// </summary>
        /// <param name="tileX">The X position of the tile to change.</param>
        /// <param name="tileY">The Y position of the tile to change.</param>
        /// <param name="tileIndex">The index of the tile to change to.</param>
        /// <returns>The TileInfo from the altered tile.</returns>
        public TileInfo SetTile(int tileX, int tileY, int tileIndex) {
            int sourceX = (int)(Util.TwoDeeX((int)tileIndex, (int)sourceColumns) * TileWidth);
            int sourceY = (int)(Util.TwoDeeY((int)tileIndex, (int)sourceColumns) * TileHeight);
            return SetTile(tileX, tileY, sourceX, sourceY);
        }

        /// <summary>
        /// Set a rectangle area of tiles to a defined color.
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <param name="color"></param>
        public void SetRect(int tileX, int tileY, int tileWidth, int tileHeight, Color color) {
            for (int xx = tileX; xx < tileX + tileWidth; xx++) {
                for (int yy = tileY; yy < tileY + tileHeight; yy++) {
                    SetTile(xx, yy, color);
                }
            }
        }

        /// <summary>
        /// Set a rectangle of tiles to a tile defined by texture coordinates.
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <param name="sourceX"></param>
        /// <param name="sourceY"></param>
        public void SetRect(int tileX, int tileY, int tileWidth, int tileHeight, int sourceX, int sourceY) {
            for (int xx = tileX; xx < tileX + tileWidth; xx++) {
                for (int yy = tileY; yy < tileY + tileHeight; yy++) {
                    SetTile(xx, yy, sourceX, sourceY);
                }
            }
        }

        /// <summary>
        /// Set a rectangle of tiles to a tile defined by an index.
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <param name="tileIndex"></param>
        public void SetRect(int tileX, int tileY, int tileWidth, int tileHeight, int tileIndex) {
            for (int xx = tileX; xx < tileX + tileWidth; xx++) {
                for (int yy = tileY; yy < tileY + tileHeight; yy++) {
                    SetTile(xx, yy, tileIndex);
                }
            }
        }

        /// <summary>
        /// Get the TileInfo of a specific tile on the tilemap.
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public TileInfo GetTile(int tileX, int tileY, string layer = "") {
            if (layer == "") layer = DefaultLayerName;

            foreach (var t in TileLayers[layerNames[layer]]) {
                if (t.X == tileX) {
                    if (t.Y == tileY) {
                        return t;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Load tiles in from a GridCollider.
        /// </summary>
        /// <param name="grid">The GridCollider to reference.</param>
        /// <param name="color">The color to set tiles that are collidable on the grid.</param>
        public void LoadGrid(GridCollider grid, Color color) {
            for (var i = 0; i < grid.TileColumns; i++) {
                for (var j = 0; j < grid.TileRows; j++) {
                    if (grid.GetTile(i, j)) {
                        SetTile(i, j, color);
                    }
                }
            }
        }

        /// <summary>
        /// Get the layer name for a specific layer on the tilemap.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public string LayerName(int layer) {
            foreach (var l in layerNames) {
                if (l.Value == layer) {
                    return l.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the layer depth of a layer on the tilemap by name.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public int LayerDepth(string layer) {
            return layerNames[layer];
        }

        /// <summary>
        /// Load the tilemap with a color based on a string.
        /// </summary>
        /// <param name="source">The string data to load.</param>
        /// <param name="color">The color to fill occupied tiles with.</param>
        /// <param name="empty">The character that represents an empty tile.</param>
        /// <param name="filled">The character that represents a filled tile.</param>
        public void LoadString(string source, Color color = null, char empty = '0', char filled = '1') {
            int xx = 0, yy = 0;

            if (color == null) {
                color = Color.Red;
            }

            for (int i = 0; i < source.Length; i++) {
                if (source[i] != empty && source[i] != filled) continue;

                if (xx == TileColumns) {
                    xx = 0;
                    yy++;
                }

                if (source[i] == filled) {
                    SetTile(xx, yy, color);
                }

                xx++;
            }
        }

        /// <summary>
        /// Load the tilemap from a CSV formatted string.
        /// </summary>
        /// <param name="str">The string data to load.</param>
        /// <param name="columnSep">The character that separates columns in the CSV.</param>
        /// <param name="rowSep">The character that separates rows in the CSV.</param>
        public void LoadCSV(string str, char columnSep = ',', char rowSep = '\n') {
            bool u = UsePositions;
            UsePositions = false;

            string[] row = str.Split(rowSep);
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
                    if (col[x].Equals("") || Convert.ToInt32(col[x]) < 0) {
                        continue;
                    }

                    SetTile(x, y, Convert.ToInt16(col[x]));
                }
            }

            UsePositions = u;
        }

        /// <summary>
        /// Remove a tile from the tilemap.
        /// </summary>
        /// <param name="tileX">The tile's x coordinate on the map.</param>
        /// <param name="tileY">The tile's y coordinate on the map.</param>
        public TileInfo ClearTile(int tileX, int tileY, string layer = "") {
            if (layer == "") layer = DefaultLayerName;

            var t = GetTile(tileX, tileY, layer);

            if (t != null) {
                TileLayers[layerNames[layer]].Remove(t);
            }

            NeedsUpdate = true;

            return t;
        }

        public void ClearLayer(string layer = "") {
            if (layer == "") layer = DefaultLayerName;

            TileLayers[layerNames[layer]].Clear();
        }

        public void ClearAll() {
            foreach (var kv in TileLayers) {
                kv.Value.Clear();
            }
        }

        public override void Update() {
            base.Update();

            UpdateVertices();
        }

        void UpdateVertices() {
            if (!NeedsUpdate) return;

            vertices = new VertexArray(PrimitiveType.Quads);

            foreach (var layer in TileLayers) {
                foreach (var tile in layer.Value) {
                    tile.Alpha = alpha;
                    tile.AppendVertices(vertices);
                }
            }

            DrawableSource = vertices;

            NeedsUpdate = false;
        }

        /// <summary>
        /// Add a new layer to the tilemap.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public int AddLayer(string name, int depth = 0) {
            layerNames.Add(name, depth);
            TileLayers.Add(depth, new List<TileInfo>());
            return TileLayers.Count - 1;
        }

        public bool LayerExists(string name) {
            return TileLayers.ContainsKey(layerNames[name]);
        }

        /// <summary>
        /// Merges another tilemap into this one.
        /// </summary>
        /// <param name="other">The tilemap to merge into this one.</param>
        /// <param name="above">True if the other tilemap's base layer should be above this one's base layer.</param>
        public void MergeTilemap(Tilemap other, string layerPrefix = "", int layerOffset = -1) {
            foreach (var layer in other.TileLayers) {
                AddLayer(layerPrefix + other.LayerName(layer.Key), layer.Key + layerOffset);
                TileLayers[layer.Key + layerOffset] = new List<TileInfo>(other.TileLayers[layer.Key]);
            }
        }
    }

    /// <summary>
    /// A class containing all the info to describe a specific tile.
    /// </summary>
    public class TileInfo {
        /// <summary>
        /// The X position of the tile.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y position of the tile.
        /// </summary>
        public int Y;

        /// <summary>
        /// The X position of the source texture to render the tile from.
        /// </summary>
        public int TX;

        /// <summary>
        /// The Y position of the source texture to render the tile from.
        /// </summary>
        public int TY;

        /// <summary>
        /// The width of the tile.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the tile.
        /// </summary>
        public int Height;

        /// <summary>
        /// The color of the tile, or the color to tint the texture.
        /// </summary>
        public Color Color;

        /// <summary>
        /// The alpha of the tile.
        /// </summary>
        public float Alpha = 1f;

        public TileInfo(int x, int y, int tx, int ty, int width, int height, Color color = null, float alpha = 1) {
            X = x;
            Y = y;
            TX = tx;
            TY = ty;
            Width = width;
            Height = height;
            if (color == null) {
                Color = Color.White;
            }
            else {
                Color = color;
            }
            Alpha = alpha;
        }

        internal Vector2f SFMLPosition {
            get { return new Vector2f(X, Y); }
        }

        internal Vector2f SFMLTextureCoord {
            get { return new Vector2f(TX, TY); }
        }

        internal Vertex CreateVertex(int x = 0, int y = 0, int tx = 0, int ty = 0) {
            var tileColor = new Color(Color);
            tileColor.A = Color.A * Alpha;
            if (TX == -1 || TY == -1) {
                return new Vertex(new Vector2f(X + x, Y + y), tileColor.SFMLColor);
            }
            return new Vertex(new Vector2f(X + x, Y + y), tileColor.SFMLColor, new Vector2f(TX + tx, TY + ty));
        }

        internal void AppendVertices(VertexArray array) {
            array.Append(CreateVertex());
            array.Append(CreateVertex(Width, 0, Width));
            array.Append(CreateVertex(Width, Height, Width, Height));
            array.Append(CreateVertex(0, Height, 0, Height));
        }
    }
}
