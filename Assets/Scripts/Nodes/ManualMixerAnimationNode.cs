using System.Collections.Generic;

namespace Saivs.Animation.Nodes
{
    public class ManualMixerAnimationNode : BaseAnimationNode
    {
        private List<BaseAnimationNode> _childStates = new List<BaseAnimationNode>();
        private List<float> _weights = new List<float>();

        public override void Evaluate(AnimationStream stream)
        {
            //Check if one of node has weight == 1 then we shouldn't blend anything
            for (int childIndex = _childStates.Count - 1; childIndex > 0; childIndex--)
            {
                BaseAnimationNode animationNode = _childStates[childIndex];
               
                if (animationNode != null)
                {
                    float weight = _weights[childIndex];

                    if (weight >= 0.999999f)
                    {
                        animationNode.Evaluate(stream);
                        return;
                    }
                }
            }

            //Ensure that we can blend with something
            bool isStreamFilled = false;

            for (int childIndex = 0; childIndex < _childStates.Count; childIndex++)
            {
                BaseAnimationNode animationNode = _childStates[childIndex];

                if (animationNode != null)
                {
                    float weight = _weights[childIndex];

                    if (weight > 0.000001f)
                    {
                        if (!isStreamFilled)
                        {
                            animationNode.Evaluate(stream);

                            if (stream.IsStreamValid)
                                isStreamFilled = true;
                        }
                        else
                        {
                            AnimationStream blendStream = AnimationStreamPool.Get(stream, RootGraph.ThreadIndex);
                            animationNode.Evaluate(blendStream);

                            BlendStreams(blendStream, stream, weight);

                            AnimationStreamPool.Release(blendStream, RootGraph.ThreadIndex);
                        }
                    }
                }
            }

        }

        //Virtual for case if we need blend only some bones or we need to blend in meshSpace
        //TODO: Move this into AnimationStreamUtility
        protected internal virtual void BlendStreams(AnimationStream blendStream, AnimationStream outputStream, float alpha)
        {
            if (outputStream.IsStreamValid)
            {
                for (int boneIndex = 0; boneIndex < outputStream.BoneTransforms.Count; boneIndex++)
                {
                    outputStream.BoneTransforms[boneIndex] = BoneTransform.Lerp(outputStream.BoneTransforms[boneIndex], blendStream.BoneTransforms[boneIndex], alpha);
                }
            }
        }

        #region Parenting
        public override int ChildCount => _childStates.Count;

        public void AddChild(BaseAnimationNode node)
        {
            node.SetParent(this, ChildCount);
        }

        public override BaseAnimationNode GetChild(int index)
        {
            return _childStates[index];
        }

        protected internal override void OnAddChild(BaseAnimationNode node)
        {
            _childStates.Add(node);
            _weights.Add(0f);
        }

        protected internal override void OnRemoveChild(BaseAnimationNode node)
        {
            _childStates.RemoveAt(node.Index);
            _weights.RemoveAt(node.Index);

            for (int index = 0; index < _childStates.Count; index++)
            {
                _childStates[index].Index = index;
            }
        }
        #endregion
    }
}