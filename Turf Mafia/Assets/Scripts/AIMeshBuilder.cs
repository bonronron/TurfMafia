using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class AIMeshBuilder : MonoBehaviour
{
    NavMeshSurface navMeshSurface;
    private void Start()
    {
        LightshipMapView lightShipMapView = GameObject.FindAnyObjectByType<LightshipMapView>();
        lightShipMapView.MapTileAdded += LightShipMapView_MapTileAdded;
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    private void LightShipMapView_MapTileAdded(IMapTile mapTile, IMapTileObject mapTileObject)
    {
        GameObject meshObj = mapTileObject.Transform.Find("AIRoad Mesh").gameObject;
        //Debug.Log(meshObj.activeInHierarchy);
        if (meshObj)
        {
            meshObj.AddComponent<NavMeshModifier>();
        }
    }

    public void RenderMesh()
    {
        StartCoroutine(RenderCoroutine());
    }
    IEnumerator RenderCoroutine()
    {
        navMeshSurface.BuildNavMesh();
        yield return null;
    }
}
