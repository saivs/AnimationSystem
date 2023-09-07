using Saivs.Animation.Nodes;
using Unity.Collections;
using UnityEngine;

namespace Saivs.Animation
{
    [RequireComponent(typeof(GenericBindings))]
    public class AnimationGraph : MonoBehaviour
    {
        public RigDefinition Rig
        {
            get
            {
                if (_genericBindings != null)
                {
                    return _genericBindings.RigDefinition;
                }

                return null;
            }
        }

        /// <summary>
        /// Thread index is used for threadsafe pooling
        /// </summary>
        public int ThreadIndex { get; internal set; }

        internal NativeSlice<BoneTransform> _transformsOutput;
        private AnimationStream _outputAnimationStream;
        private OutputAnimationNode _outputAnimationNode;
        private GenericBindings _genericBindings;

        private void Awake()
        {
            _genericBindings = GetComponent<GenericBindings>();
            _outputAnimationStream = new AnimationStream();
            _outputAnimationNode = new OutputAnimationNode();
        }

        private void OnEnable()
        {
            AnimationGroup.Instance.AddAnimationGraph(this);
        }

        private void OnDisable()
        {
            AnimationGroup.Instance.RemoveAnimationGraph(this);
        }

        public void Evaluate()
        {
            _outputAnimationNode.Evaluate(_outputAnimationStream);

            for (int i = 0; i < _outputAnimationStream.BoneTransforms.Count; i++)
            {
                _transformsOutput[i] = _outputAnimationStream.BoneTransforms[i];
            }
        }
    }
}