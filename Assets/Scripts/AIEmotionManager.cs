using UnityEngine;
using System.Collections;
using TMPro;

public class AIEmotionManager : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI aiDialogueText;
    [SerializeField] private TextMeshProUGUI aiBottomDialogueText;
    [SerializeField] private UnityEngine.UI.Image bgImageShow;
    [SerializeField] private SpriteRenderer monitorScreenImage;

    [Header("Timing")]
    [SerializeField] private float messageDuration = 2.5f;
    private float timer = 0f;

    private void Start()
    {
        ClearText();
        bgImageShow.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                ClearText();
            }
        }
    }

    public void ShowMessage(string message, bool showcase = false)
    {

        bgImageShow.gameObject.SetActive(showcase);
        aiDialogueText.text = message;
        aiBottomDialogueText.text = showcase? "" : message;
        timer = messageDuration;  // reset timer every time a message is shown

    }

    private void ClearText()
    {
        aiBottomDialogueText.text = "";
    }

    //flickering
    public void TriggerScreenFlicker(float intensity, float duration)
    {
        StartCoroutine(ScreenFlickerRoutine(intensity, duration));
    }

    private IEnumerator ScreenFlickerRoutine(float intensity, float duration)
    {
        Color original = monitorScreenImage.color;

        // Quick flicker animation
        for (int i = 0; i < 3; i++)
        {
            monitorScreenImage.color = new Color(intensity, intensity, intensity, 1);
            yield return new WaitForSeconds(duration);
            monitorScreenImage.color = original;
            yield return new WaitForSeconds(duration / 2);
        }
    }
}