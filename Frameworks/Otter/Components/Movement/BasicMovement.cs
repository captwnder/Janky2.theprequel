using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Movement Component that can be used by an Entity to provide simple top-down style movement.
    /// This class requires an Axis component be assigned to it, and that Axis must be also updated
    /// by another source.  A simple way to do this is to just have the Entity have an Axis component
    /// as well as the movement component, and then pass a reference to the axis into the movement.
    /// 
    /// The movement also requires a Collider if you want it to be able to collide with walls and other
    /// things!
    /// </summary>
    public class BasicMovement : Movement {
        public Speed Speed;
        public Speed TargetSpeed;
        public float Accel;

        /// <summary>
        /// Determines if diagonal movement should be limited.  This is to fix the problem where a vector of
        /// (1, 1) would have more length than a vector of (1, 0) or (0, 1).
        /// </summary>
        public bool CircleClamp = true;

        /// <summary>
        /// The axis used to move.  This must be set to something and updated for the movement to work.
        /// </summary>
        public Axis Axis;

        /// <summary>
        /// Creates the basic movement.
        /// </summary>
        /// <param name="xMax">The maximum speed allowed in the x axis.</param>
        /// <param name="yMax">The maximum speed allowed in the y axis.</param>
        /// <param name="accel">The acceleration.</param>
        public BasicMovement(float xMax, float yMax, float accel) : base() {
            Speed = new Speed(xMax, yMax);
            TargetSpeed = new Speed(xMax, yMax);
            Accel = accel;
        }

        public override void Update() {
            base.Update();

            TargetSpeed.XMax = Speed.XMax;
            TargetSpeed.YMax = Speed.YMax;

            if (Axis != null) {
                TargetSpeed.X = Axis.X * TargetSpeed.XMax;
                TargetSpeed.Y = Axis.Y * TargetSpeed.YMax;

                // Multiply by 1/sqrt(2) for circle clamp.
                if (CircleClamp && Math.Abs(TargetSpeed.X) == 1 && Math.Abs(TargetSpeed.Y) == 1) {
                    TargetSpeed.X *= .7071f;
                    TargetSpeed.Y *= .7071f;
                }
            }

            Speed.X = Util.Approach(Speed.X, TargetSpeed.X, Accel);
            Speed.Y = Util.Approach(Speed.Y, TargetSpeed.Y, Accel);

            MoveXY((int)Speed.X, (int)Speed.Y, Collider);

            if (OnMove != null) {
                OnMove();
            }
        }

        public override void MoveCollideX(Collider collider) {
            Speed.X = 0;
        }

        public override void MoveCollideY(Collider collider) {
            Speed.Y = 0;
        }
    }
}
