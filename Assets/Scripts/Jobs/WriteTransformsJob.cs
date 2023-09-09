using Unity.Burst;
using Unity.Collections;
using UnityEngine.Jobs;

namespace Saivs.Animation
{
    /// <summary>
    /// The fastest way to write a large number of transforms
    /// </summary>
    [BurstCompile]
    public struct WriteTransformsJob : IJobParallelForTransform
    {
        [ReadOnly]
        public NativeArray<BoneTransform> BoneTransformsData;

        public void Execute(int index, TransformAccess transform)
        {
            BoneTransform boneTransform = BoneTransformsData[index];
            transform.SetLocalPositionAndRotation(boneTransform.LocalPosition, boneTransform.LocalRotation);
        }
    }
}