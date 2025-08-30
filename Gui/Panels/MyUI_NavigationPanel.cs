using API;
using EpicMMOSystem.MonoScripts;
using EpicMMOSystem.OtherApi;
using UnityEngine;
using UnityEngine.UI;


namespace EpicMMOSystem;

public partial class MyUI
{
    internal static GameObject navigationPanel;
    private static Transform buttonLevelSystem;
    private static Transform buttonFriendsList;
    private static Transform buttonQuestPanel;
    private static Transform buttonProfessionsPanel;
    private static Transform ButtonGuildsPanel;
    private static void InitNavigationPanel()
    {
        navigationPanel = UI.transform.Find("Canvas/NavigatePanel").gameObject;
        buttonLevelSystem = navigationPanel.transform.Find("Buttons/ButtonLevelSystem");
        buttonLevelSystem.GetComponent<Button>().onClick.AddListener(ClickButtonLevelSystem);
        DragWindowCntrl.ApplyDragWindowCntrl(navigationPanel);
        //DragControl.Apply

        buttonFriendsList = navigationPanel.transform.Find("Buttons/ButtonFriends");
        buttonFriendsList.GetComponent<Button>().onClick.AddListener(ClickButtonFriendsList);

        if (Marketplace_API.IsInstalled())
        {
            buttonQuestPanel = navigationPanel.transform.Find("Buttons/ButtonQuests");
            buttonQuestPanel.GetComponent<Button>().onClick.AddListener(ClickQuestPanel);
            buttonQuestPanel.gameObject.SetActive(true);
        }

        if (Professions_API.IsInstalled())
        {
            buttonProfessionsPanel = navigationPanel.transform.Find("Buttons/ButtonProffesions");
            buttonProfessionsPanel.GetComponent<Button>().onClick.AddListener(ClickProfessionsPanel);
            buttonProfessionsPanel.gameObject.SetActive(true);
        }

        if (Guilds_API.IsInstalled())
        {
            ButtonGuildsPanel = navigationPanel.transform.Find("Buttons/ButtonGuilds");
            ButtonGuildsPanel.GetComponent<Button>().onClick.AddListener(ClickButtonButtonGuilds);
            ButtonGuildsPanel.gameObject.SetActive(true);
        }
    }

    private static void ClickButtonLevelSystem()
    {
        if (!levelSystemPanel.activeSelf) UpdateParameterPanel();
        levelSystemPanel.SetActive(!levelSystemPanel.activeSelf);
    }

    private static void ClickButtonFriendsList()
    {
        if (!friendsListPanel.activeSelf) updateList();
        friendsListPanel.SetActive(!friendsListPanel.activeSelf);
    }
    
    private static void ClickButtonButtonGuilds()
    {
        Guilds_API.ShowGuilds();
    }
    
    private static void ClickQuestPanel()
    {
        Marketplace_API.OpenQuestJournal();
    }
    
    private static void ClickProfessionsPanel()
    {
        Professions_API.showProfessions();
    }

    private static void ShowNavigationPanel()
    {
        var point = LevelSystem.Instance.getFreePoints();
        buttonLevelSystem.GetChild(0).gameObject.SetActive(point > 0);
        buttonFriendsList.GetChild(0).gameObject.SetActive(hasInvite());
    }
    
    
}