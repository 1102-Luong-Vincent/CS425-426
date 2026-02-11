using TMPro.Examples;
using UnityEngine;

public class PanelControl : MonoBehaviour
{
    public GameObject Panel;


    public void SetActive(bool isActive)
    {
        if (isActive)
        {
            ShowPanel();
        }
        else
        {
            HidePanel();
        }
    }


    public virtual void ShowPanel()
    {
        Debug.Log("showpanel: " + this.gameObject.name);
        Panel.SetActive(true);
    }

    public virtual void HidePanel()
    {
        if (Panel == null)
        {
            Debug.LogWarning($"HidePanel() s{gameObject.name}"); 
            return;
        }
            
        Panel.SetActive(false);
    }

}
