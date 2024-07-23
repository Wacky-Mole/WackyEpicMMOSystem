using EpicMMOSystem.MonoScripts;
using HarmonyLib;
using LocalizationManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EpicMMOSystem;

public partial class MyUI
{
    internal static GameObject levelSystemPanel;
    internal static GameObject levelSystemPanelRoot;
    //AlertsApplyReset
    private static GameObject alertResetPointPanel;
    private static Text alertResetPointText;
    //Stats
    private static List<ParameterButton> parameterButtons = new ();
    private static GameObject freePointsPanel;
    private static Text freePointsText;
    //CurentLevel
    private static Text currentLevelText;
    private static Text expText;
    

    //Description
    //Description strength
    private static Text physicDamageText;
    private static Text addWeightText;
    private static Text criticalDmgMultText;

    //Description dexterity
    private static Text attackSpeedMultiText;
    private static Text reducedStaminaText; 
    private static Text attackStamina;

    //Description intellect
    private static Text magicDamageText;  
    private static Text magicEitrRegText;
    private static Text eitrIncreaseText;

    //Description endurance
    
    private static Text addStaminaText;
    private static Text staminaRegen;
    private static Text reducedStaminaBlockText;
    private static Text physicArmorText;

    //Description Vigour
    private static Text addHpText;
    private static Text regenHpText;
    private static Text magicArmorText;

    //Description Specializing
    private static Text criticalChanceText;
    private static Text miningSpeedText;
    private static Text addpieceHealthText;
    private static Text treeCuttingSpeedText;


    private static void InitLevelSystem()
    {
        levelSystemPanel = UI.transform.Find("Canvas/PointPanel").gameObject;
        levelSystemPanelRoot = UI.transform.Find("Canvas").gameObject;
        levelSystemPanelRoot.GetComponent<CanvasScaler>().scaleFactor = EpicMMOSystem.LevelHudGroupScale.Value;

        //levelSystemPanel.transform.Find("Background").gameObject.AddComponent<DragMenu>().menu = levelSystemPanel.transform;
        //levelSystemPanel.transform.Find("Header").gameObject.AddComponent<DragMenu>().menu = levelSystemPanel.transform;
        levelSystemPanel.transform.Find("Header/Text").GetComponent<Text>().text =localization["$attributes"];
        DragWindowCntrl.ApplyDragWindowCntrl(levelSystemPanel);

        //alertReset
        alertResetPointPanel = UI.transform.Find("Canvas/ApplyReset").gameObject;
        alertResetPointText = alertResetPointPanel.transform.Find("Text").GetComponent<Text>();
        alertResetPointPanel.transform.Find("Buttons/Yes").GetComponent<Button>().onClick.AddListener(ResetYes);
        alertResetPointPanel.transform.Find("Buttons/Yes/Text").GetComponent<Text>().text = localization["$yes"];
        alertResetPointPanel.transform.Find("Buttons/No").GetComponent<Button>().onClick.AddListener(ResetNo);
        alertResetPointPanel.transform.Find("Buttons/No/Text").GetComponent<Text>().text = localization["$no"];
        
        
        //Stats
        var strength = levelSystemPanel.transform.Find("Points/ListStat/Strength");
        parameterButtons.Add(new ParameterButton(strength, Parameter.Strength));
        var dexterity = levelSystemPanel.transform.Find("Points/ListStat/Dexterity");
        parameterButtons.Add(new ParameterButton(dexterity, Parameter.Agility));
        var intellect = levelSystemPanel.transform.Find("Points/ListStat/Intelect");
        parameterButtons.Add(new ParameterButton(intellect, Parameter.Intellect));
        var endurance = levelSystemPanel.transform.Find("Points/ListStat/Endurance");
        parameterButtons.Add(new ParameterButton(endurance, Parameter.Body));
        var vigour = levelSystemPanel.transform.Find("Points/ListStat/Vigour");
        parameterButtons.Add(new ParameterButton(vigour, Parameter.Vigour));
        var specializing = levelSystemPanel.transform.Find("Points/ListStat/Specializing");
        parameterButtons.Add(new ParameterButton(specializing, Parameter.Special));


        freePointsPanel = levelSystemPanel.transform.Find("Points/FreePoints").gameObject;
        freePointsText = levelSystemPanel.transform.Find("Points/FreePoints/Text").GetComponent<Text>();

        freePointsPanel.transform.Find("Cancel").GetComponent<Button>().onClick.AddListener(ClickCancel);
        freePointsPanel.transform.Find("Cancel/Text").GetComponent<Text>().text = localization["$cancel"];
        freePointsPanel.transform.Find("Apply").GetComponent<Button>().onClick.AddListener(ClickApply);
        freePointsPanel.transform.Find("Apply/Text").GetComponent<Text>().text = localization["$apply"];
        
        levelSystemPanel.transform.Find("Points/ResetButton/Button").GetComponent<Button>().onClick.AddListener(ResetParameters);
        levelSystemPanel.transform.Find("Points/ResetButton/Button/Text").GetComponent<Text>().text = localization["$reset_parameters"]; 
        
        //CurrentLevel
        currentLevelText = levelSystemPanel.transform.Find("CurrentLevel/Content/Lvl").GetComponent<Text>();
        expText = levelSystemPanel.transform.Find("CurrentLevel/Content/Exp").GetComponent<Text>();

        //Description Content 
        Transform contentStats = levelSystemPanel.transform.Find("DescriptionStats/Scroll View/Viewport/Content");
        contentStats.transform.Find("HeaderDamage/Text").GetComponent<Text>().text = localization["$damage"];
        contentStats.transform.Find("HeaderDefence/Text").GetComponent<Text>().text = localization["$armor"];
        contentStats.transform.Find("HeaderSurv/Text").GetComponent<Text>().text = localization["$survival"];
        contentStats.transform.Find("HeaderOther/Text").GetComponent<Text>().text = localization["$specialother"];


        //Description strength
        physicDamageText = contentStats.transform.Find("PhysicDamage").GetComponent<Text>();
        addWeightText = contentStats.transform.Find("Weight").GetComponent<Text>();
        reducedStaminaBlockText = contentStats.transform.Find("StaminaBlock").GetComponent<Text>();
        criticalDmgMultText = contentStats.transform.Find("CriticalDmg").GetComponent<Text>();
        //Description Dexterity
        attackSpeedMultiText = contentStats.transform.Find("SpeedAttack").GetComponent<Text>();
        attackStamina = contentStats.transform.Find("AttackStamina").GetComponent<Text>(); // rename and add section for this
        reducedStaminaText = contentStats.transform.Find("StaminaReduction").GetComponent<Text>();  
        //Description intellect
        magicDamageText = contentStats.transform.Find("MagicDamage").GetComponent<Text>();     
        magicEitrRegText = contentStats.transform.Find("EitrReg").GetComponent<Text>();
        eitrIncreaseText = contentStats.transform.Find("EitrIncr").GetComponent<Text>();
        //Description Endurance      
        physicArmorText = contentStats.transform.Find("DamageReduction").GetComponent<Text>();  
        staminaRegen = contentStats.transform.Find("StaminaRegeneration").GetComponent<Text>();
        addStaminaText = contentStats.transform.Find("Stamina").GetComponent<Text>();
        //Description Vigour
        addHpText = contentStats.transform.Find("Hp").GetComponent<Text>();
        regenHpText = contentStats.transform.Find("HpRegeneration").GetComponent<Text>();
        magicArmorText = contentStats.transform.Find("MagicDamageReduction").GetComponent<Text>();
        //Description Specializing
        criticalChanceText = contentStats.transform.Find("CriticalChance").GetComponent<Text>();
        miningSpeedText = contentStats.transform.Find("MiningSpeed").GetComponent <Text>();
        addpieceHealthText = contentStats.transform.Find("ConstructionPieceHealth").GetComponent<Text>();
        treeCuttingSpeedText = contentStats.transform.Find("TreeCutting").GetComponent<Text>() ;

    }

    private static void ClickCancel()
    {
        LevelSystem.Instance.cancelDepositPoints();
        levelSystemPanel.SetActive(false);
    }
    
    private static void ClickApply()
    {
        LevelSystem.Instance.applyDepositPoints();
        levelSystemPanel.SetActive(false);
    }

    private static void ResetParameters()
    {
        var textCoin = EpicMMOSystem.viewTextCoins.Value;
        var price = LevelSystem.Instance.getPriceResetPoints();
        var text = localization["$reset_point_text"];
        alertResetPointText.text = String.Format(text, price, textCoin);
        levelSystemPanel.GetComponent<CanvasGroup>().interactable = false;
        alertResetPointPanel.SetActive(true);
    }

    private static void ResetYes()
    {
        ResetNo();
        LevelSystem.Instance.ResetAllParameterPayment();
    }
    
    private static void ResetNo()
    {
        alertResetPointPanel.SetActive(false);
        levelSystemPanel.GetComponent<CanvasGroup>().interactable = true;
    }

    public static void UpdateParameterPanel()
    {
        var points = LevelSystem.Instance.getFreePoints();
        var hasDeposit = LevelSystem.Instance.hasDepositPoints();
        parameterButtons.ForEach(p => p.UpdateParameters(points));

        freePointsPanel.SetActive(points > 0 || hasDeposit);
        freePointsText.text = $"{localization["$free_points"]}: {points}";
        
        currentLevelText.text = $"{localization["$level"]}: {LevelSystem.Instance.getLevel()}";
        var currentExp = LevelSystem.Instance.getCurrentExp();
        var needExp = LevelSystem.Instance.getNeedExp();
        var locText = localization["$exp"];
        expText.text = $"{locText}: {currentExp} / {needExp}";


        #region parameter


        //Description strength
        physicDamageText.text = $"{localization["$physic_damage"]}: +{LevelSystem.Instance.getAddPhysicDamage()}%";
        addWeightText.text = $"{localization["$add_weight"]}: +{LevelSystem.Instance.getAddWeight()}";
        reducedStaminaBlockText.text = $"{localization["$reduced_stamina_block"]}: -{LevelSystem.Instance.getReducedStaminaBlock()}%";
        criticalDmgMultText.text = $"{localization["$crtcDmgMulti"]}: +{LevelSystem.Instance.getAddCriticalDmg()}%"; // new

        //Description Dexterity
        attackSpeedMultiText.text = $"{localization["$attack_speed"]}: +{LevelSystem.Instance.getAddAttackSpeed()}%"; // new
        attackStamina.text = $"{localization["$attack_stamina"]}: -{LevelSystem.Instance.getAttackStamina()}%"; // rename
        reducedStaminaText.text = $"{localization["$reduced_stamina"]}: -{LevelSystem.Instance.getStaminaReduction()}%";


        //Description intellect
        magicDamageText.text = $"{localization["$magic_damage"]}: +{LevelSystem.Instance.getAddMagicDamage()}%";    
        eitrIncreaseText.text = $"{localization["$add_eitr"]}: +{LevelSystem.Instance.getAddEitr()}";
        magicEitrRegText.text = $"{localization["$regen_eitr"]}: +{LevelSystem.Instance.getEitrRegen()}%";

        //Description Endurance
        physicArmorText.text = $"{localization["$physic_armor"]}: +{LevelSystem.Instance.getAddPhysicArmor()}%";    
        staminaRegen.text = $"{localization["$stamina_reg"]}: +{LevelSystem.Instance.getStaminaRegen()}%";
        addStaminaText.text = $"{localization["$add_stamina"]}: +{LevelSystem.Instance.getAddStamina()}";

        //Description Vigour
        addHpText.text = $"{localization["$add_hp"]}: +{LevelSystem.Instance.getAddHp()}";
        regenHpText.text = $"{localization["$regen_hp"]}: +{LevelSystem.Instance.getAddRegenHp()}%";
        magicArmorText.text = $"{localization["$magic_armor"]}: +{LevelSystem.Instance.getAddMagicArmor()}%";

        //Description Specializing
        criticalChanceText.text = $"{localization["$crit_chance"]}: +{LevelSystem.Instance.getAddCriticalChance()}%"; // new
        miningSpeedText.text = $"{localization["$mining_speed"]}: +{LevelSystem.Instance.getaddMiningDmg()}%"; // new
        addpieceHealthText.text = $"{localization["$piece_health"]}: +{LevelSystem.Instance.getAddPieceHealth()}"; // new
        treeCuttingSpeedText.text = $"{localization["$tree_cutting"]}: +{LevelSystem.Instance.getAddTreeCuttingDmg()}%"; //new

        #endregion
    }

    #region ParameterButton
    private class ParameterButton
    {
        private Parameter parameter;
        private Transform head;
        private GameObject buttons;
        private Text text;

        public ParameterButton(Transform head, Parameter parameter)
        {
            this.head = head;
            this.parameter = parameter;

            text = head.Find("Text").GetComponent<Text>();
            buttons = head.Find("Buttons").gameObject;
            buttons.transform.Find("Plus1").GetComponent<Button>().onClick.AddListener(ClickButton1);
            buttons.transform.Find("Plus5").GetComponent<Button>().onClick.AddListener(ClickButton5);
        }

        private void ClickButton1()
        {
            LevelSystem.Instance.addPointsParametr(parameter, 1);
            UpdateParameterPanel();
        }
        private void ClickButton5()
        {
            LevelSystem.Instance.addPointsParametr(parameter, 5);
            UpdateParameterPanel();
        }

        public void UpdateParameters(int freePoints)
        {

            int max = parameter.ToString() switch
            {
                "Strength" => EpicMMOSystem.maxValueStrength.Value,
                "Agility" => EpicMMOSystem.maxValueDexterity.Value,
                "Intellect" => EpicMMOSystem.maxValueIntelligence.Value,
                "Body" => EpicMMOSystem.maxValueEndurance.Value,
                "Vigour" => EpicMMOSystem.maxValueVigour.Value,
                "Special" => EpicMMOSystem.maxValueSpecializing.Value,
                _ => max = 205,
            };
            var current = LevelSystem.Instance.getParameter(parameter);
            text.text = $"{localization[$"$parameter_{parameter.ToString().ToLower()}"]}: {current}";
            buttons.SetActive(freePoints > 0 && current < max);
        }
    }
    #endregion


    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.Awake))]
    private static class InventoryGui_Awake_Patch
    {
        private static void Postfix(InventoryGui __instance)
        {
            foreach (UITooltip uiTooltip in levelSystemPanel.GetComponentsInChildren<UITooltip>(true))
            {
                uiTooltip.m_tooltipPrefab = __instance.m_playerGrid.m_elementPrefab.GetComponent<UITooltip>()
                    .m_tooltipPrefab;

                switch (uiTooltip.m_topic)
                { 
                    case "Strength":
                        uiTooltip.m_text = localization["$strength_tooltip"];
                        uiTooltip.m_topic = localization["$parameter_strength"];
                        break;
                    case "Dexterity":
                        uiTooltip.m_text = localization["$dexterity_tooltip"];
                        uiTooltip.m_topic = localization["$parameter_agility"];
                        break;
                    case "Intelect":
                        uiTooltip.m_text = localization["$intelect_tooltip"];
                        uiTooltip.m_topic = localization["$parameter_intellect"]; 
                        break;
                    case "Endurance":
                        uiTooltip.m_text = localization["$endurance_tooltip"];
                        uiTooltip.m_topic = localization["$parameter_body"]; 
                        break;
                    case "Vigour":
                        uiTooltip.m_text = localization["$vigour_tooltip"];
                        uiTooltip.m_topic = localization["$parameter_vigour"];
                        break;
                    case "Special":
                        uiTooltip.m_text = localization["$special_tooltip"];
                        uiTooltip.m_topic = localization["$parameter_special"];
                        break;

                }
            }
        }
    }
}