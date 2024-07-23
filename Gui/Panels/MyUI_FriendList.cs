using System;
using System.Collections.Generic;
using System.Linq;
using Groups;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Object = UnityEngine.Object;

namespace EpicMMOSystem;

public partial class MyUI
{
    private static List<Sprite> sprites = new();
    private static FriendsData friendsData = new();
    private static GameObject friendsListPanel;
    private static GameObject friendsCell;
    
    private static GameObject headerInvited;
    private static GameObject contentInvited;
    private static GameObject headerFriends;
    private static GameObject contentFriends;

    private static GameObject addFriendAlert;
    private static InputField textField;
    
    private static List<FriendsCell> friendsCells = new ();
    private static List<FriendsCell> inviteCells = new ();
    private static Dictionary<FriendInfo, ZNet.PlayerInfo> inviteList = new ();

    private static void InitFriendsList()
    {
        friendsListPanel = UI.transform.Find("Canvas/FriendList").gameObject;
        friendsListPanel.transform.Find("Background").gameObject.AddComponent<DragMenu>().menu =
            friendsListPanel.transform;
        friendsListPanel.transform.Find("Header").gameObject.AddComponent<DragMenu>().menu =
            friendsListPanel.transform;
        friendsListPanel.transform.Find("Header/Text").GetComponent<Text>().text = localization["$friends_list"];
        friendsCell = EpicMMOSystem._asset.LoadAsset<GameObject>("FriendCell");
        friendsListPanel.transform.Find("Border").GetComponent<Image>().raycastTarget = false;
        friendsListPanel.transform.Find("Add").GetComponent<Button>().onClick.AddListener(clickButtonAdd);

        headerInvited = friendsListPanel.transform.Find("Scroll View/Viewport/Content/HeaderInvited").gameObject;
        headerInvited.SetActive(false);
        contentInvited = friendsListPanel.transform.Find("Scroll View/Viewport/Content/Invited").gameObject;
        headerFriends = friendsListPanel.transform.Find("Scroll View/Viewport/Content/HeaderFriends").gameObject;
        contentFriends = friendsListPanel.transform.Find("Scroll View/Viewport/Content/Friends").gameObject;

        headerInvited.transform.Find("Text").GetComponent<Text>().text = localization["$invited"];
        headerFriends.transform.Find("Text").GetComponent<Text>().text = localization["$friends"];
        
        
        addFriendAlert = UI.transform.Find("Canvas/SendInvite").gameObject;
        textField = addFriendAlert.transform.Find("InputField").GetComponent<InputField>();
        textField.transform.Find("Placeholder").GetComponent<Text>().text = ""; // for now
        addFriendAlert.transform.Find("Buttons/Send").GetComponent<Button>().onClick.AddListener(clickButtonSend);
        addFriendAlert.transform.Find("Buttons/Send/Text").GetComponent<Text>().text = localization["$send"];
        addFriendAlert.transform.Find("Buttons/Cancel").GetComponent<Button>().onClick.AddListener(clickButtonCancel);
        addFriendAlert.transform.Find("Buttons/Cancel/Text").GetComponent<Text>().text = localization["$cancel"];
        
        sprites.Add(EpicMMOSystem._asset.LoadAsset<Sprite>("manIcon"));
        for (int i = 1; i < 15; i++)
        {
            sprites.Add(EpicMMOSystem._asset.LoadAsset<Sprite>($"Class{i}"));
        }
    }

    private static void updateList()
    {
        updateFriendsList();
        updateInviteList();
    }
    private static void updateFriendsList()
    {
        if (friendsCells.Count > 0)
        {
            friendsCells.ForEach(f => f.Destroy());
            friendsCells.Clear();
        }
        
        Dictionary<string, ZNet.PlayerInfo> playersInfo = new ();
        foreach (var player in ZNet.instance.GetPlayerList())
        {
            playersInfo.Add(player.m_name, player);
        }
        var friends = new Dictionary<string, FriendInfo>(friendsData.getFriends());
        headerFriends.SetActive(friends.Count > 0);
        List<FriendInfo> offline = new();
        foreach (var friend in friends)
        {
            if (playersInfo.ContainsKey(friend.Key))
            {
                var playerOnly = playersInfo[friend.Key];
                var cell = Object.Instantiate(friendsCell, contentFriends.transform);
                if (cell)
                {
                    var fCell = new FriendsCell(cell, friend.Value, playerOnly, FriendsCell.StatusFriend.online);
                    friendsCells.Add(fCell);
                }
            }
            else
            {
                offline.Add(friend.Value);
            }
        }
        
        foreach (var friend in offline)
        {
            var cell = Object.Instantiate(friendsCell, contentFriends.transform);
            if (cell)
            {
                var fCell = new FriendsCell(cell, friend, new ZNet.PlayerInfo(), FriendsCell.StatusFriend.offline);
                friendsCells.Add(fCell);
            }
        }
    }

    private static void updateInviteList()
    {
        if (inviteCells.Count > 0)
        {
            inviteCells.ForEach(f => f.Destroy());
            inviteCells.Clear();
        }
        headerInvited.SetActive(inviteList.Count > 0);
        foreach (var friend in inviteList)
        {
            var cell = Object.Instantiate(friendsCell, contentInvited.transform);
            if (cell)
            {
                var fCell = new FriendsCell(cell, friend.Key, friend.Value, FriendsCell.StatusFriend.invite);
                inviteCells.Add(fCell);
            }
        }
    }

    public static void addInviteFriend(FriendInfo info, ZNet.PlayerInfo playerInfo)
    {
        foreach (var keyValuePair in inviteList)
        {
            if (keyValuePair.Key.name == info.name) return;
        }
        inviteList.Add(info, playerInfo);
        updateInviteList();
    }

    private static bool hasInvite()
    {
        return inviteList.Count > 0;
    }

    public static void acceptInvited(FriendInfo friendInfo)
    {
        friendsData.addFriends(friendInfo);
        if (friendsListPanel.activeSelf)
        {
            updateList();
        }
    }

    private static void clickButtonAdd()
    {
        friendsListPanel.GetComponent<CanvasGroup>().interactable = false;
        textField.SetTextWithoutNotify("");
        addFriendAlert.SetActive(true);
    }
    
    private static void clickButtonSend()
    {
        string name = textField.text ?? "";
        foreach (var keyValuePair in inviteList)
        {
            if (keyValuePair.Key.name == name)
            {
                return;
            }
        }
        if (name == Player.m_localPlayer.GetPlayerName() 
            || friendsData.getFriends().ContainsKey(name)) 
        {
            return;
        }
        FriendsSystem.inviteFriend(name);
        friendsListPanel.GetComponent<CanvasGroup>().interactable = true;
        addFriendAlert.SetActive(false);
    }
    private static void clickButtonCancel()
    {
        friendsListPanel.GetComponent<CanvasGroup>().interactable = true;
        addFriendAlert.SetActive(false);
    }
    
    //Cell
    private class FriendsCell
    {
        public enum StatusFriend
        {
            online, offline, invite,
        } 
        private GameObject cell;
        private FriendInfo friend;
        private Text nameText;
        private Text levelText;
        private Image icon;
        private ZNet.PlayerInfo player;
        private StatusFriend status;
        

        public FriendsCell(GameObject cell, FriendInfo friend, ZNet.PlayerInfo info, StatusFriend status)
        {
            this.cell = cell;
            this.friend = friend;
            player = info;
            nameText = cell.transform.Find("Name").GetComponent<Text>();
            levelText = cell.transform.Find("Level").GetComponent<Text>();
            icon = cell.transform.Find("IconClass").GetComponent<Image>();
            nameText.text = friend.name;
            this.status = status;
            int level = friend.level;
            int moClass = friend.moClass;
            switch (status)
            {
                case StatusFriend.online:
                    try
                    {
                        var zdo = ZDOMan.instance.GetZDO(info.m_characterID);
                        level = zdo.GetInt($"{EpicMMOSystem.ModName}_level", 1);
                        moClass = zdo.GetInt("MagicOverhaulClass", 0);
                    }
                    catch (Exception e)
                    {
                        EpicMMOSystem.print($"Ошибка в получении значений из здо: {e.Message}");
                    }
                    cell.transform.Find("Status").GetComponent<Text>().text = localization["$online"];
                    cell.transform.Find("Status").GetComponent<Text>().color = Color.green;
                    cell.transform.Find("Buttons/Accept").gameObject.SetActive(false);
                    if (!Groups.API.IsLoaded())
                    {
                        cell.transform.Find("Buttons/AddGroup").gameObject.SetActive(false);
                    }
                    else
                    {
                        if (Groups.API.GroupPlayers().Count > 0
                            && (Groups.API.GetLeader()?.name ?? "") != Player.m_localPlayer.GetPlayerName())
                        {
                            cell.transform.Find("Buttons/AddGroup").gameObject.SetActive(false);
                        }
                    }
                    break;
                case StatusFriend.offline:
                    cell.transform.Find("Buttons/AddGroup").gameObject.SetActive(false);
                    cell.transform.Find("Buttons/Accept").gameObject.SetActive(false);
                    cell.transform.Find("Status").GetComponent<Text>().text = localization["$offline"];
                    cell.transform.Find("Status").GetComponent<Text>().color = Color.red;
                    break;
                case StatusFriend.invite:
                    cell.transform.Find("Buttons/AddGroup").gameObject.SetActive(false);
                    cell.transform.Find("Status").GetComponent<Text>().text = "";
                    break;
            }
            levelText.text = $"{localization["$level"]}: {level}";
            if (moClass != 0)
            {
                icon.sprite = sprites[moClass];
            }
            cell.transform.Find("Buttons/AddGroup").GetComponent<Button>().onClick.AddListener(inviteGroup);
            cell.transform.Find("Buttons/Accept").GetComponent<Button>().onClick.AddListener(acceptInvite);
            cell.transform.Find("Buttons/Delete").GetComponent<Button>().onClick.AddListener(deleteFriend);

            if (level > friend.level || moClass != friend.moClass)
            {
                friend.level = level;
                friend.moClass = moClass;
                friendsData.updateFriends(friend);
            }
        }

        private void inviteGroup()
        {
            if (Groups.API.GroupPlayers().Count > 0)
            {
                foreach (var playerReference in Groups.API.GroupPlayers())
                {
                    if (playerReference.name == friend.name) return;
                }
                var selfName = Player.m_localPlayer.GetPlayerName();
                if ((Groups.API.GetLeader()?.name ?? "") == selfName)
                {
                    ZRoutedRpc.instance.InvokeRoutedRPC(player.m_characterID.UserID, "Groups InvitePlayer", selfName);
                }
            }
            else
            {
                Groups.API.CreateNewGroup();
                ZRoutedRpc.instance.InvokeRoutedRPC(player.m_characterID.UserID, "Groups InvitePlayer", Player.m_localPlayer.GetPlayerName());
            }
            
            
        }

        private void acceptInvite()
        {
            FriendsSystem.acceptInvite(friend, player);
            inviteList.Remove(friend);
            acceptInvited(friend);
            Destroy();
        }

        private void deleteFriend()
        {
            if (status == StatusFriend.invite)
            {
                inviteList.Remove(friend);
                FriendsSystem.rejectInvite(friend, player);
                updateInviteList();
            }
            else
            {
                friendsData.deleteFriend(friend);
                updateFriendsList();
            }
        }

    
        public void Destroy()
        {
            Object.Destroy(cell);
        }
    }
    
    [HarmonyPatch(typeof(Game), nameof(Game.Logout))]
    public static class DataFriendsClear
    {
        public static void Prefix()
        {
            inviteList.Clear();
            friendsData.ClearFriend();
        }
    }
    
    
}