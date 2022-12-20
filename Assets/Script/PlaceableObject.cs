using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    public bool Placed { get; private set; }
    public Vector3Int Size { get; private set; }
    private Vector3[] Vertices;

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

    public Vector3 GetStartPosition()
    {
        return transform.TransformPoint(Vertices[0]);
    }

    private void Start()
    {
        GetColliderVertexPositionsLocal();
        CalculateSizeInCells();
    }

    public void Rotate()
    {
        transform.Rotate(new Vector3(0, 90, 0));
        Size = new Vector3Int(Size.y, Size.x, 1);

        Vector3[] vertices = new Vector3[Vertices.Length];

        for(int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = Vertices[(i + 1) % Vertices.Length];
        }

        Vertices = vertices;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="LayerMask">物件放置的地方</param>
    //public void Straight(int LayerMask=1<<6)
    //{
    //    if (LayerMask == 1 << 7)
    //    {
    //        transform.Rotate(new Vector3(-90, 0, 0));
    //        Size = new Vector3Int(Size.y, Size.x, 1);
    //        Vector3[] vertices = new Vector3[Vertices.Length];
    //        for (int i = 0; i < vertices.Length; i++)
    //        {
    //            vertices[i] = Vertices[(i + 1) % Vertices.Length];
    //        }
    //        Vertices = vertices;
    //    }
        
    //    else if (LayerMask == 1 << 8)
    //    {
    //        transform.Rotate(new Vector3(0, 0, 0));
    //        Size = new Vector3Int(Size.y, Size.x, 1);
    //        Vector3[] vertices = new Vector3[Vertices.Length];
    //        for (int i = 0; i < vertices.Length; i++)
    //        {
    //            vertices[i] = Vertices[(i + 1) % Vertices.Length];
    //        }
    //        Vertices = vertices;
    //    }
    //}

    public virtual void Place()
    {
        ObjectDrag drag = gameObject.GetComponent<ObjectDrag>();
        Destroy(drag);

        Placed = true;

        //invoke events of placement
    }
}
