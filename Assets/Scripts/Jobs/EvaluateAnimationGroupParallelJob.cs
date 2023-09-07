using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;
using Unity.Jobs;

namespace Saivs.Animation
{
    /// <summary>
    /// Stable way to execute AnimationGraphs on a multithread. Execution via ThreadPool creates spikes.
    /// Doesn't support Burst.
    /// </summary>
    public struct EvaluateAnimationGroupParallelJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<GCHandle> GraphsCGHandles;

        [NativeSetThreadIndex]
        private int _threadIndex;

        public void Execute(int index)
        {
            AnimationGraph graph = (AnimationGraph)GraphsCGHandles[index].Target;
            graph.ThreadIndex = _threadIndex;
            graph.Evaluate();
        }
    }
}