using System.Collections.Generic;
using UnityEngine;

public static class DirectionUtil
{
    public static Dictionary<Direction, Vector2Int> ToAxis = new Dictionary<Direction, Vector2Int>()
    {
        {Direction.North, new Vector2Int(0, 1)},
        {Direction.South, new Vector2Int(0, -1)},
        {Direction.East, new Vector2Int(1, 0)},
        {Direction.West, new Vector2Int(-1, 0)}
    };

    public static void Rotate(ref Direction dir)
    {   // It always rotates clockwise.
        switch (dir)
        {
            case Direction.East:
                dir = Direction.South;
                break;
            case Direction.South:
                dir = Direction.West;
                break;
            case Direction.West:
                dir = Direction.North;
                break;
            default:
                dir = Direction.East;
                break;
        }
    }
}
