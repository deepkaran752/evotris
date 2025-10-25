using UnityEngine;

public static class TetroShapeLibrary
{
    // O, I, S, Z, L, J, T - Using positive coordinates & consistent rotation points
    public static readonly Vector2Int[][] Classic = {
        new []{ new Vector2Int(0,0), new(1,0), new(0,1), new(1,1) },                    // O
        new []{ new Vector2Int(0,1), new(1,1), new(2,1), new(3,1) },                    // I
        new []{ new Vector2Int(1,0), new(2,0), new(0,1), new(1,1) },                    // S
        new []{ new Vector2Int(0,0), new(1,0), new(1,1), new(2,1) },                    // Z
        new []{ new Vector2Int(2,0), new(0,1), new(1,1), new(2,1) },                    // L
        new []{ new Vector2Int(0,0), new(0,1), new(1,1), new(2,1) },                    // J
        new []{ new Vector2Int(0,0), new(1,0), new(2,0), new(1,1) },                    // T
    };

    // Matching correct rotation points based on cell layout
    public static readonly Vector2[] RotationPoints = {
        new (0.5f, 0.5f),   // O
        new (1.5f, 1f),     // I
        new (1f, 1f),       // S
        new (1f, 1f),       // Z
        new (1f, 1f),       // L
        new (1f, 1f),       // J
        new (1f, 0f)        // T 
    };

    public static TetroShapeData MakeClassic(int index, Color c) => new()
    {
        cells = Classic[Mathf.Clamp(index, 0, Classic.Length - 1)],
        rotationPoint = RotationPoints[Mathf.Clamp(index, 0, RotationPoints.Length - 1)],
        color = c,
        fallSpeedMod = 1f,
        label = "Classic",
        mutated = false
    };
}
