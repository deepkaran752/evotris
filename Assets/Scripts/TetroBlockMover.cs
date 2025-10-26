using Unity.VisualScripting;
using UnityEngine;

public class TetroBlockMover : MonoBehaviour
{
    #region Variables for falling
    [SerializeField] float previousTime = 0f;
    [SerializeField] float fallingTime = 0.8f;
    [SerializeField] float baseFallingTime = 0.8f;
    #endregion

    #region readonly grid value
    public static readonly int height = 20;
    public static readonly int width = 20;
    public static Transform[,] grid = new Transform[width , height];
    #endregion

    public static bool inputEnabled = true;
    #region KeyBoardButton VARS
    bool leftButtonPressed = false;
    bool rightButtonPressed = false;
    bool downButtonPressed = false;
    bool upButtonPressed = false;
    #endregion

    private int linesCleared = 0;       // Tracks number of lines cleared by this piece
    private bool wasMutatedBlock = false;  // Set when spawned

    #region RotationPoint
    [SerializeField] Vector3 rotationPoint;
    #endregion

    GameHandler gameHandler;

    void Start() =>
        gameHandler = FindAnyObjectByType<GameHandler>();

    void Update()
    {
        //input handling.
        InputHandling();

        //main movement logic
        MovementOnTheGrid();

        //rotation logic
        RotateTheTetro();
    }

    private void InputHandling()
    {
        //if input not enabled -> return;
        if (!inputEnabled) return;

        leftButtonPressed = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
        rightButtonPressed = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
        downButtonPressed = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        upButtonPressed = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
    }

    private void MovementOnTheGrid()
    {
        //move left or right in the grid.
        if (leftButtonPressed)
        {
            //play sound
            AudioManager.Instance.PlayButtonClickSound();
            transform.position += new Vector3(-1, 0, 0);
            if (!ValidMoveChecker())
                transform.position -= new Vector3(-1, 0, 0);
        }
        else if (rightButtonPressed)
        {
            //play sound
            AudioManager.Instance.PlayButtonClickSound();
            transform.position += new Vector3(1, 0, 0);
            if (!ValidMoveChecker())
                transform.position -= new Vector3(1, 0, 0);
        }

        //falling down logic
        if (Time.time - previousTime > (downButtonPressed ? fallingTime / 10 : fallingTime))
        {
            previousTime = Time.time;
            transform.position += new Vector3(0, -1, 0);
            if (!ValidMoveChecker())
            {
                transform.position -= new Vector3(0, -1, 0);
                AddToGrid();
                linesCleared = CheckForLines();
                gameHandler.OnBlockLocked(linesCleared, wasMutatedBlock);
                this.enabled = false;
            }
        }
        
    }

    private int CheckForLines()
    {
        int lines = 0;
        for(int i=  height -1; i>=0; i--)
        {
            if (HasLine(i))
            {
                DestroyLine(i);
                RowDown(i);
                lines++;
            }
        }
        return lines;
    }

    bool HasLine(int i)
    {
        for(int j = 0; j<width; j++)
        {
            if ((grid[j, i]) == null)
                return false;
        }
        return true;
    }

    void DestroyLine(int i)
    {
        for(int j=0; j<width; j++)
        {
            grid[j, i].gameObject.SetActive(false);
            grid[j, i] = null;
        }
    }

    void RowDown(int i)
    {
        for(int y = i; y<height; y++)
        {
            for(int x = 0; x<width; x++)
            {
                if (grid[x, y] != null)
                {
                    grid[x,y-1] = grid[x,y];
                    grid[x,y] = null;
                    grid[x,y-1].transform.position -= new Vector3(0,1,0);
                }
            }
        }
    }
    private void AddToGrid()
    {
        foreach (Transform child in transform)
        {
            int roundedX = Mathf.RoundToInt(child.position.x);
            int roundedY = Mathf.RoundToInt(child.position.y);
            grid[roundedX, roundedY] = child; 
        }
    }

    public void ApplyAIMeta(float fallSpeedMod, bool mutated)
    {
        fallingTime = Mathf.Clamp(baseFallingTime * (1f / Mathf.Max(0.1f, fallSpeedMod)), 0.08f, 2.0f);
        wasMutatedBlock = mutated;
    }

    private void RotateTheTetro()
    {
        if (upButtonPressed)
        {
            AudioManager.Instance.PlayButtonClickSound();
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0,0,1), 90);
            if (!ValidMoveChecker())
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
        }
    }
    private bool ValidMoveChecker()
    {
        foreach (Transform child in transform)
        {
            int roundedX = Mathf.RoundToInt(child.position.x);
            int roundedY = Mathf.RoundToInt(child.position.y);

            if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
                return false;

            if (grid[roundedX, roundedY] != null)
                return false;
        }
        return true;
    }

}
