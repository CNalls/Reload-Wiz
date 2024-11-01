using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Tilemaps;
using System.Linq;
using SysRandom = System.Random;
using UnityRandom = UnityEngine.Random;
using System;


//a lot of parts are wip having to do with generating the full 360 walls
sealed class ProcGen
{
    private List<Tuple<int, int>> maxItemsByFloor = new List<Tuple<int, int>> 
    {
    new Tuple<int, int>(1, 1),
    new Tuple<int, int>(4, 2),
    new Tuple<int, int>(7, 3),
    new Tuple<int, int>(10, 4),
    };
    private List<Tuple<int, int>> maxMonstersByFloor = new List<Tuple<int, int>> 
    {
    new Tuple<int, int>(1, 2),
    new Tuple<int, int>(4, 3),
    new Tuple<int, int>(6, 5),
    new Tuple<int, int>(8, 7),
    new Tuple<int, int>(10, 10),
    };

    private List<Tuple<int, string, int>> itemChances = new List<Tuple<int, string, int>> 
    {
    new Tuple<int, string, int>(0, "Potion of Health", 50),
    new Tuple<int, string, int>(1, "Confusion Scroll", 45),
    new Tuple<int, string, int>(3, "Lightning Scroll", 25),
    new Tuple<int, string, int>(3, "Fireball Scroll", 35),
    };

    private List<Tuple<int, string, int>> monsterChances = new List<Tuple<int, string, int>> 
    {
    new Tuple<int, string, int>(1, "Goblin", 60),
    new Tuple<int, string, int>(2, "Skeleton", 20),
    new Tuple<int, string, int>(2, "Zombie", 20),
    new Tuple<int, string, int>(3, "Orc", 40),
    new Tuple<int, string, int>(4, "Troll", 60),
    };

  public int GetMaxValueForFloor(List<Tuple<int, int>> values, int floor) 
  {
    int currentValue = 0;

    foreach (Tuple<int, int> value in values) 
    {
      if (floor >= value.Item1) 
      {
        currentValue = value.Item2;
      }
    }

    return currentValue;
  }
//better rand
  public List<string> GetEntitiesAtRandom(List<Tuple<int, string, int>> chances, int numberOfEntities, int floor) 
  {
    List<string> entities = new List<string>();
    List<int> weightedChances = new List<int>();

    foreach (Tuple<int, string, int> chance in chances) 
    {
      if (floor >= chance.Item1) 
      {
        entities.Add(chance.Item2);
        weightedChances.Add(chance.Item3);
      }
    }

    SysRandom rnd = new SysRandom();
    List<string> chosenEntities = rnd.Choices(entities, weightedChances, numberOfEntities);

    return chosenEntities;
  }

    /// <summary>
    /// Generate a new dungeon map
    /// </summary>
    public void GenerateDungeon(int mapWidth, int mapHeight, int roomMaxSize, int roomMinSize, int maxRooms, List<RectangularRoom> rooms, bool isNewGame)
    {
        //Generate the rooms
        for (int roomNum = 0; roomNum < maxRooms; roomNum++) //room num = 0 and if less that max rooms increment +
        {
            int roomWidth = UnityRandom.Range(roomMinSize, roomMaxSize);
            int roomHeight = UnityRandom.Range(roomMinSize, roomMaxSize);

            int roomX = UnityRandom.Range(0, mapWidth - roomWidth - 1);
            int roomY = UnityRandom.Range(0, mapHeight - roomHeight - 1);

            RectangularRoom newRoom = new RectangularRoom(roomX, roomY, roomWidth, roomHeight, MapManager.instance.Rooms.Count);

            //checks if this room intersects with any of the other rooms
            if (newRoom.Overlaps(rooms))
            {
                continue;
            }
            //if there are no intersections, the room is valid

            //digs out this rooms area and builds walls
            for (int x = roomX; x < roomX + roomWidth; x++)
            {
                for (int y = roomY; y < roomY + roomHeight; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x, y, 0);

                    // Check for corners first
                    if (x == roomX && y == roomY)
                    {
                        // Bottom-left corner
                        SetSpecificWallTile(tilePos, MapManager.instance.BottomLeftCornerTile);
                    }
                    else if (x == roomX && y == roomY + roomHeight - 1)
                    {
                        // Top-left corner
                        SetSpecificWallTile(tilePos, MapManager.instance.TopLeftCornerTile);
                    }
                    else if (x == roomX + roomWidth - 1 && y == roomY)
                    {
                        // Bottom-right corner
                        SetSpecificWallTile(tilePos, MapManager.instance.BottomRightCornerTile);
                    }
                    else if (x == roomX + roomWidth - 1 && y == roomY + roomHeight - 1)
                    {
                        // Top-right corner
                        SetSpecificWallTile(tilePos, MapManager.instance.TopRightCornerTile);
                    }
                    // Now check for walls
                    else if (x == roomX)
                    {
                        // Left wall
                        SetSpecificWallTile(tilePos, MapManager.instance.LeftWallTile);
                    }
                    else if (x == roomX + roomWidth - 1)
                    {
                        // Right wall
                        SetSpecificWallTile(tilePos, MapManager.instance.RightWallTile);
                    }
                    else if (y == roomY)
                    {
                        // Bottom wall
                        SetSpecificWallTile(tilePos, MapManager.instance.BottomWallTile);
                    }
                    else if (y == roomY + roomHeight - 1)
                    {
                        // Top wall
                        SetSpecificWallTile(tilePos, MapManager.instance.TopWallTile);
                    }
                    else
                    {
                        // Place floor tile
                        SetFloorTile(new Vector3Int(x, y));
                
                    /*if (x == roomX || x == roomX + roomWidth - 1 || y == roomY || y == roomY + roomHeight - 1)
                    {
                        if (SetWallTileIfEmpty(new Vector3Int(x, y, 0)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (MapManager.instance.ObstacleMap.GetTile(new Vector3Int(x, y, 0)))
                        {
                            MapManager.instance.ObstacleMap.SetTile(new Vector3Int(x, y, 0), null);
                        }
                        MapManager.instance.FloorMap.SetTile(new Vector3Int(x, y, 0), MapManager.instance.FloorTile);*/
                    }
                }
            }
            if (rooms.Count != 0)
            {
                //Dig out a tunnel between this room and previous one
                TunnelBetween(rooms[rooms.Count - 1], newRoom);
            }

            PlaceEntities(newRoom, SaveManager.instance.CurrentFloor);
            rooms.Add(newRoom);
        }

        //Add the stairs to the last room.
        MapManager.instance.FloorMap.SetTile((Vector3Int)rooms[rooms.Count - 1].RandomPoint(), MapManager.instance.DownStairsTile);

        //Add the player to the first room.
        Vector3Int playerPos = (Vector3Int)rooms[0].RandomPoint();

        while (GameManager.instance.GetActorAtLocation(playerPos) is not null) 
        {
        playerPos = (Vector3Int)rooms[0].RandomPoint();
        }

        MapManager.instance.FloorMap.SetTile(playerPos, MapManager.instance.UpStairsTile);

        if (!isNewGame) 
        {
        GameManager.instance.Actors[0].transform.position = new Vector3(playerPos.x + 0.5f, playerPos.y + 0.5f, 0);
        } 
        else 
        {
        MapManager.instance.CreateEntity("Player", (Vector2Int)playerPos);
        }
    }

    /// <summary>
    ///    Return an L-Shaped tunnel between the 2 points using Bresenham lines
    /// </summary>
    
    private void TunnelBetween(RectangularRoom oldRoom, RectangularRoom newRoom)
    {
        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();    
        Vector2Int tunnelCorner; 

        if (UnityRandom.value < 0.5f)
        {
            //Move Horizontally, then Vertically
            tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y);
        }
        else
        {
            //Move Vertically, the Horizontally
            tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y);
        }
        //Gen the coordinates for this tunnel
        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
        BresenhamLine.Compute(oldRoomCenter, tunnelCorner, tunnelCoords);
        BresenhamLine.Compute(tunnelCorner, newRoomCenter, tunnelCoords);

        //set tiles for the tunnel
        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            SetFloorTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y));

            // Set the Wall Tiles aroudn this tile to be walls
            for (int x = tunnelCoords[i].x - 1; x <= tunnelCoords[i].x + 1; x++)
            {
                for (int y = tunnelCoords[i].y - 1; y <= tunnelCoords[i].y + 1; y++)
                {
                    if (SetWallTileIfEmpty(new Vector3Int(x, y)))
                    {
                        continue;
                    }
                }
            }
        }

    }
        
    private bool SetWallTileIfEmpty(Vector3Int pos)
    {
        if (MapManager.instance.FloorMap.GetTile(pos))
    {
        return true;
    }
    else
    {
            // Check for neighboring floor tiles to determine which wall tile to place
            bool left = MapManager.instance.FloorMap.HasTile(new Vector3Int(pos.x - 1, pos.y, 0));
            bool right = MapManager.instance.FloorMap.HasTile(new Vector3Int(pos.x + 1, pos.y, 0));
            bool top = MapManager.instance.FloorMap.HasTile(new Vector3Int(pos.x, pos.y + 1, 0));
            bool bottom = MapManager.instance.FloorMap.HasTile(new Vector3Int(pos.x, pos.y - 1, 0));

            // Determine which wall or corner tile to place
            if (left && bottom)
            {
                MapManager.instance.ObstacleMap.SetTile(pos, MapManager.instance.BottomLeftCornerTile);
            }
            else if (right && bottom)
            {
                MapManager.instance.ObstacleMap.SetTile(pos, MapManager.instance.BottomRightCornerTile);
            }
            else if (left && top)
            {
                MapManager.instance.ObstacleMap.SetTile(pos, MapManager.instance.TopLeftCornerTile);
            }
            else if (right && top)
            {
                MapManager.instance.ObstacleMap.SetTile(pos, MapManager.instance.TopRightCornerTile);
            }
            else if (left)
            {
                MapManager.instance.ObstacleMap.SetTile(pos, MapManager.instance.LeftWallTile);
            }
            else if (right)
            {
                MapManager.instance.ObstacleMap.SetTile(pos, MapManager.instance.RightWallTile);
            }
            else if (top)
            {
                MapManager.instance.ObstacleMap.SetTile(pos, MapManager.instance.TopWallTile);
            }
            else if (bottom)
            {
                MapManager.instance.ObstacleMap.SetTile(pos, MapManager.instance.BottomWallTile);
            }
            else
            {
                // Default to a simple wall tile if no specific match is found
                MapManager.instance.ObstacleMap.SetTile(pos, MapManager.instance.BottomWallTile);
            }
            return false;
        }
    } 

    private void SetFloorTile(Vector3Int pos)
    {
        // Place floor tile
        if (MapManager.instance.ObstacleMap.GetTile(pos))
        {
            MapManager.instance.ObstacleMap.SetTile(pos, null);
        }
        MapManager.instance.FloorMap.SetTile(pos, MapManager.instance.FloorTile);
    }
    private void SetSpecificWallTile(Vector3Int pos, TileBase wallTile)
    {
        if (!MapManager.instance.FloorMap.GetTile(pos)) // Ensure no floor tile already exists
        {
            MapManager.instance.ObstacleMap.SetTile(pos, wallTile);
        }
    } 
    /*private void PlaceEntities(RectangularRoom newRoom, int floor) 
    {
        //int numberOfMonsters = UnityRandom.Range(0, maximumMonsters + 1);
        //int numberOfItems = UnityRandom.Range(0, maxi   umItems + 1);

        int numberOfMonsters = UnityRandom.Range(0, GetMaxValueForFloor(maxMonstersByFloor, floorNumber) + 1);
        int numberOfItems = UnityRandom.Range(0, GetMaxValueForFloor(maxItemsByFloor, floorNumber) + 1);

        for (int monster = 0; monster < numberOfMonsters;) 
        {
            int x = UnityRandom.Range(newRoom.X, newRoom.X + newRoom.Width);
            int y = UnityRandom.Range(newRoom.Y, newRoom.Y + newRoom.Height);

            if (x == newRoom.X || x == newRoom.X + newRoom.Width - 1 || y == newRoom.Y || y == newRoom.Y + newRoom.Height - 1) 
            {
                continue;
            }

        for (int entity = 0; entity < GameManager.instance.Entities.Count; entity++) 
        {
            Vector3Int pos = MapManager.instance.FloorMap.WorldToCell(GameManager.instance.Entities[entity].transform.position);

            if (pos.x == x && pos.y == y) 
            {
            return;
            }
        }

        float randomValue = Random.value;

        if (randomValue < 0.35f) //use float holder value to store the number | and upadate this randomness to increase the likely hood of a certain enemy spawning until it does
        {
            MapManager.instance.CreateEntity("Skeleton", new Vector2(x, y));
        } 
        else if(randomValue < 0.55f)
        {
            MapManager.instance.CreateEntity("Zombie", new Vector2(x, y));
        }
        else if(randomValue < 0.75f)
        {
            MapManager.instance.CreateEntity("Goblin", new Vector2(x, y));
        }
        else if(randomValue < 0.9f)
        {
            MapManager.instance.CreateEntity("Orc", new Vector2(x, y));
        }
        else
        {
            MapManager.instance.CreateEntity("Troll", new Vector2(x, y));
        }
        monster++;
        }

        for (int item = 0; item < numberOfItems;) 
        {
            int x = Random.Range(newRoom.X, newRoom.X + newRoom.Width);
            int y = Random.Range(newRoom.Y, newRoom.Y + newRoom.Height);

        if (x == newRoom.X || x == newRoom.X + newRoom.Width - 1 || y == newRoom.Y || y == newRoom.Y + newRoom.Height - 1) 
        {
            continue;
        }

        for (int entity = 0; entity < GameManager.instance.Entities.Count; entity++) 
        {
            Vector3Int pos = MapManager.instance.FloorMap.WorldToCell(GameManager.instance.Entities[entity].transform.position);

            if (pos.x == x && pos.y == y) 
            {
            return;
            }
        }

        float randomValue = Random.value;
        if (randomValue < 0.7f) 
        {
            MapManager.instance.CreateEntity("Potion of Health", new Vector2(x, y));
        } 
        else if (randomValue < 0.8f) 
        {
            MapManager.instance.CreateEntity("Fireball Scroll", new Vector2(x, y));
        } 
        else if (randomValue < 0.9f) 
        {
            MapManager.instance.CreateEntity("Confusion Scroll", new Vector2(x, y));
        } 
        else 
        {
            MapManager.instance.CreateEntity("Lightning Scroll", new Vector2(x, y));
        }

        item++;

        //if (Random.value < 0.8f) 
        //{
        //    MapManager.instance.CreateEntity("Potion Of Health", new Vector2(x, y));
        //} 
        //else 
        //{
        //    MapManager.instance.CreateEntity("Troll", new Vector2(x, y));
        }
    }*/

    private void PlaceEntities(RectangularRoom newRoom, int floorNumber) 
    {
    int numberOfMonsters = UnityRandom.Range(0, GetMaxValueForFloor(maxMonstersByFloor, floorNumber) + 1);
    int numberOfItems = UnityRandom.Range(0, GetMaxValueForFloor(maxItemsByFloor, floorNumber) + 1);

    List<string> monsterNames = GetEntitiesAtRandom(monsterChances, numberOfMonsters, floorNumber);
    List<string> itemNames = GetEntitiesAtRandom(itemChances, numberOfItems, floorNumber);

    List<string> entityNames = monsterNames.Concat(itemNames).ToList();

    foreach (string entityName in entityNames) 
    {
      Vector3Int entityPos = (Vector3Int)newRoom.RandomPoint();

      while (GameManager.instance.GetActorAtLocation(entityPos) is not null) 
      {
        entityPos = (Vector3Int)newRoom.RandomPoint();
      }

      MapManager.instance.CreateEntity(entityName, (Vector2Int)entityPos);
    }
  }
}
