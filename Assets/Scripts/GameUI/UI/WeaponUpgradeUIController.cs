// Author: Yuhan Tang
// Created by: Yuhan Tang
// Modified by: Yuhan Tang
// No external source was used.
//using UnityEngine;

//public class WeaponUpgradeUIController : MonoBehaviour
//{
//    [Header("Panels")]
//    [SerializeField] private GameObject selectPanel;
//    [SerializeField] private GameObject resultPanel;

//    private void Start()
//    {
//        ShowSelect();
//    }

//    public void OnSelectConfirm()
//    {
//        ShowResult();
//    }

//    public void OnResultConfirm()
//    {
//        ShowSelect();
//    }

//    private void ShowSelect()
//    {
//        if (selectPanel != null) selectPanel.SetActive(true);
//        if (resultPanel != null) resultPanel.SetActive(false);
//    }

//    private void ShowResult()
//    {
//        if (selectPanel != null) selectPanel.SetActive(false);
//        if (resultPanel != null) resultPanel.SetActive(true);
//    }
//}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEngine.UIElements;

public class WeaponUpgradeUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject selectPanel;
    [SerializeField] private GameObject resultPanel;

    [Header("Select - Left Area UI")]
    [SerializeField] private Image weaponIcon;                 // SelectPanel/.../InfoBlock/WeaponIcon
    [SerializeField] private TMP_Text weaponInfoText;          // SelectPanel/.../InfoBlock/WeaponInfoText
    [SerializeField] private TMP_Text materialNameText;        // SelectPanel/.../MaterialRow/MaterialNameText
    [SerializeField] private TMP_Text materialNumberText;      // SelectPanel/.../MaterialRow/MaterialNumberText

    [Header("Select - Buttons")]
    [SerializeField] private Button confirmButton;             // SelectPanel/.../ButtonRow/ConfirmButton
    [SerializeField] private Button cancelButton;              // SelectPanel/.../ButtonRow/CancelButton

    [Header("Weapon List Toggles (Right Area)")]
    [SerializeField] private Toggle weaponBtnKnife;            // RightArea/WeaponList/WeaponBtn_Knife
    [SerializeField] private Toggle weaponBtnPistol;           // RightArea/WeaponList/WeaponBtn_Pistol
    [SerializeField] private Toggle weaponBtnShotgun;          // RightArea/WeaponList/WeaponBtn_Shotgun

    [Header("Result UI")]
    [SerializeField] private Image resultWeaponIcon;           // ResultPanel/ResultContent/ResultWeaponIcon
    [SerializeField] private TMP_Text resultInfoText;          // ResultPanel/ResultContent/ResultInfoText
    [SerializeField] private Button resultConfirmButton;       // ResultPanel/ResultConfirmButton

    private string selectedWeaponName = "Knife";

    private void Start()
    {
        Debug.Log("GameValue.Instance is null? " + (GameValue.Instance == null));
        // 绑定按钮的事件
        if (weaponBtnKnife != null)
            weaponBtnKnife.onValueChanged.AddListener(isOn => { if (isOn) SelectWeapon("Knife"); });

        if (weaponBtnPistol != null)
            weaponBtnPistol.onValueChanged.AddListener(isOn => { if (isOn) SelectWeapon("Pistol"); });

        if (weaponBtnShotgun != null)
            weaponBtnShotgun.onValueChanged.AddListener(isOn => { if (isOn) SelectWeapon("Shotgun"); });

        if (confirmButton != null) confirmButton.onClick.AddListener(OnSelectConfirm);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnSelectCancel);
        if (resultConfirmButton != null) resultConfirmButton.onClick.AddListener(OnResultConfirm);

        ShowSelect();
        RefreshSelectUI();
    }

    private void SelectWeapon(string weaponName)
    {
        selectedWeaponName = weaponName;
        RefreshSelectUI();
    }

    private void RefreshSelectUI()
    {
        var player = GameValue.Instance.GetPlayerValue();
        var current = FindPlayerWeapon(player, selectedWeaponName);

        if (current == null)
        {
            SetLeftUI($"No {selectedWeaponName} owned.", "", "");
            if (confirmButton != null) confirmButton.interactable = false;
            return;
        }

        if (weaponIcon != null) weaponIcon.sprite = current.WeaponSprite;
        if (weaponInfoText != null) weaponInfoText.text = $"{current.WeaponName} LV{current.weaponLevel}";

        // 武器满级
        if (current.weaponLevel >= current.maxLevel)
        {
            SetMaterialUI("MAX LEVEL", "");
            if (confirmButton != null) confirmButton.interactable = false;
            return;
        }

        // 武器的下一个等级
        int nextLevel = current.weaponLevel + 1;
        var next = GameValue.Instance.GetWeaponByNameAndLevel(current.WeaponName, nextLevel);

        if (next == null)
        {
            SetMaterialUI("Next level data missing", "");
            if (confirmButton != null) confirmButton.interactable = false;
            return;
        }

        // 升级到下一级需要的材料
        string mat = next.upgradeMaterialName;
        int need = next.upgradeMaterialNeed;
        int have = player.GetMaterialCount(mat);

        SetMaterialUI(mat, $"x{need}");

        if (confirmButton != null)
        {
            // 测试，如果升级要求是0也能升级
            confirmButton.interactable = (string.IsNullOrEmpty(mat) || need <= 0) ? true : (have >= need);
        }
    }

    public void OnSelectConfirm()
    {
        var player = GameValue.Instance.GetPlayerValue();
        var current = FindPlayerWeapon(player, selectedWeaponName);

        if (current == null)
        {
            ShowResultMessage($"Upgrade Failed: No {selectedWeaponName} owned.");
            return;
        }

        if (current.weaponLevel >= current.maxLevel)
        {
            ShowResultMessage($"Upgrade Failed: {current.WeaponName} is already MAX level.");
            return;
        }

        int nextLevel = current.weaponLevel + 1;
        var next = GameValue.Instance.GetWeaponByNameAndLevel(current.WeaponName, nextLevel);
        if (next == null)
        {
            ShowResultMessage($"Upgrade Failed: Next level data not found ({current.WeaponName} LV{nextLevel}).");
            return;
        }

        string mat = next.upgradeMaterialName;
        int need = next.upgradeMaterialNeed;

        if (!string.IsNullOrEmpty(mat) && need > 0)
        {
            int have = player.GetMaterialCount(mat);
            if (have < need)
            {
                ShowResultMessage($"Upgrade Failed: Need {mat} x{need}, but you have x{have}.");
                return;
            }

            if (!player.TrySpendMaterial(mat, need))
            {
                ShowResultMessage($"Upgrade Failed: Spend material error ({mat} x{need}).");
                return;
            }
        }

        // 替换玩家武器库里当前的
        ReplacePlayerWeapon(player, current, next);

        if (player.EquipmentWeapon != null &&
            player.EquipmentWeapon.WeaponName == current.WeaponName &&
            player.EquipmentWeapon.weaponLevel == current.weaponLevel)
        {
            player.EquipmentWeapon = next;
        }

        ShowResultMessage($"Upgrade Success!\n{current.WeaponName}: LV{current.weaponLevel} -> LV{next.weaponLevel}");
    }

    public void OnSelectCancel()
    {
        ShowSelect();
        RefreshSelectUI();
    }

    public void OnResultConfirm()
    {
        ShowSelect();
        RefreshSelectUI();
    }

    private WeaponValue FindPlayerWeapon(PlayerValue player, string weaponName)
    {
        // 在玩家背包找最高等级的那把武器
        WeaponValue best = null;
        foreach (var w in player.HadWeaponsLibrary)
        {
            if (w != null && w.WeaponName == weaponName)
            {
                if (best == null || w.weaponLevel > best.weaponLevel)
                    best = w;
            }
        }
        return best;
    }

    private void ReplacePlayerWeapon(PlayerValue player, WeaponValue current, WeaponValue next)
    {
        for (int i = 0; i < player.HadWeaponsLibrary.Count; i++)
        {
            var w = player.HadWeaponsLibrary[i];
            if (w != null && w.WeaponName == current.WeaponName && w.weaponLevel == current.weaponLevel)
            {
                player.HadWeaponsLibrary[i] = next;
                return;
            }
        }

        // 测试：如果没有直接添加
        player.HadWeaponsLibrary.Add(next);
    }

    private void ShowResultMessage(string msg)
    {
        // 结果面板图标和文字
        var player = GameValue.Instance.GetPlayerValue();
        var current = FindPlayerWeapon(player, selectedWeaponName);

        if (resultWeaponIcon != null && current != null)
            resultWeaponIcon.sprite = current.WeaponSprite;

        if (resultInfoText != null)
            resultInfoText.text = msg;

        ShowResult();
    }

    private void SetLeftUI(string weaponText, string matName, string matNum)
    {
        if (weaponInfoText != null) weaponInfoText.text = weaponText;
        SetMaterialUI(matName, matNum);
    }

    private void SetMaterialUI(string matName, string matNum)
    {
        if (materialNameText != null) materialNameText.text = matName;
        if (materialNumberText != null) materialNumberText.text = matNum;
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
