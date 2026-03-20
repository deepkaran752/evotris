using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] GameObject arrows;
    public bool IsLeftPressed { get; private set; } = false;

    public bool IsRightPressed { get; private set; } = false;

    public bool IsDownPressed { get; private set; } = false;

    public bool IsUpPressed { get; private set; } = false;


    public void LeftButtonPressed (bool value) => IsLeftPressed = value;
    public void RightButtonPressed (bool value) => IsRightPressed = value;
    public void UpButtonPressed (bool value) => IsUpPressed = value;
    public void DownButtonPressed (bool value) => IsDownPressed = value;


    public void ResetFlags()
    {
        IsLeftPressed = false;
        IsRightPressed = false; 
        IsUpPressed = false;
        IsDownPressed = false;
    }
    private void Start()
    {
        arrows.SetActive(false);
    }
    private void Update()
    {
        //for mobile specific
        if (Application.isMobilePlatform && Input.touchCount > 0)
        {
            arrows.SetActive(true);
        } // for windows/pc
        else if (Input.anyKey)
        {
            // Handle keyboard input for desktop or mobile with keyboard
            arrows.SetActive(false);
        }
    }
}
