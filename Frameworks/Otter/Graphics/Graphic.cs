using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace Otter {
    /// <summary>
    /// Base class used for anything that can be rendered.
    /// </summary>
    public class Graphic {

        /// <summary>
        /// The x position of the graphic.
        /// </summary>
        public float X = 0;

        /// <summary>
        /// The y position of the graphic.
        /// </summary>
        public float Y = 0;

        /// <summary>
        /// Determines if the graphic will render.
        /// </summary>
        public bool Visible = true;

        /// <summary>
        /// The scroll factor for the x position. Used for parallax like effects. Values lower than 1
        /// will scroll slower than the camera (appear to be further away) and values higher than 1
        /// will scroll faster than the camera (appear to be closer.)
        /// </summary>
        public float ScrollX = 1;

        /// <summary>
        /// The scroll factor for the y position. Used for parallax like effects. Values lower than 1
        /// will scroll slower than the camera (appear to be further away) and values higher than 1
        /// will scroll faster than the camera (appear to be closer.)
        /// </summary>
        public float ScrollY = 1;

        /// <summary>
        /// The width of the graphic.
        /// </summary>
        public int Width { get; protected set; }

        /// <summary>
        /// The height of the graphic.
        /// </summary>
        public int Height { get; protected set; }

        /// <summary>
        /// The shader to be applied to this graphic.
        /// </summary>
        public virtual Shader Shader {
            get { return shader; }
            set { shader = value; }
        }

        /// <summary>
        /// The render states to be passed to sfml for drawing.
        /// </summary>
        protected RenderStates RenderStates = RenderStates.Default;

        Shader shader;

        /// <summary>
        /// The name of the graphic.
        /// </summary>
        public string Name = "Graphic";

        /// <summary>
        /// The blend mode to be applied to this graphic.
        /// </summary>
        public BlendMode Blend = BlendMode.Alpha;

        public Graphic() {

        }

        /// <summary>
        /// Removes the shader from the graphic.
        /// </summary>
        public virtual void ClearShader() {
            Shader = null;
        }

        /// <summary>
        /// Set both ScrollX and ScrollY.
        /// </summary>
        public float Scroll {
            set { ScrollX = value; ScrollY = value; }
            get { return (ScrollX + ScrollY) / 2f; }
        }

        /// <summary>
        /// Update the graphic.
        /// </summary>
        public virtual void Update() {

        }

        /// <summary>
        /// Render the graphic.
        /// </summary>
        /// <param name="x">The x offset to render at.</param>
        /// <param name="y">The y offset to render at.</param>
        public virtual void Render(float x = 0, float y = 0) {

        }
    }

    /// <summary>
    /// The blendmodes that can be used for graphic rendering.
    /// </summary>
    public enum BlendMode {
        Alpha,
        Add,
        Multiply,
        None,
        Null
    }

    /// <summary>
    /// The repeat modes for rendering graphics that support Repeated rendering.
    /// </summary>
    public enum Repeat {
        None,
        X,
        Y,
        XY
    }

}
