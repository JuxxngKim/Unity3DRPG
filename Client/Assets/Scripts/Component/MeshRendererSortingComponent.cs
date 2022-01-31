using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class MeshRendererSortingComponent : MonoBehaviour
{
    [SerializeField] int _sortingOrder;
    [SerializeField] SkinnedMeshRenderer _renderer;

    public void OnEnable()
    {
        _renderer.sortingOrder = _sortingOrder;
    }
}
