using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Jobs
{
    [BurstCompile]
    public struct FindCommonTranslationsFromAll : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Matrix4x4> AllPossibleOffsets;
        public NativeArray<Matrix4x4> ResultOffsets;
        public NativeArray<bool> InitializedResultsFlags;

        public int Rows;
        public int Columns;
        
        public void Execute(int index)
        {
            int foundDuplicates = 0;
            
            if (!IsResultingArrayContainsMatrix(AllPossibleOffsets[index]))
            {
                var currentOffsetMatrix = AllPossibleOffsets[index];

                for (int i = 1; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        if (IsEqualMatrices(AllPossibleOffsets[i * Columns + j], currentOffsetMatrix))
                        {
                            foundDuplicates++;
                            break;
                        }
                    }

                    if (foundDuplicates < i)
                    {
                        break;
                    }

                    if (foundDuplicates == Rows - 1)
                    {
                        ResultOffsets[index] = currentOffsetMatrix;
                        InitializedResultsFlags[index] = true;
                    }
                }
            }
        }
        
        private bool IsResultingArrayContainsMatrix(Matrix4x4 matrix)
        {
            foreach (Matrix4x4 m in ResultOffsets)
            {
                if (IsEqualMatrices(m, matrix))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsEqualMatrices(Matrix4x4 a, Matrix4x4 b)
        {
            return a.GetRow(0).Equals(b.GetRow(0)) &&
                   a.GetRow(1).Equals(b.GetRow(1)) &&
                   a.GetRow(2).Equals(b.GetRow(2)) &&
                   a.GetRow(3).Equals(b.GetRow(3));
        }
    }
}