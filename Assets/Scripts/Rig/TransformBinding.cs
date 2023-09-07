using System;
using UnityEngine;

namespace Saivs.Animation
{
    [Serializable]
    public class TransformBinding
    {
        public Transform BoneTransform => _boneTransform;
        public string BindingName => _bindingName;

        [SerializeField]
        private Transform _boneTransform;
        [SerializeField]
        private string _bindingName;

        public TransformBinding(Transform boneTransform, string bindingName)
        {
            _boneTransform = boneTransform;
            _bindingName = bindingName;
        }
    }
}