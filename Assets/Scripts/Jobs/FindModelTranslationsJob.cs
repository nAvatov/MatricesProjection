using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Jobs
{
    [BurstCompile]
    public struct FindModelTranslationsJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Matrix4x4> M;
        [ReadOnly] public NativeArray<Matrix4x4> S;
        [WriteOnly] public NativeArray<Matrix4x4> AllResultingOffsets;
        
        public void Execute(int index)
        {
            int currentModelMatrixIndex = index / S.Length;
            int targetSpaceMatrixIndex = index % S.Length;
            
            var modelMatrix = M[currentModelMatrixIndex];
            var targetMatrix = S[targetSpaceMatrixIndex];

            var resultingOffset = modelMatrix.inverse * targetMatrix;
            
            AllResultingOffsets[index] = resultingOffset;
        }
    }
}