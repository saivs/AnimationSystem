using System;

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
        public BonesContainer BonesContainer { get; private set; }

        public AnimationStream()
        {
            BonesContainer = new BonesContainer();
        }

        public void Init(RigDefinition rig)
        {
            Rig = rig;

            int boneCount = Rig.Skeleton.BoneCount;

            BonesContainer.Init(boneCount);
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