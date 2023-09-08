using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Profiling;

namespace Saivs.Animation
{
    public class AnimationGroup : MonoBehaviour
    {
        public static AnimationGroup Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject director = new GameObject("Animation Group");
                    director.hideFlags = HideFlags.HideAndDontSave;
                    _instance = director.AddComponent<AnimationGroup>();
                    DontDestroyOnLoad(director);
                }

                return _instance;
            }
        }

        private static AnimationGroup _instance;

        private HashSet<AnimationGraph> _animGraphs;

        private NativeArray<BoneTransform> _boneTransformsData;
        private NativeArray<GCHandle> _gcHandles;
        private TransformAccessArray _bonesAccessArray;

        private WriteTransformsJob _writeTransformsJob;
        private EvaluateAnimationGroupParallelJob _evaluateAnimationGroupParallelJob;
        private bool _isDirty;
        private int _totalBonesCount;

        private void Awake()
        {
            _animGraphs = new HashSet<AnimationGraph>(100);
        }

        private void LateUpdate()
        {
            if (_animGraphs.Count == 0)
                return;

            KeepDataValid();

            Evaluate();
        }

        private void Evaluate()
        {
            Profiler.BeginSample("AnimationGroup.Evaluate");

            JobHandle evaluateHandle = _evaluateAnimationGroupParallelJob.Schedule(_gcHandles.Length, 32);
            JobHandle writeTransformsHandle = _writeTransformsJob.Schedule(_bonesAccessArray, evaluateHandle);
            writeTransformsHandle.Complete();

            Profiler.EndSample();
        }

        private void KeepDataValid()
        {
            if (!_isDirty)
                return;

            Dispose();

            SetupEvaluateGroupJob();
            SetupTransformsWriteJob();

            _isDirty = false;
        }

        private void SetupEvaluateGroupJob()
        {
            _gcHandles = new NativeArray<GCHandle>(_animGraphs.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            int graphIndex = 0;

            foreach (AnimationGraph graph in _animGraphs)
            {
                _gcHandles[graphIndex] = GCHandle.Alloc(graph);
                graphIndex++;
            }

            _evaluateAnimationGroupParallelJob = new EvaluateAnimationGroupParallelJob()
            {
                GraphsCGHandles = _gcHandles
            };
        }

        private void SetupTransformsWriteJob()
        {
            _boneTransformsData = new NativeArray<BoneTransform>(_totalBonesCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            Transform[] bones = new Transform[_totalBonesCount];

            int startIndex = 0;

            foreach (AnimationGraph graph in _animGraphs)
            {
                Skeleton skeleton = graph.Rig.Skeleton;
                int bonesCount = skeleton.BoneCount;

                for (int i = 0; i < bonesCount; i++)
                {
                    bones[startIndex + i] = skeleton.BonesTransforms[i];
                    _boneTransformsData[startIndex + i] = skeleton.BonesNodes[i].DefaultTransform;
                }

                graph._transformsOutput = _boneTransformsData.Slice(startIndex, bonesCount);

                startIndex += bonesCount;
            }

            _bonesAccessArray = new TransformAccessArray(bones);

            _writeTransformsJob = new WriteTransformsJob()
            {
                BoneTransformsData = _boneTransformsData
            };
        }

        internal void AddAnimationGraph(AnimationGraph graph)
        {
            if (_animGraphs.Add(graph))
            {
                _totalBonesCount += graph.Rig.Skeleton.BoneCount;
                _isDirty = true;
            }
        }

        internal void RemoveAnimationGraph(AnimationGraph graph)
        {
            if (_animGraphs.Remove(graph))
            {
                _totalBonesCount -= graph.Rig.Skeleton.BoneCount;
                _isDirty = true;
            }
        }

        private void Dispose()
        {
            if (_gcHandles.IsCreated)
            {
                for (int i = 0; i < _gcHandles.Length; i++)
                {
                    _gcHandles[i].Free();
                }

                _gcHandles.Dispose();
            }

            if (_boneTransformsData.IsCreated)
                _boneTransformsData.Dispose();

            if (_bonesAccessArray.isCreated)
                _bonesAccessArray.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}