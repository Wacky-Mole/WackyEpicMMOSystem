using System.Collections.Generic;
using EpicMMOSystem.OtherApi;
using HarmonyLib;
using LocalizationManager;
using UnityEngine;
using UnityEngine.UI;


namespace EpicMMOSystem;

public static partial class MyUI
{
    private static GameObject UI;
    private static readonly List<StyleTarget> StyleTargets = new();
    private static readonly HashSet<GameObject> RegisteredTargets = new();
    private static InventoryGui styleSource;
    private static Text[] sourceTexts = new Text[0];
    private static ButtonStyleTarget[] sourceButtons = new ButtonStyleTarget[0];
    private static Localizationold localization => EpicMMOSystem.localizationold;

    private sealed class StyleTarget
    {
        public readonly Text[] Texts;
        public readonly ButtonStyleTarget[] Buttons;

        public StyleTarget(GameObject target)
        {
            Texts = target.GetComponentsInChildren<Text>(true);
            Button[] buttons = target.GetComponentsInChildren<Button>(true);
            Buttons = new ButtonStyleTarget[buttons.Length];
            for (int i = 0; i < buttons.Length; i++)
            {
                Buttons[i] = new ButtonStyleTarget(buttons[i]);
            }
        }
    }

    private sealed class ButtonStyleTarget
    {
        public readonly Button Button;
        public readonly Image Image;
        public readonly bool HasText;

        public ButtonStyleTarget(Button button)
        {
            Button = button;
            Image = button.targetGraphic as Image;
            HasText = button.GetComponentInChildren<Text>(true) != null;
        }
    }

    internal static void RegisterForVanillaUIStyle(GameObject target)
    {
        if (target == null || !RegisteredTargets.Add(target))
        {
            return;
        }

        StyleTargets.Add(new StyleTarget(target));
        if (InventoryGui.instance != null)
        {
            ApplyVanillaUIStyle(InventoryGui.instance);
        }
    }

    internal static void ApplyVanillaUIStyle(InventoryGui source)
    {
        if (source == null)
        {
            return;
        }

        if (styleSource != source)
        {
            styleSource = source;
            sourceTexts = source.GetComponentsInChildren<Text>(true);
            Button[] buttons = source.GetComponentsInChildren<Button>(true);
            sourceButtons = new ButtonStyleTarget[buttons.Length];
            for (int i = 0; i < buttons.Length; i++)
            {
                sourceButtons[i] = new ButtonStyleTarget(buttons[i]);
            }
        }

        Text sourceText = null;
        foreach (Text text in sourceTexts)
        {
            if (text != null && text.font != null)
            {
                sourceText = text;
                break;
            }
        }

        ButtonStyleTarget sourceButton = null;
        ButtonStyleTarget fallbackButton = null;
        foreach (ButtonStyleTarget buttonStyle in sourceButtons)
        {
            if (buttonStyle.Button == null || buttonStyle.Image == null || buttonStyle.Image.sprite == null)
            {
                continue;
            }

            if (fallbackButton == null)
            {
                fallbackButton = buttonStyle;
            }

            if (buttonStyle.HasText)
            {
                sourceButton = buttonStyle;
                break;
            }
        }

        if (sourceButton == null)
        {
            sourceButton = fallbackButton;
        }

        foreach (StyleTarget target in StyleTargets)
        {
            foreach (Text text in target.Texts)
            {
                if (text != null && sourceText != null)
                {
                    text.font = sourceText.font;
                }
            }

            if (sourceButton == null)
            {
                continue;
            }

            foreach (ButtonStyleTarget buttonStyle in target.Buttons)
            {
                Button button = buttonStyle.Button;
                if (button == null)
                {
                    continue;
                }

                button.transition = sourceButton.Button.transition;
                button.colors = sourceButton.Button.colors;
                button.spriteState = sourceButton.Button.spriteState;

                if (buttonStyle.HasText && buttonStyle.Image != null && sourceButton.Image != null)
                {
                    buttonStyle.Image.sprite = sourceButton.Image.sprite;
                    buttonStyle.Image.type = sourceButton.Image.type;
                    buttonStyle.Image.color = sourceButton.Image.color;
                    buttonStyle.Image.material = sourceButton.Image.material;
                }
            }
        }
    }

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
        RegisterForVanillaUIStyle(UI);
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
       // DonatShop_API.HidePanel();
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
            if (InventoryGui.instance != null)
            {
                ApplyVanillaUIStyle(InventoryGui.instance);
            }

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