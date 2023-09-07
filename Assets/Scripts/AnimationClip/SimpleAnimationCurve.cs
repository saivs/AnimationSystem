using Unity.Mathematics;
using UnityEngine;

namespace Saivs.Animation
{
    public class SimpleAnimationCurve
    {
        private Keyframe[] _keyframes;
        private bool _useStaticValue;
        private float _staticValue;

        public SimpleAnimationCurve(AnimationCurve animationCurve)
        {
            if (animationCurve == null || animationCurve.length == 0)
            {
                _useStaticValue = true;
                _staticValue = 0;
                return;
            }

            Keyframe[] keys = animationCurve.keys;
            
            _useStaticValue = true;

            for (int i = 0; i < keys.Length; i++)
            {
                if (Mathf.Abs(_staticValue - keys[i].value) >= 0.0001f)
                {
                    _useStaticValue = false;
                }
            }

            if (_useStaticValue)
            {
                _staticValue = keys[0].value;
                return;
            }

            _keyframes = keys;
        }

        public float Evaluate(KeyfameData keyfameData)
        {
            if (_useStaticValue)
                return _staticValue;

            return math.lerp(
                _keyframes[keyfameData.LeftKeyframeIndex].value,
                _keyframes[keyfameData.RightKeyframeIndex].value,
                keyfameData.Weight);
        }
    }
}