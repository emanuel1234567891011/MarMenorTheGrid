using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingBar : MonoBehaviour
{
    public GameObject canvasGO;
    public TextMeshProUGUI progressText;
    public Image fillImage;

    public void Show()
    {
        canvasGO.SetActive(true);
    }

    public void Hide()
    {
        canvasGO.SetActive(false);
    }

    public void UpdateProgress(string message, int progress, int totalProgress)
    {
        fillImage.fillAmount = (float)progress / (float)totalProgress;
        progressText.text = message;
    }
}
