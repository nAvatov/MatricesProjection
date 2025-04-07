using System.Collections.Generic;
using Jobs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;

public class TranslationOffsetFounder
{
    private readonly Button _applyOffsetButton;
    private readonly MatricesVisualizer _visualizer;
    
    private NativeArray<Matrix4x4> _allPossibleOffsets;
    private NativeArray<Matrix4x4> _resultingOffsets;
    
    private NativeArray<bool> _initializedResultsFlags;
    private List<Matrix4x4> _relevantOffsets;

    private int _currentOffsetVisualizedProjection;
    public TranslationOffsetFounder(Button applyOffsetButton, MatricesVisualizer visualizer)
    {
        _applyOffsetButton = applyOffsetButton;
        _visualizer = visualizer;
    }
    
    public void FindTranslations(NativeArray<Matrix4x4> targetProjectionMatrices, NativeArray<Matrix4x4> translatableMatrices)
    {
        int resultingOffsetsAmount = targetProjectionMatrices.Length * translatableMatrices.Length;
        _allPossibleOffsets = new NativeArray<Matrix4x4>(resultingOffsetsAmount, Allocator.Persistent);
        
        var possibleOffsetsFinderJob = new FindModelTranslationsJob()
        {  
            M = translatableMatrices,
            S = targetProjectionMatrices,
            AllResultingOffsets = _allPossibleOffsets
        };
        
        var jobHandle = possibleOffsetsFinderJob.Schedule(resultingOffsetsAmount, 4096);
        
        jobHandle.Complete();
        
        _resultingOffsets = new NativeArray<Matrix4x4>(targetProjectionMatrices.Length, Allocator.Persistent);
        _initializedResultsFlags = new NativeArray<bool>(targetProjectionMatrices.Length, Allocator.Persistent);

        for (int i = 0; i < _initializedResultsFlags.Length; i++)
        {
            _initializedResultsFlags[i] = false;
        }

        FindRelevantTranslationOffsets(translatableMatrices.Length, targetProjectionMatrices.Length);
    }

    private void FindRelevantTranslationOffsets(int rows, int columns)
    {
        var commonOffsetsFinderJob = new FindCommonTranslationsFromAll()
        {
            AllPossibleOffsets = _allPossibleOffsets,
            ResultOffsets = _resultingOffsets,
            InitializedResultsFlags = _initializedResultsFlags,
            Rows = rows,
            Columns = columns
        };
        
        var jobHandle = commonOffsetsFinderJob.Schedule(columns, 512);
        
        jobHandle.Complete();
        
        CutOffRelevantOffsets();
        
        _applyOffsetButton.onClick.AddListener(() =>
        {
            _visualizer.VisualizeNextProjection(_relevantOffsets[_currentOffsetVisualizedProjection], _currentOffsetVisualizedProjection);
            _currentOffsetVisualizedProjection = _currentOffsetVisualizedProjection == _relevantOffsets.Count - 1 ? 0 : _currentOffsetVisualizedProjection + 1;
        });
    }

    private void CutOffRelevantOffsets()
    {
        _relevantOffsets = new List<Matrix4x4>();
        
        for (int i = 0; i < _resultingOffsets.Length; i++)
        {
            if (_initializedResultsFlags[i] == true)
            {
                _relevantOffsets.Add(_resultingOffsets[i]);
            }
        }

        if (_relevantOffsets.Count > 0)
        {
            _applyOffsetButton.interactable = true;
            _visualizer.DisplayProjectionsAmount(_relevantOffsets.Count);
        }
        else
        {
            Debug.Log("No common offsets found");
        }
    }
    
    public void CleanUp()
    {
        _allPossibleOffsets.Dispose();
        _resultingOffsets.Dispose();
        _initializedResultsFlags.Dispose();
        
        _applyOffsetButton.onClick.RemoveAllListeners();
    }
}