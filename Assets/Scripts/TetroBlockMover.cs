using Unity.VisualScripting;
using UnityEngine;

public class TetroBlockMover : MonoBehaviour
{
    #region Variables for falling
    [SerializeField] float previousTime = 0f;
    [SerializeField] float fallingTime = 0.8f;
    #endregion

    #region readonly grid value
    static readonly int height = 20;
    static readonly int width = 20;
    #endregion

    #region KeyBoardButton VARS
    bool leftButtonPressed = false;
    bool rightButtonPressed = false;
    bool downButtonPressed = false;
    bool upButtonPressed = false;
    #endregion

    #region RotationPoint
    [SerializeField] Vector3 rotationPoint;
    #endregion
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
            transform.position += new Vector3(-1, 0, 0);
            if (!ValidMoveChecker())
                transform.position -= new Vector3(-1, 0, 0);
        }
        else if (rightButtonPressed)
        {
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
                this.enabled = false;
            }
        }
        
    }

    private void RotateTheTetro()
    {
        if (upButtonPressed)
        {
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
            {
                return false;
            }
        }
        return true;
    }

}
