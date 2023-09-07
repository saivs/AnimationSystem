using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Pool;

namespace Saivs.Animation
{
    public static class AnimationStreamPool
    {
        private static int _maxPoolSize = 10;
        private static ObjectPool<AnimationStream>[] _pools;

        static AnimationStreamPool()
        {
            _pools = new ObjectPool<AnimationStream>[JobsUtility.MaxJobThreadCount];

            for (int i = 0; i < _pools.Length; i++)
            {
                _pools[i] = new ObjectPool<AnimationStream>(CreatePooledItem, null, null, OnDestroyPoolObject, collectionCheck: false, 50, _maxPoolSize);
            }
        }

        public static AnimationStream Get(AnimationStream sampleStream, int threadIndex)
        {
            AnimationStream result = _pools[threadIndex].Get();
            result.Init(sampleStream.Rig);

            return result;
        }

        public static void Release(AnimationStream pose, int threadIndex)
        {
            _pools[threadIndex].Release(pose);
        }

        private static AnimationStream CreatePooledItem()
        {
            AnimationStream pose = new AnimationStream();
            return pose;
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        private static void OnDestroyPoolObject(AnimationStream pose)
        {
            pose.Dispose();
        }
    }
}