using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem current;

    public GridLayout gridLayout;
    private Grid grid;

    [SerializeField] private Tilemap GroundTilemap;
    [SerializeField] private Tilemap LeftWallTilemap;
    [SerializeField] private Tilemap RightWallTilemap;
    [SerializeField] private TileBase whiteTile;

    public GameObject prefab1;
    public GameObject prefab2;

    [SerializeField] private PlaceableObject objectToPlace;

    #region Unity methods

    private void Awake()
    {
        current = this;
        grid = gridLayout.gameObject.GetComponent<Grid>();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (objectToPlace == null) InitializeWithObject(prefab1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (objectToPlace == null) InitializeWithObject(prefab2);
        }

        if (!objectToPlace)
        {
            return;
        }


        PlaceObject();

    }

    #endregion


    #region Utils

    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if(Physics.Raycast(ray,out RaycastHit raycastHit))
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 100, 1 << 6))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    /// <summary>
    /// 捕捉坐標到網格
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPos = gridLayout.WorldToCell(position);
        position = grid.GetCellCenterWorld(cellPos);
        return position;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="area"></param>
    /// <param name="tilemap"></param>
    /// <returns></returns>
    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }

        return array;
    }


    #endregion

    #region Building Placement 建築佈置
    /// <summary>
    /// 物件初始化
    /// </summary>
    /// <param name="prefab">物件</param>
    public void InitializeWithObject(GameObject prefab)
    {
        Vector3 position = SnapCoordinateToGrid(Vector3.zero);

        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        objectToPlace = obj.GetComponent<PlaceableObject>();
        obj.AddComponent<ObjectDrag>();
    }

    private bool CanBePlaced(PlaceableObject placeableObject)
    {
        BoundsInt area = new BoundsInt();
        area.position = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
        //area.size = new Vector3Int(area.size.x + 1, area.size.y + 1, area.size.z);
        area.size = new Vector3Int(placeableObject.Size.x + 1, placeableObject.Size.y + 1, placeableObject.Size.z);

        TileBase[] baseArray = GetTilesBlock(area, GroundTilemap);

        foreach (var b in baseArray)
        {
            if (b == whiteTile)
            {
                return false;
            }
        }
        return true;
    }

    public void TakeArea(Vector3Int start, Vector3Int size)
    {
        GroundTilemap.BoxFill(start, whiteTile, start.x, start.y, start.x + size.x, start.y + size.y);
    }

    #endregion


    #region command 命令
    
    public void PlaceObject()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (CanBePlaced(objectToPlace))
            {
                objectToPlace.Place();
                Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
                TakeArea(start, objectToPlace.Size);
                objectToPlace = null;
            }
            else
            {
                Destroy(objectToPlace.gameObject);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            objectToPlace.Rotate();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(objectToPlace.gameObject);
        }
    }

    #endregion
}
