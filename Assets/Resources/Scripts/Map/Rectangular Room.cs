using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RectangularRoom
{
    [SerializeField] private int x, y, width, height;

    public int X { get => x; } 
    public int Y { get => y; }
    public int Width { get => width; }
    public int Height { get => height; }

    public RectangularRoom(int x, int y, int width, int height, int count)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    // X1, Y1 are the top-left corner, X2, Y2 are the bottom-right corner
    public int X1 => X;
    public int Y1 => Y;
    public int X2 => X + Width;
    public int Y2 => Y + Height;

    // Get a random valid position within the room (avoiding the walls)
    public Vector2 RandomPosition()
    {
        int randomX = Random.Range(X1 + 1, X2 - 1); // Inside room, avoiding walls
        int randomY = Random.Range(Y1 + 1, Y2 - 1); // Inside room, avoiding walls
        return new Vector2(randomX, randomY);
    }

    public Vector2Int Center() => new Vector2Int(x + width / 2, y + height / 2); //returns center most points of the room

    /// <summary>
    /// Return the area of this room as a Bounds
    /// <summary>
    public Bounds GetBounds() => new Bounds(new Vector3(x, y, 0), new Vector3(width, height, 0));

    /// <summary>
    /// Return the area of this room as a BoundsInt
    /// <summary>

    public BoundsInt GetBoundsInt() => new BoundsInt(new Vector3Int(x, y, 0), new Vector3Int(width, height, 0));

    public bool Overlaps(List<RectangularRoom> otherRooms)
    {
        foreach (RectangularRoom otherRoom in otherRooms)
        {
            if (GetBounds().Intersects(otherRoom.GetBounds()))
            {
                return true;
            }
        }
        return false;
    }

    // method to check if a point is inside the room
    public bool Contains(int x, int y)
    {
        return x >= X1 && x <= X2 && y >= Y1 && y <= Y2;
    }

}
