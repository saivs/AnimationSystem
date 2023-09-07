using UnityEngine;

namespace Saivs.Animation
{
    public class RigDefinition
    {
        public Skeleton Skeleton;
    }

    public class Skeleton
    {
        public Transform[] BonesTransforms;
        public SkeletonNode[] BonesNodes;
        public int BoneCount => BonesTransforms.Length;
    }

    public struct SkeletonNode
    {
        public StringHash Id;
        public int ParentIndex;

        public BoneTransform DefaultTransform;
    }
}