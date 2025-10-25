using Unity.VisualScripting;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public System.Action canSpawnNewTetris;

    public System.Action triggerGameOver;

    public System.Action<int> onScoreUpdated;

    public int Score { get; private set; } = 0;
    private bool isGameOver = false;

    //AI metrics tracking (optional but useful)
    private int recentLinesCleared = 0;
    private int stackPressure = 0;  // you can calculate based on grid height

    private void Start()
    {

    }
    public void OnBlockLocked(int linesCleared, bool wasMutatedBlock)
    {
        AddScore(linesCleared, wasMutatedBlock);
        UpdateMetrics(linesCleared);

        // Spawn next piece if not game over
        if (!isGameOver)
            canSpawnNewTetris?.Invoke();
    }
    public void AddScore(int linesCleared, bool wasMutatedBlock)
    {
        int pointsAwarded = 0;
        switch (linesCleared)
        {
            case 1: pointsAwarded = 100; break;
            case 2: pointsAwarded = 300; break;
            case 3: pointsAwarded = 500; break;
            case 4: pointsAwarded = 800; break;
        }

        // Bonus multiplier for AI-mutated blocks (unique EvoTris twist)
        if (wasMutatedBlock && linesCleared > 0)
            pointsAwarded = Mathf.RoundToInt(pointsAwarded * 1.5f);

        Score += pointsAwarded;
        onScoreUpdated?.Invoke(Score); // update UI
        Debug.Log($"Score Updated! +{pointsAwarded}, Total Score = {Score}");
    }

    // This feeds data into the AI for next block decisions
    public void UpdateMetrics(int linesCleared)
    {
        recentLinesCleared = linesCleared;
        stackPressure = CalculateStackPressure();
        AIManager.Instance.UpdateMetrics(recentLinesCleared, stackPressure);
    }

    // Measures how high the stack is
    private int CalculateStackPressure()
    {
        for (int y = TetroBlockMover.height - 1; y >= 0; y--)
        {
            for (int x = 0; x < TetroBlockMover.width; x++)
            {
                if (TetroBlockMover.grid != null && TetroBlockMover.grid[x, y] != null)
                {
                    return TetroBlockMover.height - y; // higher = more danger
                }
            }
        }
        return 0; // empty
    }


}
