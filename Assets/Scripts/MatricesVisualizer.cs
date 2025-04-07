using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MatricesVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject _spaceObjectPrefab;
    [SerializeField] private GameObject _modelObjectPrefab;
    
    [SerializeField] private TextMeshProUGUI _projectionsAmountText;
    [SerializeField] private TextMeshProUGUI _currentProjectionIndexText;

    private List<GameObject> _modelObjects;
    private List<Matrix4x4> _cachedModelMatrices;

    public void DisplayProjectionsAmount(int amount)
    {
        _projectionsAmountText.SetText("Projections amount: " + amount);
    }
        
    public void VisualizeNextProjection(Matrix4x4 offset, int currentProjectionIndex)
    {
        ReturnModelsToInitialTransformations();
        
        foreach (var model in _modelObjects)
        {
            var objectsTransformation = Matrix4x4.TRS(model.transform.position, model.transform.rotation, Vector3.one);

            var newTransformation = objectsTransformation * offset;


            model.transform.position = newTransformation.GetColumn(3);
            model.transform.rotation = newTransformation.rotation;
        }
        
        _currentProjectionIndexText.SetText("Current projection index: " + currentProjectionIndex);
    }

    private void ReturnModelsToInitialTransformations()
    {
        for (int i = 0; i < _modelObjects.Count; i++)
        {
            _modelObjects[i].transform.position = _cachedModelMatrices[i].GetColumn(3);
            _modelObjects[i].transform.rotation = _cachedModelMatrices[i].rotation;
        }
    }

    public void SpawnObjects(NativeArray<Matrix4x4> modelArray, NativeArray<Matrix4x4> spaceArray)
    {
        _modelObjects = VisualizeObjectsInScene(modelArray, _modelObjectPrefab);
        VisualizeObjectsInScene(spaceArray, _spaceObjectPrefab);

        _cachedModelMatrices = modelArray.ToList();
    }

    private List<GameObject> VisualizeObjectsInScene(NativeArray<Matrix4x4> objectsTransformationData, GameObject targetObjectPrefab)
    {
        List<GameObject> spawnedObjectCache = new List<GameObject>();
        
        foreach (var objectTData in objectsTransformationData)
        {
            spawnedObjectCache.Add(SpawnObject(objectTData, targetObjectPrefab));
        }

        return spawnedObjectCache;
    }

    private GameObject SpawnObject(Matrix4x4 objectsTransformationMatrix, GameObject prefab)
    {
        var obj = Instantiate(prefab);
        
        obj.transform.position = objectsTransformationMatrix.GetColumn(3);
        obj.transform.rotation = objectsTransformationMatrix.rotation;

        return obj;
    }
}