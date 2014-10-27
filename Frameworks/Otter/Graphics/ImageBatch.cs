using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Otter {
    /// <summary>
    /// Graphic that can draw a bunch of simple images from a single texture.  Very early work in progress.
    /// You can add Images to the batch, but they all require using the same texture!
    /// </summary>
    public class ImageBatch : Image {

        List<Image> images = new List<Image>();
        List<VertexArray> vertexArrays = new List<VertexArray>();

        /// <summary>
        /// Determines if the batch should update each frame for images and sprites that are constantly
        /// animating or changing.  Dynamic batches are far more expensive than static!
        /// </summary>
        public bool Dynamic = false;

        /// <summary>
        /// How many updates to wait before updating the vertices.  Set this higher if there's a super
        /// huge dynamic vertex array that is updating very slowly.
        /// </summary>
        public int RefreshDelay = 0;

        int refreshCount = 0;

        int quadLimit = 2000;

        VertexArray vertices;

        public ImageBatch(string source = "", int width = 0, int height = 0) {
            if (source != "") {
                Texture = new Texture(source);
            }
            else {
                //No texture, just colored quads.
            }
            Width = width;
            Height = height;
            Init();
        }

        void Init() {

        }

        public void Add(Image img) {
            if (img.isCircle) throw new ArgumentException("ImageBatch cannot batch circles.  Quads only!");
            images.Add(img);

            NeedsUpdate = true;
        }

        public void UpdateBatch() {
            if (!NeedsUpdate && !Dynamic) return;
            if (refreshCount < RefreshDelay) {
                refreshCount++;
                if (refreshCount == RefreshDelay) {
                    refreshCount = 0;
                }
                else {
                    return;
                }
            }

            vertexArrays.Clear();
            vertexArrays.Add(new VertexArray(PrimitiveType.Quads));
            vertices = vertexArrays[0];
           
            var quadCount = 0;
            Image img;
            Color nextColor;
            for(var i = 0; i < images.Count; i++) {
                img = images[i];
                img.Update();
                nextColor = img.Color.Copy();
                
                nextColor.A *= alpha;
                nextColor.R *= color.R;
                nextColor.G *= color.G;
                nextColor.B *= color.B;
                 

                if (quadCount == quadLimit) {
                    quadCount = 0;
                    vertexArrays.Add(new VertexArray(PrimitiveType.Quads));
                    vertices = vertexArrays[vertexArrays.Count - 1];
                }
                
                img.Batched(vertices);

                quadCount++;
            }

            NeedsUpdate = false;
        }

        protected override void RenderImage(float x = 0, float y = 0) {
            if (vertexArrays.Count == 0) return;

            foreach (var v in vertexArrays) {
                DrawableSource = v;
                base.RenderImage(x, y);
            }
        }

        void Append(float x, float y, Color color, float tx, float ty) {
            Width = (int)Util.Max(x, Width);
            Height = (int)Util.Max(y, Height);
            vertices.Append(new Vertex(new Vector2f(x, y), Color.SFMLColor, new Vector2f(tx, ty)));
        }

        void Append(float x, float y, Color color) {
            Width = (int)Util.Max(x, Width);
            Height = (int)Util.Max(y, Height);
            vertices.Append(new Vertex(new Vector2f(x, y), color.SFMLColor));
        }

        public override void Update() {
            base.Update();

            UpdateBatch();
        }

    }


}
