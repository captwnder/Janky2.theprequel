using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Component that will slowly interpolate a value toward a target using a speed and acceleration.
    /// This component can move the value and does not know about time at all.
    /// </summary>
    public class Lerper : Component {

        public float Acceleration;
        public float MaxSpeed;

        public bool Completed {
            get;
            private set;
        }

        public float Target {
            get;
            private set;
        }

        public float Value {
            get;
            private set;
        }

        float initValue, targetSpeed, speed, distance;

        bool endPhase = false;
        
        public Lerper(float value, float accel, float maxSpeed) {
            initValue = value;
            Value = value;
            Acceleration = accel;
            MaxSpeed = maxSpeed;
            Completed = false;
        }

        public void SetTarget(float value) {
            if (Target == value) return;

            Target = value;

            initValue = Value;
            endPhase = false;
            Completed = false;
            distance = Math.Abs(initValue - Target);
        }

        public void SetValue(float value) {
            Value = value;
        }

        public override void Update() {
            base.Update();

            if (Completed) {
                Value = Target;
                return;
            }

            var currentDistance = Math.Abs(Target - Value);
            var stoppingDistance = (MaxSpeed * MaxSpeed) / (2 * Acceleration);
            var earlyStopDistance = (speed * speed) / (2 * Acceleration);

            if (!endPhase) {
                targetSpeed = MaxSpeed * Math.Sign(Target - Value);

                if (currentDistance <= stoppingDistance) {
                    if (speed == MaxSpeed) {
                        targetSpeed = 0;
                        endPhase = true;
                    }
                    if (speed < MaxSpeed) {
                        if (currentDistance <= earlyStopDistance) {
                            targetSpeed = 0;
                            endPhase = true;
                        }
                    }
                }
            }

            speed = Util.Approach(speed, targetSpeed, Acceleration);

            if (endPhase) {
                if (Target > Value) {
                    if (speed < Acceleration) speed = Acceleration;
                    Value = Util.Approach(Value, Target, speed);
                }
                if (Target < Value) {
                    if (speed > -Acceleration) speed = -Acceleration;
                    Value = Util.Approach(Value, Target, -speed);
                }
            }
            else {
                Value += speed;
            }

            if (currentDistance <= Acceleration * 5) {
                speed = 0;
                Completed = true;
            }
        }

        public static implicit operator float(Lerper lerper) {
            return lerper.Value;
        }
        public static implicit operator int(Lerper lerper) {
            return (int)lerper.Value;
        }
    }
}
