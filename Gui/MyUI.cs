using EpicMMOSystem.OtherApi;
using HarmonyLib;
using LocalizationManager;
using UnityEngine;
using UnityEngine.UI;


namespace EpicMMOSystem;

public static partial class MyUI
{
    private static GameObject UI;
    private static Localizationold localization => EpicMMOSystem.localizationold;
    public static bool IsPanelVisible()
    {
        return UI && UI.activeSelf;
    }

    public static void Init()
    {
        UI = EpicMMOSystem.Instantiate(EpicMMOSystem._asset.LoadAsset<GameObject>("LevelHud"));
        EpicMMOSystem.DontDestroyOnLoad(UI);
        UI.SetActive(false);
        InitNavigationPanel();
        InitLevelSystem();
        InitFriendsList();
        UI.GetComponentInChildren<Canvas>().sortingOrder = 1510;
        //Localizer.Load();
    }

    public static void Hide()
    {
        navigationPanel.SetActive(false);
        levelSystemPanel.SetActive(false);
        levelSystemPanel.GetComponent<CanvasGroup>().interactable = true;
        alertResetPointPanel.SetActive(false);
        friendsListPanel.SetActive(false);
        friendsListPanel.GetComponent<CanvasGroup>().interactable = true;
        alertResetPointPanel.SetActive(false);
        addFriendAlert.SetActive(false);
        UI.SetActive(false);
        LevelSystem.Instance.cancelDepositPoints();
        DonatShop_API.HidePanel();
    }

    public static void Show()
    {
        ShowNavigationPanel();
        navigationPanel.SetActive(true);
        UI.SetActive(true);
        
    }

    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.Show))]
    public static class ShowPanel_Path
    {
        public static void Postfix()
        {
            Show();
        }
    }
    
    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.Hide))]
    public static class HidePanel_Path
    {
        public static void Postfix()
        {
            Hide();
        }
    }
    
    [HarmonyPatch(typeof(Menu), nameof(Menu.IsVisible))]
    private static class BufferUIFix
    {
        private static void Postfix(ref bool __result)
        {
            if (textField.isFocused) __result = true;
        }
    }

}