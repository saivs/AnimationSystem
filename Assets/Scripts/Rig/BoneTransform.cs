using Unity.Mathematics;

namespace Saivs.Animation
{
    public struct BoneTransform
    {
        public float4x4 LocalSpaceMatrix => float4x4.TRS(LocalPosition, LocalRotation, LocalScale);
        public float4x4 MeshSpaceMatrix => float4x4.TRS(MeshSpacePosition, MeshSpaceRotation, MeshSpaceScale);

        public float3 LocalPosition;
        public quaternion LocalRotation;
        public float3 LocalScale;

        public float3 MeshSpacePosition;
        public quaternion MeshSpaceRotation;
        public float3 MeshSpaceScale;

        public bool IsMeshSpaceTransformDirty; 

        public static BoneTransform Null => new BoneTransform()
        {
            IsMeshSpaceTransformDirty = false,
            LocalPosition = float3.zero,
            LocalRotation = quaternion.identity,
            LocalScale = float3.zero,
            MeshSpacePosition = float3.zero,
            MeshSpaceRotation = quaternion.identity,
            MeshSpaceScale = float3.zero
        };
    }
}