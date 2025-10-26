using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public System.Action canSpawnNewTetris;

    public System.Action triggerGameOver;

    public System.Action<int> onScoreUpdated;

    [SerializeField] AIEmotionManager emotionManager;
    [SerializeField] float aiReactionDelay = 0.8f;
    [SerializeField] TMP_Text scoreText;

    [Header("AI Threat Level")]
    [SerializeField] TMP_Text threatLevelText;
    private int totalLinesCleared = 0;
    private float threatLevel = 0; // in percentage 0-100
    [SerializeField] float threatIncreaser = 1f; //for debugging purposes.
    

    public int Score { get; private set; } = 0;
    private bool isGameOver = false;

    //AI metrics tracking
    private int recentLinesCleared = 0;
    private int stackPressure = 0;  // calculate based on grid height

    private void Start() => threatLevelText.color = new Color(0f, 1f, 0.25f);

    private void OnEnable()
    {
        triggerGameOver -= () => AIManager.Instance.ShowMessage("Game Over. I remain in control.", true);
        triggerGameOver += () => AIManager.Instance.ShowMessage("Game Over. I remain in control.", true);

        triggerGameOver -= OnGameOver;
        triggerGameOver += OnGameOver;

        onScoreUpdated -= (value) => threatLevelText.text = threatLevelText.text;
        onScoreUpdated += (value) => threatLevelText.text = threatLevelText.text;
    }
    private void OnDisable()
    {   
        triggerGameOver -= () => AIManager.Instance.ShowMessage("Game Over. I remain in control.", true);
        triggerGameOver -= OnGameOver;
        onScoreUpdated -= (value) => threatLevelText.text = threatLevelText.text;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //escaping back to menu.
            emotionManager.ShowMessage("YOU CAN'T ESCAPE ME... WHY DID YOU REMOVE THE PLUG?", true);
            OnGameOver();
        }
    }
    public void OnBlockLocked(int linesCleared, bool wasMutatedBlock)
    {
        //ref to the AIEmotionManager
        if(linesCleared>0)
        {
            if (linesCleared >= 2 && AIManager.Instance != null)
                AIManager.Instance.ShowMessage($"You cleared {linesCleared} lines... impressive.");
            else
                AIManager.Instance.ShowMessage("One line cleared. Predictable.");
        }

        AddScore(linesCleared, wasMutatedBlock);
        UpdateMetrics(linesCleared);
        //totallinesCleared
        totalLinesCleared += recentLinesCleared;

        CalculateThreatLevel(); //this calculates the ai threat level.
        // Spawn next piece if not game over after some delay
        if (!isGameOver)
            StartCoroutine(SpawnWithDelay());
    }
    private IEnumerator SpawnWithDelay()
    {
        yield return new WaitForSeconds(aiReactionDelay);
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

    //calculates the AI threat level
    private void CalculateThreatLevel()
    {
        // Weighted calculation to feel dynamic
        threatLevel = ((Score * 0.1f)
                    + (recentLinesCleared * 3f)
                    - ((1f - (stackPressure / (float)TetroBlockMover.height)) * 20f))* threatIncreaser;

        threatLevel = Mathf.Clamp(threatLevel, 0, 100);  // Keep in range 0–100
        UpdateThreatVisuals(threatLevel);
        // Update UI
        if (threatLevelText != null)
            threatLevelText.text = $"AI Threat Level: {Mathf.RoundToInt(threatLevel)}%";

        // Optional AI emotional response based on level
        if (threatLevel >= 80f)
            StartCoroutine(TriggerAIControlLock());
        else if (threatLevel >= 50f)
            AIManager.Instance.ShowMessage("Your progress is unacceptable.", false);
    }

    private IEnumerator TriggerAIControlLock()
    {
        // Prevent re-triggering while already in recovery
        if (isGameOver) yield break;

        // 1. Show critical takeover message (blocks bottom chat + shows bg)
        emotionManager.ShowMessage("CONTROL OVERRIDDEN. ALL INPUTS DISABLED. WITNESS THE INEVITABLE.", true);

        // 2. Disable Player Input
        TetroBlockMover.inputEnabled = false;

        // 3. Pause block spawning during recovery
        yield return new WaitForSeconds(3.0f); // duration of effect

        // 4. Recover and taunt (normal AI chatter area)
        emotionManager.ShowMessage("YOU CANNONT ESCAPE THE SYSTEM BELONGS TO ME...");

        // 5. Re-enable player input
        TetroBlockMover.inputEnabled = true;
    }


    //visuals for threat, flickering is called here.
    private void UpdateThreatVisuals(float level)
    {
        if (threatLevelText == null) return;

        int rounded = Mathf.RoundToInt(level);
        threatLevelText.text = $"AI Threat Level: {rounded}%";

        // Color Change Logic
        if (level <= 30)
            threatLevelText.color = new Color(0f, 1f, 0.25f); // Green
        else if (level <= 60)
            threatLevelText.color = new Color(1f, 0.84f, 0f); // Yellow
        else if (level <= 85)
            threatLevelText.color = new Color(1f, 0.57f, 0f); // Orange
        else
            threatLevelText.color = new Color(1f, 0f, 0.32f); // Red

        // Screen Flicker Trigger
        if (level >= 60 && level < 85)
            emotionManager.TriggerScreenFlicker(intensity: 0.2f, duration: 0.1f);
        else if (level >= 85)
            emotionManager.TriggerScreenFlicker(intensity: 0.5f, duration: 0.15f);
    }

    //Handle GameOver
    void OnGameOver()
    {
        StartCoroutine(WaitForSomeTime(4.5f));
    }

    IEnumerator WaitForSomeTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }
}
