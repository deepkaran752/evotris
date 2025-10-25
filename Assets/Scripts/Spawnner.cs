using UnityEngine;

public class Spawnner : MonoBehaviour
{
    [Header("AI-Based Spawning System")]
    [SerializeField] private GameObject pieceRootPrefab;  // Empty object with TetroBlockMover
    [SerializeField] private GameObject blockCellPrefab;  // Single cell prefab (square block)
    [SerializeField] private GameHandler gameHandler;

    [Header("Spawn Position")]
    [SerializeField] private Vector3 spawnWorldPos = new(); // adjust to your grid center

    private void Awake()
    {
        spawnWorldPos = transform.position;
    }
    void Start() => GenerateTetrisBlock();

    void OnEnable()
    {
        gameHandler.canSpawnNewTetris -= GenerateTetrisBlock;
        gameHandler.canSpawnNewTetris += GenerateTetrisBlock;
    }

    void OnDisable() =>
        gameHandler.canSpawnNewTetris -= GenerateTetrisBlock;

    public void GenerateTetrisBlock()
    {
        // 1. Ask AI for next block shape
        TetroShapeData shapeData = AIManager.Instance.GetNextShape(boardWidth: 20);

        if (IsGameOver(shapeData))
        {
            Debug.Log("GAME OVER!");
            gameHandler.triggerGameOver?.Invoke();
            return;
        }
        // 2. Spawn a new root object to hold this piece
        GameObject pieceRoot = Instantiate(pieceRootPrefab, spawnWorldPos, Quaternion.identity);

        // 3. Get the movement script to apply speed controls
        var mover = pieceRoot.GetComponent<TetroBlockMover>();
        if (mover != null)
            mover.ApplyAIMeta(shapeData.fallSpeedMod, shapeData.mutated);

        // 4. Create block cells for each shape position
        foreach (Vector2Int cell in shapeData.cells)
        {
            GameObject block = Instantiate(blockCellPrefab, pieceRoot.transform);
            block.transform.localPosition = new Vector3(cell.x, cell.y, 0);

            // Assign color from AI data
            var renderer = block.GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.color = shapeData.color;
        }

        // 5. (Optional TA-DA moment): display AI label in UI
        Debug.Log($"Spawned: {shapeData.label} | Mutated: {shapeData.mutated} | FallSpeed: {shapeData.fallSpeedMod}");
    }

    //Game Over logic
    private bool IsGameOver(TetroShapeData shapeData)
    {
        foreach (var cell in shapeData.cells)
        {
            int x = Mathf.RoundToInt(spawnWorldPos.x + cell.x);
            int y = Mathf.RoundToInt(spawnWorldPos.y + cell.y);

            if (TetroBlockMover.grid[x, y] != null)
                return true;
        }
        return false;
    }
}
