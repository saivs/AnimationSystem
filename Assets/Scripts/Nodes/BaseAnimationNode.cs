namespace Saivs.Animation.Nodes 
{
    public abstract class BaseAnimationNode
    {
        public virtual void Init() { }
        public abstract void Evaluate(AnimationStream stream);

        #region Parenting

        public AnimationGraph RootGraph { get => _root; private set => _root = value; }
        public BaseAnimationNode Parent { get => _parent; }

        public int Index { get; set; } = int.MinValue;

        private BaseAnimationNode _parent;
        private AnimationGraph _root;

        public void SetParent(BaseAnimationNode parent, int index)
        {
            if (_parent != null)
            {
                _parent.OnRemoveChild(this);
                _parent = null;
            }

            if (parent == null)
            {
                Index = -1;
                return;
            }

            SetRoot(parent.RootGraph);
            Index = index;
            _parent = parent;
            parent.OnAddChild(this);
        }
        public void SetRoot(AnimationGraph root)
        {
            if (RootGraph == root)
                return;

            // Remove from the old root.
            if (RootGraph != null)
            {
                if (_parent != null && _parent.RootGraph != root)
                {
                    _parent.OnRemoveChild(this);
                    _parent = null;

                    Index = -1;
                }
            }

            // Set the root.
            RootGraph = root;

            Init();

            for (int i = ChildCount - 1; i >= 0; i--)
                GetChild(i)?.SetRoot(root);
        }

        #region Children
        public virtual int ChildCount => 0;
        public virtual BaseAnimationNode GetChild(int index)
            => null;

        protected internal virtual void OnAddChild(BaseAnimationNode node)
        {
            node.SetParent(null, -1);
        }

        protected internal virtual void OnRemoveChild(BaseAnimationNode node)
        {
            node.SetParent(null, -1);
        }
        #endregion

        #endregion
    }
}