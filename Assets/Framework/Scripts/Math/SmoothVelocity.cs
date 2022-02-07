using UnityEngine;

namespace Common
{

    /// A helper for assistance with smoothing the movement.
    /// 帮助使动作平滑的助手。
    public class SmoothVelocity
    {
        private float _current;
        private float _currentVelocity;

        /// Returns the smoothed velocity.
        public float Update(float target, float smoothTime)
        {
            return _current = Mathf.SmoothDamp(_current, target, ref _currentVelocity, smoothTime);
        }

        public float Current
        {
            get { return _current; }
            set { _current = value; }
        }
    }
}