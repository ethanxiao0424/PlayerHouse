using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem current;

    public GridLayout Cur_gridLayout;
    [SerializeField] GridLayout groundGridLayout;
    [SerializeField] GridLayout leftWallGridLayout;
    [SerializeField] GridLayout rightWallGridLayout;
    private Grid grid;
    private Grid groundGrid;
    private Grid leftWallGrid;
    private Grid rightWallGrid;

    [SerializeField] private Tilemap Cur_Tilemap;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap groundTilemap_Temp;
    [SerializeField] private Tilemap leftWallTilemap;
    [SerializeField] private Tilemap rightWallTilemap;

    [SerializeField] private TileBase whiteTile;
    [SerializeField] private TileBase GreenTile;

    [SerializeField] private GridTilemap gridTilemap;

    public GameObject prefab1;
    public GameObject prefab2;

    [SerializeField] public PlaceableObject objectToPlace;
    [SerializeField] private bool HangOnTheWall;
    [SerializeField] private GameObject cube;

    private static Dictionary<TileType, TileBase> tileBases = new Dictionary<TileType, TileBase>();
    public enum GridTilemap
    {
        Ground,
        LeftWall,
        RightWall
    }
    public enum TileType
    {
        Empty,
        White,
        Green,
        Red
    }

    #region Unity methods

    private void Awake()
    {
        current = this;
        grid = Cur_gridLayout.gameObject.GetComponent<Grid>();
        groundGrid = groundGridLayout.gameObject.GetComponent<Grid>();
        leftWallGrid = leftWallGridLayout.gameObject.GetComponent<Grid>();
        rightWallGrid = rightWallGridLayout.gameObject.GetComponent<Grid>();
    }
    private void Start()
    {
        string tilePath = @"Tiles\";
        tileBases.Add(TileType.Empty, null);
        tileBases.Add(TileType.White, Resources.Load<TileBase>(tilePath + "white"));
        tileBases.Add(TileType.Green, Resources.Load<TileBase>(tilePath + "green"));
        tileBases.Add(TileType.Red, Resources.Load<TileBase>(tilePath + "red"));
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
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchGridLayout(leftWallGrid, leftWallTilemap, leftWallTilemap);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchGridLayout(rightWallGrid, rightWallTilemap, rightWallTilemap);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchGridLayout(groundGrid, groundTilemap, groundTilemap);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            
        }

        if (!objectToPlace)
        {
            return;
        }


        PlaceObject();

    }

    #endregion


    #region Utils

    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out RaycastHit raycastHit, 100, 1<<6))
        if (Physics.Raycast(ray, out RaycastHit raycastHit,100,1<<6))
        {
            if(gridTilemap!= GridTilemap.Ground)
            {
                SwitchGridLayout(groundGrid, groundTilemap, groundTilemap);
                gridTilemap = GridTilemap.Ground;
                objectToPlace.ResetRotate();
            }    
            return raycastHit.point;
        }
        else if(Physics.Raycast(ray, out raycastHit, 100, 1 << 7))
        {
            if (gridTilemap != GridTilemap.LeftWall)
            {
                SwitchGridLayout(leftWallGrid, leftWallTilemap, leftWallTilemap);
                gridTilemap = GridTilemap.LeftWall;

                objectToPlace.ResetRotate();
                objectToPlace.RotateL();
            }
            return raycastHit.point;
        }
        else if(Physics.Raycast(ray, out raycastHit, 100, 1 << 8))
        {
            if (gridTilemap != GridTilemap.RightWall)
            {
                SwitchGridLayout(rightWallGrid, rightWallTilemap, rightWallTilemap);
                gridTilemap = GridTilemap.RightWall;
                objectToPlace.ResetRotate();
                objectToPlace.RotateR();
            }
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
        Vector3Int cellPos = Cur_gridLayout.WorldToCell(position);
        position = grid.GetCellCenterWorld(cellPos);
        return position;
    }

    /// <summary>
    /// 取得Tiles全部的格子數
    /// </summary>
    /// <param name="area"></param>
    /// <param name="tilemap"></param>
    /// <returns>陣列</returns>
    private TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
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
    private void FillTiles(TileBase[] arr,TileType type)
    {
        for(int i = 0; i < arr.Length; i++)
        {
            arr[i] = tileBases[type];
        }
    }

    private void SetTilesBlock(BoundsInt area,TileType type,Tilemap tilemap)
    {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArray = new TileBase[size];
        FillTiles(tileArray, type);
        tilemap.SetTilesBlock(area, tileArray);
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
        objectToPlace.Drag(true);

        switch (gridTilemap)
        {
            case GridTilemap.Ground:

                break;

            case GridTilemap.LeftWall:
                objectToPlace.RotateL();
                break;

            case GridTilemap.RightWall:
                objectToPlace.RotateR();
                break;

            default:
                break;

        }
    }

    private void FollowBuilding()
    {
        BoundsInt buildingArea = new BoundsInt();
        buildingArea.position = Cur_gridLayout.WorldToCell(objectToPlace.GetStartPosition());

        TileBase[] baseArray = GetTilesBlock(buildingArea, Cur_Tilemap);

        int size = baseArray.Length;
        TileBase[] tileArray = new TileBase[size];

        for(int i = 0; i < baseArray.Length; i++)
        {
            if (baseArray[i] == tileBases[TileType.White])
            {
                tileArray[i] = tileBases[TileType.Green];
            }
            else
            {
                FillTiles(tileArray, TileType.Red);
                break;
            }
        }
       // SetTilesBlock(buildingArea, tileArray);
        
    }
    /// <summary>
    /// 檢查擺放的物件能否放置
    /// </summary>
    /// <param name="placeableObject"></param>
    /// <returns></returns>
    private bool CanBePlaced(PlaceableObject placeableObject)
    {
        BoundsInt area = new BoundsInt();
        area.position = Cur_gridLayout.WorldToCell(objectToPlace.GetStartPosition());
        //area.size = new Vector3Int(area.size.x + 1, area.size.y + 1, area.size.z);
        area.size = new Vector3Int(placeableObject.Size.x, placeableObject.Size.y, placeableObject.Size.z);

        TileBase[] baseArray = GetTilesBlock(area, Cur_Tilemap);

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
        Cur_Tilemap.BoxFill(start, whiteTile, start.x, start.y, start.x + size.x, start.y + size.y);
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
                Vector3Int start = Cur_gridLayout.WorldToCell(objectToPlace.GetStartPosition());
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
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            objectToPlace.RotateL();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            objectToPlace.RotateR();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(objectToPlace.gameObject);
        }
    }

    private void SwitchGridLayout(Grid _Grid, GridLayout _gridLayout, Tilemap _tilemap)
    {
            grid = _Grid;
            Cur_gridLayout = _gridLayout;
            Cur_Tilemap = _tilemap;
    }

    #endregion
}
