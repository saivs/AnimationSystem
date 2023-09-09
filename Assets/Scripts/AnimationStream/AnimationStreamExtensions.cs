using Saivs.Extensions;
using Unity.Mathematics;

namespace Saivs.Animation
{
    public static class AnimationStreamExtensions
    {
        #region LocalSpace
        public static void GetLocalPosition(this AnimationStream animationStream, int boneIndex, out float3 position)
        {
            GetLocalTRS(animationStream, boneIndex, out position, out _, out _);
        }

        public static void GetLocalRotation(this AnimationStream animationStream, int boneIndex, out quaternion rotation)
        {
            GetLocalTRS(animationStream, boneIndex, out _, out rotation, out _);
        }

        public static void SetLocalPosition(this AnimationStream animationStream, int boneIndex, float3 position)
        {
            GetLocalTRS(animationStream, boneIndex, out _, out quaternion rotation, out float3 scale);
            SetLocalTRS(animationStream, boneIndex, position, rotation, scale);
        }

        public static void SetLocalRotation(this AnimationStream animationStream, int boneIndex, quaternion rotation)
        {
            GetLocalTRS(animationStream, boneIndex, out float3 position, out _, out float3 scale);
            SetLocalTRS(animationStream, boneIndex, position, rotation, scale);
        }

        public static void GetLocalTRS(this AnimationStream animationStream, int boneIndex, out float3 position, out quaternion rotation, out float3 scale)
        {
            ref BoneTransform boneTransform = ref animationStream.BonesContainer[boneIndex];

            position = boneTransform.LocalPosition;
            rotation = boneTransform.LocalRotation;
            scale = boneTransform.LocalScale;
        }

        public static void SetLocalTRS(this AnimationStream animationStream, int boneIndex, float3 position, quaternion rotation, float3 scale)
        {
            ref BoneTransform boneTransform = ref animationStream.BonesContainer[boneIndex];

            boneTransform.LocalPosition = position;
            boneTransform.LocalRotation = rotation;
            boneTransform.LocalScale = scale;

            SetMeshSpaceDirtyForBone(animationStream, boneIndex, setForChildrensOnly: false);
        }
        #endregion

        #region MeshSpace
        public static void GetMeshSpacePosition(this AnimationStream animationStream, int boneIndex, out float3 position)
        {
            GetMeshSpaceTRS(animationStream, boneIndex, out position, out _, out _);
        }

        public static void GetMeshSpaceRotation(this AnimationStream animationStream, int boneIndex, out quaternion rotation)
        {
            GetMeshSpaceTRS(animationStream, boneIndex, out _, out rotation, out _);
        }

        public static void GetMeshSpacePosition(this AnimationStream animationStream, int boneIndex, float3 position)
        {
            GetMeshSpaceTRS(animationStream, boneIndex, out _, out quaternion rotation, out float3 scale);
            SetMeshSpaceTRS(animationStream, boneIndex, position, rotation, scale);
        }

        public static void GetMeshSpaceRotation(this AnimationStream animationStream, int boneIndex, quaternion rotation)
        {
            GetMeshSpaceTRS(animationStream, boneIndex, out float3 position, out _, out float3 scale);
            SetMeshSpaceTRS(animationStream, boneIndex, position, rotation, scale);
        }

        public static void GetMeshSpaceTRS(this AnimationStream animationStream, int boneIndex, out float3 position, out quaternion rotation, out float3 scale)
        {
            ResolveMeshSpaceMatrix(animationStream, boneIndex);
            ref BoneTransform boneTransform = ref animationStream.BonesContainer[boneIndex];

            position = boneTransform.MeshSpacePosition;
            rotation = boneTransform.MeshSpaceRotation;
            scale = boneTransform.MeshSpaceScale;
        }

        public static void SetMeshSpaceTRS(this AnimationStream animationStream, int boneIndex, float3 position, quaternion rotation, float3 scale)
        {
            ref BoneTransform boneTransform = ref animationStream.BonesContainer[boneIndex];
            ref SkeletonNode skeletonNode = ref animationStream.Rig.Skeleton.BonesNodes[boneIndex];

            ResolveMeshSpaceMatrix(animationStream, skeletonNode.ParentIndex);

            boneTransform.MeshSpacePosition = position;
            boneTransform.MeshSpaceRotation = rotation;
            boneTransform.MeshSpaceScale = scale;

            if (skeletonNode.ParentIndex == -1)
            {
                boneTransform.LocalPosition = position;
                boneTransform.LocalRotation = rotation;
                boneTransform.LocalScale = scale;
            }
            else
            {
                ref BoneTransform parentBoneTransform = ref animationStream.BonesContainer[skeletonNode.ParentIndex];

                float4x4 invertedMSParentMatrix = math.inverse(parentBoneTransform.MeshSpaceMatrix);
                float4x4 loacalSpaceMatrix = invertedMSParentMatrix * boneTransform.MeshSpaceMatrix;

                boneTransform.LocalPosition = loacalSpaceMatrix.GetPosition();
                boneTransform.LocalRotation = loacalSpaceMatrix.GetRotation();
                boneTransform.LocalScale = loacalSpaceMatrix.GetScale();
            }

            SetMeshSpaceDirtyForBone(animationStream, boneIndex, setForChildrensOnly: true);
        }

        /// <summary>
        /// Resolves meshSpaceMatrixes recoursive if dirty
        /// </summary>
        /// <param name="animationStream"></param>
        /// <param name="boneIndex"></param>
        private static void ResolveMeshSpaceMatrix(AnimationStream animationStream, int boneIndex)
        {
            ref BoneTransform boneTransform = ref animationStream.BonesContainer[boneIndex];

            if (boneTransform.IsMeshSpaceTransformDirty)
            {
                ref SkeletonNode skeletonNode = ref animationStream.Rig.Skeleton.BonesNodes[boneIndex];

                //ensure that parent matrix resolved first
                if (skeletonNode.ParentIndex != -1)
                    ResolveMeshSpaceMatrix(animationStream, skeletonNode.ParentIndex);

                ref BoneTransform parentTransform = ref animationStream.BonesContainer[skeletonNode.ParentIndex];
                float4x4 meshSpaceMatrix = parentTransform.MeshSpaceMatrix * boneTransform.LocalSpaceMatrix;

                boneTransform.MeshSpacePosition = meshSpaceMatrix.GetPosition();
                boneTransform.MeshSpaceRotation = meshSpaceMatrix.GetRotation();
                boneTransform.MeshSpaceScale = meshSpaceMatrix.GetScale();
                boneTransform.IsMeshSpaceTransformDirty = false;
            }
        }
        #endregion

        /// <summary>
        /// Recoursive set IsMeshSpaceTransformDirty for bone and its childrens
        /// </summary>
        /// <param name="animationStream"></param>
        /// <param name="boneIndex"></param>
        private static void SetMeshSpaceDirtyForBone(AnimationStream animationStream, int boneIndex, bool setForChildrensOnly)
        {
            ref SkeletonNode skeletonNode = ref animationStream.Rig.Skeleton.BonesNodes[boneIndex];
            ref BoneTransform boneTransform = ref animationStream.BonesContainer[boneIndex];

            //that means that all childrens already is dirty
            if (boneTransform.IsMeshSpaceTransformDirty)
                return;

            if(!setForChildrensOnly)
                boneTransform.IsMeshSpaceTransformDirty = true;

            if (skeletonNode.ChildIndexes.IsCreated)
            {
                for (int i = 0; i < skeletonNode.ChildIndexes.Length; i++)
                {
                    SetMeshSpaceDirtyForBone(animationStream, skeletonNode.ChildIndexes[i], setForChildrensOnly: false);
                }
            }
        }
    }
}