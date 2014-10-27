using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class AutoTimer : Component {

        public float Value;
        public float Max;
        public float Min;
        public float Increment = 1;

        public bool Paused { get; private set; }

        public AutoTimer(float max) {
            Max = max;
            
        }

        public AutoTimer(float value, float min, float max, float increment) {
            Value = value;
            Max = max;
            Min = min;
            Increment = increment;
        }

        public override void Update() {
            base.Update();

            if (!Paused) {
                Value += Increment;
            }
            Value = Util.Clamp(Value, Min, Max);
        }

        public bool AtMax {
            get { return Value == Max; }
        }

        public bool AtMin {
            get { return Value == Min; }
        }

        public void Reset() {
            Value = 0;
        }

        public void Pause() {
            Paused = true;
        }

        public void Resume() {
            Paused = false;
        }

        public void Start() {
            Reset();
            Paused = false;
        }

        public void Stop() {
            Paused = true;
            Reset();
        }

        public static implicit operator float(AutoTimer timer) {
            return timer.Value;
        }
        public static implicit operator int(AutoTimer timer) {
            return (int)timer.Value;
        }

    }
}
