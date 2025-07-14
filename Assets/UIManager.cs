using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] Canvas networkCanvas;
    [SerializeField] Canvas gameScreenCavas;
    [SerializeField] private GameObject networkPanel;
    [SerializeField] private GameObject initPanel;
    [SerializeField] private GameObject leavePanel;
    
    public static UIManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        DontDestroyOnLoad(gameObject);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameScreenCavas.enabled = false;
        networkPanel.SetActive(false);
        leavePanel.SetActive(false);
        
        initPanel .SetActive(true);
        networkCanvas.enabled = true;
    }

    public void LeavePanelVisibility(bool status)
    {
        leavePanel.SetActive(status);
    }


    public void NetworkCanvasVisibility(bool status)
    {
        networkCanvas.enabled = status;
    }

    public void GameSceneCanvasVisibility(bool status)
    {
        gameScreenCavas.enabled = status;
    }
    
}
