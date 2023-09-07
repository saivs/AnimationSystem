using UnityEngine;

namespace Saivs.Animation
{
    public class GenericBindings : MonoBehaviour
    {
        public RigDefinition RigDefinition
        {
            get
            {
                if (_rigDefinition == null)
                {
                    _rigDefinition = CreateRig();
                }

                return _rigDefinition;
            }
        }

        [SerializeField]
        private TransformBinding[] _bindingSet = null;

        private RigDefinition _rigDefinition;

        [ContextMenu("Setup Bindings")]
        private void SetupBindings()
        {
            Transform[] bones = GetComponentsInChildren<Transform>(includeInactive: true);

            _bindingSet = new TransformBinding[bones.Length - 1];

            for (int i = 1; i < bones.Length; i++)
            {
                Transform bone = bones[i];

                _bindingSet[i - 1] = new TransformBinding(bone, bone.name);
            }
        }

        private RigDefinition CreateRig()
        {
            Skeleton skeleton = new Skeleton();
            skeleton.BonesNodes = new SkeletonNode[_bindingSet.Length];
            skeleton.BonesTransforms = new Transform[_bindingSet.Length];

            for (int boneIndex = 0; boneIndex < _bindingSet.Length; boneIndex++)
            {
                TransformBinding boneBinding = _bindingSet[boneIndex];
                Transform boneTransform = boneBinding.BoneTransform;

                skeleton.BonesTransforms[boneIndex] = boneTransform;
                skeleton.BonesNodes[boneIndex] = new SkeletonNode()
                {
                    Id = new StringHash(boneBinding.BindingName),
                    ParentIndex = FindTransformIndex(boneTransform.parent),
                    DefaultTransform = new BoneTransform()
                    {
                        Position = boneTransform.localPosition,
                        Rotation = boneTransform.localRotation,
                        Scale = boneTransform.localScale
                    }
                };
            }

            RigDefinition result = new RigDefinition();
            result.Skeleton = skeleton;

            return result;
        }

        public int FindTransformIndex(Transform transform)
        {
            if (transform == null || _bindingSet == null)
                return -1;

            for (int i = 0; i < _bindingSet.Length; i++)
            {
                if (_bindingSet[i].BoneTransform == transform)
                    return i;
            }

            return -1;
        }
    }
}