namespace Saivs.Animation.Nodes
{
    public class ClipAnimationNode : BaseAnimationNode
    {
        public float Time;

        private AnimationClipData _animationClipData;
        private AnimationClipInstance _instance;

        public ClipAnimationNode(AnimationClipData clipData) : base()
        {
            _animationClipData = clipData;
        }
        public override void Init()
        {
            _instance = new AnimationClipInstance(_animationClipData, RootGraph.Rig);
        }

        public override void Evaluate(AnimationStream stream)
        {
            _instance.EvaluateSequenceAtTime(stream, Time);
            stream.MarkAsValid();
        }
    }
}