using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Tilemaps;

//a lot of parts are wip having to do with generating the full 360 walls
sealed class ProcGen
{
    /// <summary>
    /// Generate a new dungeon map
    /// </summary>
    public void GenerateDungeon(int mapWidth, int mapHeight, int roomMaxSize, int roomMinSize, int maxRooms, int maxMonstersPerRoom, List<RectangularRoom> rooms)
    {
        //Generate the rooms
        for (int roomNum = 0; roomNum < maxRooms; roomNum++) //room num = 0 and if less that max rooms increment +
        {
            int roomWidth = Random.Range(roomMinSize, roomMaxSize);
            int roomHeight = Random.Range(roomMinSize, roomMaxSize);

            int roomX = Random.Range(0, mapWidth - roomWidth - 1);
            int roomY = Random.Range(0, mapHeight - roomHeight - 1);

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
            else
            {
                
            }

            PlaceActors(newRoom, maxMonstersPerRoom);
            rooms.Add(newRoom);
        }

        //The first room, where player starts
        MapManager.instance.CreateEntity("Player", rooms[0].Center());
    }

    /// <summary>
    ///    Return an L-Shaped tunnel between the 2 points using Bresenham lines
    /// </summary>
    
    private void TunnelBetween(RectangularRoom oldRoom, RectangularRoom newRoom)
    {
        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();    
        Vector2Int tunnelCorner; 

        if (Random.value < 0.5f)
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
    private void PlaceActors(RectangularRoom newRoom, int maximumMonsters) 
    {
        int numberOfMonsters = Random.Range(0, maximumMonsters + 1);

        for (int monster = 0; monster < numberOfMonsters;) 
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

        if (Random.value < 0.8f) 
        {
            MapManager.instance.CreateEntity("Orc", new Vector2(x, y));
        } 
        else 
        {
            MapManager.instance.CreateEntity("Troll", new Vector2(x, y));
        }
        /*else
        {
            MapManager.instance.CreateEntity("Goblin", new Vector2(x, y));
        }
        else
        {
            MapManager.instance.CreateEntity("Skeleton", new Vector2(x, y));
        }
        else
        {
            MapManager.instance.CreateEntity("Zombie", new Vector2(x, y));
        }*/
        monster++;
        }
    }
}
