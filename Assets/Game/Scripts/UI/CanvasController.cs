using TMPro;
using UnityEngine;

[System.Serializable]
public class PanelGroup
{
    public GameObject panelMenu;
    public GameObject panelInGame;
    public GameObject panelEndGame;
}

public class CanvasController : Singleton<CanvasController>
{
    [SerializeField] private PanelGroup panels;
    [SerializeField] private TextMeshProUGUI textStackIndicator;

    private void OnEnable()
    {
        GameManager.ActionGameStart += SetInGameUI;
        GameManager.ActionMiniGame += SetMiniGameUI;
        GameManager.ActionLevelPassed += SetEndGameUI;
    }

    private void OnDisable()
    {
        GameManager.ActionGameStart -= SetInGameUI;
        GameManager.ActionMiniGame -= SetMiniGameUI;
        GameManager.ActionLevelPassed -= SetEndGameUI;
    }

    private void SetInGameUI()
    {
        panels.panelMenu.SetActive(false);
        panels.panelInGame.SetActive(true);
    }

    private void SetMiniGameUI()
    {
        panels.panelInGame.SetActive(false);
    }

    private void SetEndGameUI()
    {
        panels.panelEndGame.SetActive(true);
    }

    private void SwitchPanels(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }

    #region UI Buttons' methods
    public void ButtonRestartPressed()
    {
        GameManager.Instance.RestartLevel();
    }

    public void ButtonStartPressed()
    {
        GameManager.ActionGameStart?.Invoke();
    }

    public void ButtonNextLevelPressed()
    {
        GameManager.Instance.LoadNextLevel();
    }
    #endregion

    public void UpdateStackIndicatorText(int stackSize)
    {
        if (textStackIndicator != null)
            textStackIndicator.text = stackSize.ToString();
    }
}
