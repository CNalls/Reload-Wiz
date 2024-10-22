using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private List<Vector3Int> visibleTiles;
    [SerializeField] private List<RectangularRoom> rooms;
    private Dictionary<Vector3Int, TileData> tiles;
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
    public List<Vector3Int> VisibleTiles { get => visibleTiles; }
    public Dictionary<Vector2Int, Node> Nodes { get => nodes;  set => nodes = value; }
   private void Awake() 
   {
    if (instance == null) 
    {
      instance = this;
    } 
    else 
    {
      Destroy(gameObject);
    }

    SceneManager.sceneLoaded += OnSceneLoaded;
  }

   /*private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
   {
    SceneState sceneState = SaveManager.instance.Save.Scenes.Find(x => x.FloorNumber == SaveManager.instance.CurrentFloor);

    if (sceneState is not null) 
    {
      LoadState(sceneState.MapState);
    } 
    else 
    {
      GenerateDungeon();
    }
  }*/

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
  {
      // Check and log if tilemaps are not initialized
      if (floorMap == null) Debug.LogError("FloorMap is not assigned!");
      if (obstacleMap == null) Debug.LogError("ObstacleMap is not assigned!");
      if (fogMap == null) Debug.LogError("FogMap is not assigned!");

      // If any of these are null, you should initialize them or ensure they are assigned.
      SceneState sceneState = SaveManager.instance.Save.Scenes.Find(x => x.FloorNumber == SaveManager.instance.CurrentFloor);

      if (sceneState != null) 
      {
          LoadState(sceneState.MapState);
      } 
      else 
      {
          // Handle when there is no saved scene state
          GenerateDungeon();
      }
  }



  private void Start() 
  {
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

    public void GenerateDungeon() 
  {
    rooms = new List<RectangularRoom>();
    tiles = new Dictionary<Vector3Int, TileData>();
    visibleTiles = new List<Vector3Int>();

    ProcGen procGen = new ProcGen();
    procGen.GenerateDungeon(width, height, roomMaxSize, roomMinSize, maxRooms, maxMonstersPerRoom, maxItemsPerRoom, rooms);

    AddTileMapToDictionary(floorMap);
    AddTileMapToDictionary(obstacleMap);
    SetupFogMap();
  }

  ///<summary>Return True if x and y are inside of the bounds of this map. </summary>
  public bool InBounds(int x, int y) => 0 <= x && x < width && 0 <= y && y < height;

  public GameObject CreateEntity(string entity, Vector2 position) 
  {
    GameObject entityObject = Instantiate(Resources.Load<GameObject>($"{entity}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
    entityObject.name = entity;
    return entityObject;
  }

  /*public void CreateEntity(string entity, Vector2 position) 
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
      case "Torch":
        Instantiate(Resources.Load<GameObject>("Torch"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Torch";
        break;
      case "Chest":
        Instantiate(Resources.Load<GameObject>("Chest"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Chest";
        break;
      default:
      Debug.LogError("Entity not found");
      break;
    }
  }*/

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

  public void SetEntitiesVisibilities() 
  {
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

  public bool IsValidPosition(Vector3 futurePosition) 
  {
    Vector3Int gridPosition = floorMap.WorldToCell(futurePosition);
    if (!InBounds(gridPosition.x, gridPosition.y) || obstacleMap.HasTile(gridPosition)) 
    {
      return false;
    }
    return true;
  }



  private void AddTileMapToDictionary(Tilemap tilemap) 
  {
    foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin) 
    {
      if (!tilemap.HasTile(pos)) 
      {
        continue;
      }

      TileData tile = new TileData
      (
        name: tilemap.GetTile(pos).name,
        isExplored: false,
        isVisible: false
      );

      tiles.Add(pos, tile);
    }
  }

  private void SetupFogMap() 
  {
    foreach (Vector3Int pos in tiles.Keys) 
    {
      if (!fogMap.HasTile(pos)) 
      {
        fogMap.SetTile(pos, fogTile);
        fogMap.SetTileFlags(pos, TileFlags.None);
      }

      if (tiles[pos].IsExplored) 
      {
        fogMap.SetColor(pos, new Color(1.0f, 1.0f, 1.0f, 0.5f));
      } 
      else 
      {
        fogMap.SetColor(pos, Color.white);
      }
    }
  }

  public MapState SaveState() => new MapState(tiles, rooms);

  
  /*public void LoadState(MapState mapState) 
  {
      rooms = mapState.StoredRooms;
      tiles = mapState.StoredTiles.ToDictionary(x => new Vector3Int((int)x.Key.x, (int)x.Key.y, (int)x.Key.z), x => x.Value);

      if (visibleTiles.Count > 0) 
      {
          visibleTiles.Clear();
      }

      // Create a dictionary to map tile names to actual TileBase objects
      Dictionary<string, TileBase> tileDictionary = new Dictionary<string, TileBase>
      {
          { floorTile.name, floorTile },
          { topWallTile.name, topWallTile },
          { bottomWallTile.name, bottomWallTile },
          { leftWallTile.name, leftWallTile },
          { rightWallTile.name, rightWallTile },
          { topLeftCornerTile.name, topLeftCornerTile },
          { topRightCornerTile.name, topRightCornerTile },
          { bottomLeftCornerTile.name, bottomLeftCornerTile },
          { bottomRightCornerTile.name, bottomRightCornerTile }
      };

        // Check if the tilemaps are null or destroyed before trying to set tiles
      if (floorMap == null || obstacleMap == null || fogMap == null) 
      {
          Debug.LogError("Tilemaps are missing or destroyed.");
          return;
      }

      // Loop through each stored tile and set the correct tile on the map
      foreach (Vector3Int pos in tiles.Keys) 
      {
          string tileName = tiles[pos].Name;
          if (tileDictionary.ContainsKey(tileName)) 
          {
              if (tileName == floorTile.name)
              {
                  floorMap.SetTile(pos, tileDictionary[tileName]);
              } 
              else 
              {
                  obstacleMap.SetTile(pos, tileDictionary[tileName]);
              }
          }
      }

      // Re-setup the fog of war for the map after loading
      SetupFogMap();
  }
}*/

  public void LoadState(MapState mapState) 
  {
    rooms = mapState.StoredRooms;
    tiles = mapState.StoredTiles.ToDictionary(x => new Vector3Int((int)x.Key.x, (int)x.Key.y, (int)x.Key.z), x => x.Value);
    
    if (visibleTiles.Count > 0) 
    {
        visibleTiles.Clear();
    }
    // Loop through each stored tile and set the correct tile on the map
    foreach (Vector3Int pos in tiles.Keys) 
    {
        // Check and set the floor tile
        if (tiles[pos].Name == floorTile.name) 
        {
            floorMap.SetTile(pos, floorTile);
        } 
        // Check and set the wall tiles
        else if (tiles[pos].Name == topWallTile.name) 
        {
            obstacleMap.SetTile(pos, topWallTile);
        }
        else if (tiles[pos].Name == bottomWallTile.name) 
        {
            obstacleMap.SetTile(pos, bottomWallTile);
        }
        else if (tiles[pos].Name == leftWallTile.name) 
        {
            obstacleMap.SetTile(pos, leftWallTile);
        }
        else if (tiles[pos].Name == rightWallTile.name) 
        {
            obstacleMap.SetTile(pos, rightWallTile);
        }
        // Check and set the corner tiles
        else if (tiles[pos].Name == topLeftCornerTile.name) 
        {
            obstacleMap.SetTile(pos, topLeftCornerTile);
        }
        else if (tiles[pos].Name == topRightCornerTile.name) 
        {
            obstacleMap.SetTile(pos, topRightCornerTile);
        }
        else if (tiles[pos].Name == bottomLeftCornerTile.name) 
        {
            obstacleMap.SetTile(pos, bottomLeftCornerTile);
        }
        else if (tiles[pos].Name == bottomRightCornerTile.name) 
        {
            obstacleMap.SetTile(pos, bottomRightCornerTile);
        }
    }

    // Re-setup the fog of war for the map after loading
    SetupFogMap();
  }

   /*public void LoadState(MapState mapState) 
   {
    rooms = mapState.StoredRooms;
    tiles = mapState.StoredTiles.ToDictionary(x => new Vector3Int((int)x.Key.x, (int)x.Key.y, (int)x.Key.z), x => x.Value);
    if (visibleTiles.Count > 0) 
    {
      visibleTiles.Clear();
    }

    foreach (Vector3Int pos in tiles.Keys) 
    {
      if (tiles[pos].Name == floorTile.name) 
      {
        floorMap.SetTile(pos, floorTile);
      } 
      else if (tiles[pos].Name == wallTile.name) 
      {
        obstacleMap.SetTile(pos, wallTile);
      }
    }
    SetupFogMap();
  }*/

}


[System.Serializable]
public class MapState 
{
  [SerializeField] private Dictionary<Vector3, TileData> storedTiles;
  [SerializeField] private List<RectangularRoom> storedRooms;
  public Dictionary<Vector3, TileData> StoredTiles { get => storedTiles; set => storedTiles = value; }
  public List<RectangularRoom> StoredRooms { get => storedRooms; set => storedRooms = value; }

  public MapState(Dictionary<Vector3Int, TileData> tiles, List<RectangularRoom> rooms) 
  {
    storedTiles = tiles.ToDictionary(x => (Vector3)x.Key, x => x.Value);
    storedRooms = rooms;
  }
}
