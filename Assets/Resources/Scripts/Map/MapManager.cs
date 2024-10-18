using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour 
{
  public static MapManager instance;

    [Header("Map Settings:")]
      
    [SerializeField] private int width = 80;
    [SerializeField] private int height = 45;
    [SerializeField] private int roomMaxSize = 10;
    [SerializeField] private int roomMinSize = 6;
    [SerializeField] private int maxRooms = 30;
    [SerializeField] private int maxMonstersPerRoom = 2;
    [SerializeField] private int maxItemsPerRoom = 2;
    //[SerializeField] private int maxChestsPerRoom = 2;
    //[SerializeField] private int maxTorchesPerRoom = 2;

    [Header("Prefabs:")]
    //[SerializeField] private GameObject torchPrefab; // Add this line for the torch prefab


    [Header("Tiles:")]
    [SerializeField] private TileBase floorTile;
    //[SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase leftWallTile;
    [SerializeField] private TileBase rightWallTile;
    [SerializeField] private TileBase topWallTile;
    [SerializeField] private TileBase bottomWallTile;
    [SerializeField] private TileBase topLeftCornerTile;
    [SerializeField] private TileBase topRightCornerTile;
    [SerializeField] private TileBase bottomLeftCornerTile;
    [SerializeField] private TileBase bottomRightCornerTile;
    [SerializeField] private TileBase fogTile;

    [Header("Tilemaps:")]
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap obstacleMap;
    [SerializeField] private Tilemap fogMap;

    [Header("Features:")]
    [SerializeField] private List<Vector3Int> visibleTiles = new List<Vector3Int>();
    [SerializeField] private List<RectangularRoom> rooms = new List<RectangularRoom>();
    private Dictionary<Vector3Int, TileData> tiles = new Dictionary<Vector3Int, TileData>();
    private Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();

    public int Width { get => width; }
    public int Height { get => height; }
    public TileBase FloorTile { get => floorTile;}
    //public TileBase WallTile { get => wallTile;}

    public TileBase LeftWallTile { get => leftWallTile;}
    public TileBase RightWallTile { get => rightWallTile;}
    public TileBase TopWallTile { get => topWallTile;}
    public TileBase BottomWallTile { get => bottomWallTile;}
    public TileBase TopLeftCornerTile { get => topLeftCornerTile;}
    public TileBase TopRightCornerTile { get => topRightCornerTile;}
    public TileBase BottomLeftCornerTile { get => bottomLeftCornerTile;}
    public TileBase BottomRightCornerTile { get => bottomRightCornerTile;}
    public Tilemap FloorMap { get => floorMap; }
    public Tilemap ObstacleMap { get => obstacleMap; }
    public Tilemap FogMap { get => fogMap; }
    public List<RectangularRoom> Rooms { get => rooms; }
    public Dictionary<Vector2Int, Node> Nodes { get => nodes;  set => nodes = value; }
  private void Awake() 
  {
    if (instance == null) 
    {
      instance = this;
    } else {
      Destroy(gameObject);
    }
  }

  private void Start() 
  {
    ProcGen procGen = new ProcGen();
    procGen.GenerateDungeon(width, height, roomMaxSize, roomMinSize, maxRooms, maxMonstersPerRoom, maxItemsPerRoom, rooms);

    AddTileMapToDictionary(floorMap);
    AddTileMapToDictionary(obstacleMap);

    SetupFogMap();

    //InstantiateTorchesInRooms(); // Call the new method to place torches

    Camera.main.transform.position = new Vector3(40, 20.25f, -10);
    Camera.main.orthographicSize = 27;
  }

  // Method to instantiate torches in each room
    /*private void InstantiateTorchesInRooms()
    {
        foreach (RectangularRoom room in rooms)
        {
            // Randomly determine the number of torches to place in this room (between 1 and max per room)
            int torchCount = Random.Range(1, maxItemsPerRoom + 1);
            for (int i = 0; i < torchCount; i++)
            {
                Vector2 randomPosition = room.RandomPosition();
                CreateEntity("Torch", randomPosition); // Use the CreateEntity method
            }
        }
    }*/

  ///<summary>Return True if x and y are inside of the bounds of this map. </summary>
  public bool InBounds(int x, int y) => 0 <= x && x < width && 0 <= y && y < height;

  public void CreateEntity(string entity, Vector2 position) 
  {
    switch (entity) 
    {
      case "Player":
        Instantiate(Resources.Load<GameObject>("Player"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Player";
        break;
      case "Orc":
        Instantiate(Resources.Load<GameObject>("Orc"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Orc";
        break;
      case "Troll":
        Instantiate(Resources.Load<GameObject>("Troll"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Troll";
        break;
      case "Goblin":
        Instantiate(Resources.Load<GameObject>("Goblin"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Goblin";
        break;
      case "Skeleton":
        Instantiate(Resources.Load<GameObject>("Skeleton"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Skeleton";
        break;
      case "Zombie":
        Instantiate(Resources.Load<GameObject>("Zombie"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Zombie";
        break;
      case "Potion Of Health":
        Instantiate(Resources.Load<GameObject>("Potion Of Health"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Potion Of Health";
        break;
      case "Fireball Scroll":
        Instantiate(Resources.Load<GameObject>("Fireball Scroll"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Fireball Scroll";
        break;
      case "Confusion Scroll":
        Instantiate(Resources.Load<GameObject>("Confusion Scroll"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Confusion Scroll";
        break;
      case "Lightning Scroll":
        Instantiate(Resources.Load<GameObject>("Lightning Scroll"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Lightning Scroll";
        break;
      /*case "Torch":
        Instantiate(Resources.Load<GameObject>("Torch"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Torch";
        break;
      case "Chest":
        Instantiate(Resources.Load<GameObject>("Chest"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Chest";
        break;*/
      default:
      Debug.LogError("Entity not found");
      break;
    }
  }

  public void UpdateFogMap(List<Vector3Int> playerFOV) 
  {
    foreach (Vector3Int pos in visibleTiles) 
    {
      if (!tiles[pos].IsExplored) 
      {
        tiles[pos].IsExplored = true;
      }

      tiles[pos].IsVisible = false;
      fogMap.SetColor(pos, new Color(1.0f, 1.0f, 1.0f, 0.5f));
    }

    visibleTiles.Clear();

    foreach (Vector3Int pos in playerFOV) 
    {
      tiles[pos].IsVisible = true;
      fogMap.SetColor(pos, Color.clear);
      visibleTiles.Add(pos);
    }
  }

  public void SetEntitiesVisibilities() {
    foreach (Entity entity in GameManager.instance.Entities) 
    {
      if (entity.GetComponent<Player>()) 
      {
        continue;
      }

      Vector3Int entityPosition = floorMap.WorldToCell(entity.transform.position);

      if (visibleTiles.Contains(entityPosition)) 
      {
        entity.GetComponent<SpriteRenderer>().enabled = true;
      } 
      else 
      {
        entity.GetComponent<SpriteRenderer>().enabled = false;
      }
    }
  }

  private void AddTileMapToDictionary(Tilemap tilemap) 
  {
    foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin) 
    {
      if (!tilemap.HasTile(pos)) 
      {
        continue;
      }

      TileData tile = new TileData();
      tiles.Add(pos, tile);
    }
  }

  private void SetupFogMap() 
  {
    foreach (Vector3Int pos in tiles.Keys) 
    {
      fogMap.SetTile(pos, fogTile);
      fogMap.SetTileFlags(pos, TileFlags.None);
    }
  }

  public bool IsValidPosition(Vector3 futurePosition) 
  {
    Vector3Int gridPosition = floorMap.WorldToCell(futurePosition);
    if (!InBounds(gridPosition.x, gridPosition.y) || obstacleMap.HasTile(gridPosition)) 
    {
      return false;
    }
    return true;
  }
}