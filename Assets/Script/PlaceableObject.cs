using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    public bool Placed { get; private set; }
    public Vector3Int Size { get; private set; }
    private Vector3[] Vertices;
    private Vector3 offset;
    private bool drag;
    private BuildingSystem bs;
    private int routeTimes = 0;


    /// <summary>
    /// 取BoxCollider頂點位置
    /// </summary>
    private void GetColliderVertexPositionsLocal()
    {
        BoxCollider b = gameObject.GetComponent<BoxCollider>();
        Vertices = new Vector3[4];
        Vertices[0] = b.center + new Vector3(-b.size.x, -b.size.y, -b.size.z) * 0.5f;
        Vertices[1] = b.center + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f;
        Vertices[2] = b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f;
        Vertices[3] = b.center + new Vector3(-b.size.x, -b.size.y, b.size.z) * 0.5f;
    }

    /// <summary>
    /// 計算大小
    /// </summary>
    private void CalculateSizeInCells()
    {
        Vector3Int[] vertices = new Vector3Int[Vertices.Length];

        for(int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(Vertices[i]);
            vertices[i] = BuildingSystem.current.Cur_gridLayout.WorldToCell(worldPos);
        }

        Size = new Vector3Int(Mathf.Abs((vertices[0] - vertices[1]).x),
                                Mathf.Abs((vertices[0] - vertices[3]).y),
                                1);
    }
    /// <summary>
    /// 取頂點0位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetStartPosition()
    {
        return transform.TransformPoint(Vertices[0]);
    }
    private void Awake()
    {
        bs = GameObject.FindGameObjectWithTag("BuildManager").GetComponent<BuildingSystem>();
        GetColliderVertexPositionsLocal();
    }
    private void Start()
    {
        CalculateSizeInCells();
    }

    private void OnMouseDown()
    {
        if(drag) offset = transform.position - bs.GetMouseWorldPosition();
    }

    private void OnMouseDrag()
    {
        if (drag)
        {
            Vector3 pos = bs.GetMouseWorldPosition() + offset;
            transform.position = BuildingSystem.current.SnapCoordinateToGrid(pos);
        }
    }

    /// <summary>
    /// 旋轉
    /// </summary>
    public void Rotate()
    {
        routeTimes++;
        transform.Rotate(new Vector3(0, 90, 0));
        Size = new Vector3Int(Size.y, Size.x, Size.z);
        Vector3[] vertices = new Vector3[Vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = Vertices[(i + 1) % Vertices.Length];
        }
        Vertices = vertices;
        Debug.Log(Size);
    }
    /// <summary>
    /// 轉到左邊牆
    /// </summary>
    public void RotateL()
    {
        transform.Rotate(new Vector3(270, 0, 0));
        //Size = new Vector3Int(Size.x, Size.z, Size.y);
        Debug.Log(Size);
    }
    /// <summary>
    /// 轉到右邊牆
    /// </summary>
    public void RotateR()
    {
        transform.Rotate(new Vector3(0, 0, 90));
        //Size = new Vector3Int(Size.x, Size.z, Size.y);
        Debug.Log(Size);
    }

    public void ResetRotate()
    {
        transform.rotation= Quaternion.identity;
        Debug.Log(Size);
    }

    public void Drag(bool _CanBeDragged)
    {
        drag = _CanBeDragged;
    }

    public virtual void Place()
    {
        Placed = true;
        Drag(false);
        routeTimes = 0;
        //invoke events of placement
    }
}
