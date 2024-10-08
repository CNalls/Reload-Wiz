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
}
