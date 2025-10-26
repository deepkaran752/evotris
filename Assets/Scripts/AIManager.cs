using UnityEngine;
using System.Linq;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }

    [Header("Tuning")]
    [SerializeField] int recentWindow = 10;
    [SerializeField] float easyMutateChance = 0.15f;
    [SerializeField] float hardMutateChance = 0.45f;
    [SerializeField] float minFallMod = 0.8f;
    [SerializeField] float maxFallMod = 1.6f;

    private int linesClearedRecent = 0;   // update from GameHandler
    private int avgStackHeight = 0;       // update from GameHandler (0..height)

    [SerializeField] AIEmotionManager emotionManager;
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateMetrics(int recentLines, int avgHeight)
    {
        linesClearedRecent = Mathf.Clamp(recentLines, 0, 999);
        avgStackHeight = Mathf.Clamp(avgHeight, 0, 50);
    }

    public TetroShapeData GetNextShape(int boardWidth)
    {
        // Difficulty heuristic
        bool playerDoingWell = linesClearedRecent >= 3 && avgStackHeight <= 7;
        
        // call to emotion
        if (playerDoingWell)
            ShowMessage("You are adapting.... so must I");

        float mutateChance = playerDoingWell ? hardMutateChance : easyMutateChance;

        // Pick a classic base
        int idx = Random.Range(0, TetroShapeLibrary.Classic.Length);
        var data = TetroShapeLibrary.MakeClassic(idx, GetClassicColor(idx));

        // Maybe mutate shape
        if (Random.value < mutateChance)
        {
            data.color = GetAIMutatedColor();
            data.cells = Mutate(data.cells);
            data.mutated = true;
            data.label = "Mutated";

            //call to emotion
            ShowMessage("Mutated block deployed. Let’s see you handle this.");
        }

        // Fall speed mod adapts with performance
        float t = playerDoingWell ? 1f : 0f; // 1=hard, 0=easy
        data.fallSpeedMod = Mathf.Lerp(minFallMod, maxFallMod, t);

        // Slight color cue if mutated
        if (data.mutated) data.color = Color.Lerp(data.color, Color.magenta, .35f);
        return data;
    }

    // Simple, safe mutation: add or remove a cell near existing cells (keep size 4 or 5)
    private Vector2Int[] Mutate(Vector2Int[] src)
    {
        var list = src.ToList();

        // 50% add (cap 5), 50% shift one cell
        if (Random.value < 0.5f && list.Count < 5)
        {
            var anchor = list[Random.Range(0, list.Count)];
            var dirs = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            var candidate = anchor + dirs[Random.Range(0, dirs.Length)];
            if (!list.Contains(candidate)) list.Add(candidate);
        }
        else
        {
            // Nudge one existing cell to a neighbor to create odd shapes
            int i = Random.Range(0, list.Count);
            var dirs = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            var moved = list[i] + dirs[Random.Range(0, dirs.Length)];
            if (!list.Contains(moved)) list[i] = moved;
        }

        // Normalize so centroid/pivot near (0,0)
        int minX = list.Min(v => v.x), minY = list.Min(v => v.y);
        for (int i = 0; i < list.Count; i++) list[i] = new Vector2Int(list[i].x - minX, list[i].y - minY);
        // Shift pivot roughly to middle
        int maxX = list.Max(v => v.x);
        int shift = maxX / 2;
        for (int i = 0; i < list.Count; i++) list[i] = new Vector2Int(list[i].x - shift, list[i].y);

        return list.ToArray();
    }

    #region EmotionManager Helper Methods
    public void ShowMessage(string message, bool showcase = false) => emotionManager.ShowMessage(message, showcase);
    #endregion

    #region Color Helper Methods
    private Color GetClassicColor(int index)
    {
        // Vibrant classic palette
        Color[] classicColors = {
        new Color(1f, 0.92f, 0.23f), // Yellow O
        new Color(0f, 0.69f, 1f),    // Cyan I
        new Color(0.41f, 0.94f, 0.68f), // Green S
        new Color(1f, 0.32f, 0.32f), // Red Z
        new Color(1f, 0.57f, 0f),    // Orange L
        new Color(0.67f, 0f, 1f),    // Indigo J
        new Color(1f, 0.25f, 0.51f)  // Pink T
    };
        return classicColors[Mathf.Clamp(index, 0, classicColors.Length - 1)];
    }

    private Color GetAIMutatedColor()
    {
        // Neon colors to signify AI mutation
        Color[] aiColors = {
        new Color(0.58f, 0f, 1f),     // Neon Purple
        new Color(0f, 1f, 0.93f),     // Neon Teal
        new Color(1f, 0f, 0.53f),     // Hot Pink
        new Color(0.3f, 0.9f, 0.1f)   // Neon Lime
    };
        return aiColors[Random.Range(0, aiColors.Length)];
    }
    #endregion
}
