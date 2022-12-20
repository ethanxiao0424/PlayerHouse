using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 offset;
    [SerializeField] private bool hangOnWall=false;
    private void Update()
    {
        transform.position = BuildingSystem.current.SnapCoordinateToGrid(BuildingSystem.GetMouseWorldPosition());
    }
    private void Start()
    {
        
    }

    //private void OnMouseDown()
    //{
    //    offset = transform.position - BuildingSystem.GetMouseWorldPosition();
    //}

    //private void OnMouseDrag()
    //{
    //    Vector3 pos = BuildingSystem.GetMouseWorldPosition() + offset;
    //    transform.position = BuildingSystem.current.SnapCoordinateToGrid(pos);
    //}
}
