using UnityEngine;

[System.Serializable]
public class TetroShapeData
{
    public Vector2Int[] cells;      // Block shape definition
    public Color color = Color.white;
    public float fallSpeedMod = 1f;
    public string label = "Classic";
    public bool mutated = false;
    public Vector2 rotationPoint;   //NEW: Used by your rotation logic
}

