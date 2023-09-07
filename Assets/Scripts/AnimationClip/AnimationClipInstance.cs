using UnityEngine;

namespace Saivs.Animation
{
    public class AnimationClipInstance
    {
        private AnimationClipData _animationClip;
        private RigDefinition _rig;

        internal int[] _boneBindingMap;
        internal int[] _curveBindingMap;

        public AnimationClipInstance(AnimationClipData animationClip, RigDefinition rig)
        {
            _animationClip = animationClip;
            _rig = rig;

            Skeleton skeleton = _rig.Skeleton;

            _boneBindingMap = new int[skeleton.BoneCount];

            for (int boneIndex = 0; boneIndex < skeleton.BoneCount; boneIndex++)
            {
                _boneBindingMap[boneIndex] = -1;

                ref SkeletonNode boneNode = ref skeleton.BonesNodes[boneIndex];

                for (int bindingIndex = 0; bindingIndex < animationClip._boneBindings.Length; bindingIndex++)
                {
                    ref BoneBinding boneBinding = ref animationClip._boneBindings[bindingIndex];

                    if (boneNode.Id == boneBinding.Id)
                    {
                        _boneBindingMap[boneIndex] = bindingIndex;
                        break;
                    }
                }
            }
        }

        public void EvaluateSequenceAtTime(AnimationStream animationStream, float time)
        {
            if (_animationClip.IsLooping)
                time = time % _animationClip.Duration;
            else
                time = Mathf.Clamp(time, 0, _animationClip.Duration);

            SimpleAnimationCurve[] curves = _animationClip.Curves;

            KeyfameData keyfameData = new KeyfameData(time, _animationClip.Duration, _animationClip.SampleRate);

            for (int boneIndex = 0; boneIndex < animationStream.BoneTransforms.Count; boneIndex++)
            {
                int boneMapIndex = _boneBindingMap[boneIndex];

                BoneTransform boneTransform = _rig.Skeleton.BonesNodes[boneIndex].DefaultTransform;

                if (boneMapIndex != -1)
                {
                    ref BoneBinding boneBinding = ref _animationClip._boneBindings[boneMapIndex];

                    if (boneBinding.PositionIndex != -1)
                    {
                        boneTransform.Position = new Vector3()
                        {
                            x = curves[boneBinding.PositionIndex].Evaluate(keyfameData),
                            y = curves[boneBinding.PositionIndex + 1].Evaluate(keyfameData),
                            z = curves[boneBinding.PositionIndex + 2].Evaluate(keyfameData),
                        };
                    }

                    if (boneBinding.RotationIndex != -1)
                    {
                        boneTransform.Rotation = new Quaternion()
                        {
                            x = curves[boneBinding.RotationIndex].Evaluate(keyfameData),
                            y = curves[boneBinding.RotationIndex + 1].Evaluate(keyfameData),
                            z = curves[boneBinding.RotationIndex + 2].Evaluate(keyfameData),
                            w = curves[boneBinding.RotationIndex + 3].Evaluate(keyfameData),
                        }.normalized;
                    }

                    if (boneBinding.ScaleIndex != -1)
                    {
                        boneTransform.Scale = new Vector3()
                        {
                            x = curves[boneBinding.ScaleIndex].Evaluate(keyfameData),
                            y = curves[boneBinding.ScaleIndex + 1].Evaluate(keyfameData),
                            z = curves[boneBinding.ScaleIndex + 2].Evaluate(keyfameData),
                        };
                    }
                }

                animationStream.BoneTransforms[boneIndex] = boneTransform;
            }
        }
    }
}