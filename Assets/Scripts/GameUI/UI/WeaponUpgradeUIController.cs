// Author: Yuhan Tang
// Created by: Yuhan Tang
// Modified by: Yuhan Tang
// No external source was used.
using UnityEngine;

public class WeaponUpgradeUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject selectPanel;
    [SerializeField] private GameObject resultPanel;

    private void Start()
    {
        ShowSelect();
    }

    public void OnSelectConfirm()
    {
        ShowResult();
    }

    public void OnResultConfirm()
    {
        ShowSelect();
    }

    private void ShowSelect()
    {
        if (selectPanel != null) selectPanel.SetActive(true);
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    private void ShowResult()
    {
        if (selectPanel != null) selectPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(true);
    }
}
