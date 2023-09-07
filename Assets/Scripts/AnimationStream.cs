using System;
using System.Collections.Generic;

namespace Saivs.Animation
{
    public class AnimationStream : IDisposable
    {
        /// <summary>
        /// Used to check if a stream is valid, a stream is valid when at least one sequence has been executed.
        /// Preventing blend nodes from blending with Invalid streams.
        /// </summary>
        public bool IsStreamValid { get; private set; }
        public RigDefinition Rig { get; private set; }

        public List<BoneTransform> BoneTransforms;

        public AnimationStream()
        {
            BoneTransforms = new List<BoneTransform>(50);
        }

        public void Init(RigDefinition rig)
        {
            Rig = rig;

            BoneTransforms.Clear();

            int boneCount = Rig.Skeleton.BoneCount;
            if (BoneTransforms.Capacity < boneCount)
                BoneTransforms.Capacity = boneCount;

            for (int i = 0; i < boneCount; i++)
            {
                BoneTransforms.Add(BoneTransform.Null);
            }
        }

        public void MarkAsValid()
        {
            IsStreamValid = true;
        }

        public virtual void Dispose()
        {
            IsStreamValid = false;
            Rig = null;
        }
    }
}