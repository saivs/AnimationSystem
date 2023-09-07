namespace Saivs.Animation.Nodes
{
    public class OutputAnimationNode : BaseAnimationNode
    {
        private BaseAnimationNode _inputNode;     

        public override void Evaluate(AnimationStream stream)
        {
            if (_inputNode != null)
                _inputNode.Evaluate(stream);
        }

        #region Parenting
        public override int ChildCount => _inputNode != null ? 1 : 0;

        protected internal override void OnAddChild(BaseAnimationNode node)
        {
            if (_inputNode != null)
            {
                _inputNode.SetParent(null, -1);
            }

            _inputNode = node;
        }

        protected internal override void OnRemoveChild(BaseAnimationNode node)
        {
            base.OnRemoveChild(node);

            _inputNode = null;
        }

        public override BaseAnimationNode GetChild(int index)
        {
            return _inputNode;
        }

        #endregion
    }
}