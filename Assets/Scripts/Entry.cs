using System;
using System.IO;
using DefaultNamespace;
using Newtonsoft.Json;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Entry : MonoBehaviour
{
    [SerializeField] private MatricesVisualizer _matricesVisualizer;

    [SerializeField] private Button _applyOffsetButton;

    private NativeArray<Matrix4x4> _spaceObjectsTransformationData;
    private NativeArray<Matrix4x4> _modelObjectsTransformationData;
    
    private TranslationOffsetFounder _translationOffsetFounder;

    private void Start()
    {
        _spaceObjectsTransformationData = DeserializeTransformationMatricesData("/space.json");
        _modelObjectsTransformationData = DeserializeTransformationMatricesData("/model.json");
        
        _matricesVisualizer.SpawnObjects(_modelObjectsTransformationData, _spaceObjectsTransformationData);
        
        FindTranslations();
    }

    private void FindTranslations()
    {
        _translationOffsetFounder = new TranslationOffsetFounder(_applyOffsetButton, _matricesVisualizer);

        _translationOffsetFounder.FindTranslations(_spaceObjectsTransformationData,
            _modelObjectsTransformationData);
    }
    
    private NativeArray<Matrix4x4> DeserializeTransformationMatricesData(string jsonPath)
    {
        var matricesListObject = JsonConvert.DeserializeObject<MatricesDataJsonModel>(File.ReadAllText(Application.streamingAssetsPath + jsonPath));

        var arrayOfTransformationData = new NativeArray<Matrix4x4>(matricesListObject.Matrices.Count, Allocator.Persistent);

        int i = 0;
        
        foreach (var matrix in matricesListObject.Matrices)
        {
            arrayOfTransformationData[i] = ParseDeserializedMatrices(matrix);
            i++;
        }

        return arrayOfTransformationData;
    }

    private Matrix4x4 ParseDeserializedMatrices(MatrixJsonModel unparsedMatrix)
    {
        return new Matrix4x4
        {
            m00 = unparsedMatrix.m00,
            m01 = unparsedMatrix.m01,
            m02 = unparsedMatrix.m02,
            m03 = unparsedMatrix.m03,
            m10 = unparsedMatrix.m10,
            m11 = unparsedMatrix.m11,
            m12 = unparsedMatrix.m12,
            m13 = unparsedMatrix.m13,
            m20 = unparsedMatrix.m20,
            m21 = unparsedMatrix.m21,
            m22 = unparsedMatrix.m22,
            m23 = unparsedMatrix.m23,
            m30 = unparsedMatrix.m30,
            m31 = unparsedMatrix.m31,
            m32 = unparsedMatrix.m32,
            m33 = unparsedMatrix.m33
        };
    }


    private void OnDestroy()
    {
        _translationOffsetFounder.CleanUp();
        
        _spaceObjectsTransformationData.Dispose();
        _modelObjectsTransformationData.Dispose();
    }
}
