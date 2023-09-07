using UnityEngine;

namespace Saivs.Animation
{
    public class AnimationClipData : ScriptableObject
    {
        public bool IsValid => _clip != null;

        public bool IsLooping => _clip.isLooping;
        public float Duration => _clip.length;
        public float SampleRate => _clip.frameRate;

        public SimpleAnimationCurve[] Curves
        {
            get
            {
                if (_simpleCurves == null)
                {
                    _simpleCurves = new SimpleAnimationCurve[_curves.Length];

                    for (int curveIndex = 0; curveIndex < _curves.Length; curveIndex++)
                    {
                        _simpleCurves[curveIndex] = new SimpleAnimationCurve(_curves[curveIndex]);
                    }
                }

                return _simpleCurves;
            }
        }

        [SerializeField]
        private AnimationClip _clip;
        [SerializeField, HideInInspector]
        private AnimationCurve[] _curves;
        [SerializeField, HideInInspector]
        internal BoneBinding[] _boneBindings;
        [SerializeField, HideInInspector]
        internal CurveBinding[] _curveBindings;

        private SimpleAnimationCurve[] _simpleCurves;
    }
}