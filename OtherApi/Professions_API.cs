using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace EpicMMOSystem.OtherApi;

public static class Professions_API
{
    private static API_State state = API_State.NotReady;
    private static GameObject mShowProfessions;
    private static MethodInfo mUpdateSelected;
    private static MethodInfo mRequestServerTime;
    private static bool firstOpen = true;


    private enum API_State
    {
        NotReady, NotInstalled, Ready,
    }

    
    public static bool IsInstalled()
    {
        Init();
        return state is API_State.Ready;
    }

    public static void showProfessions()
    {
        Init();
        if (state != API_State.Ready) return;
        if (!mShowProfessions)
        {
            forceInit();
        }
        mShowProfessions.SetActive(!mShowProfessions.activeSelf);
        if (mShowProfessions.activeSelf)
        {
            mUpdateSelected.Invoke(null, null);
            mRequestServerTime.Invoke(null, null);
            firstOpen = false;
        }
    }
 
    private static void Init()
    {
        if (!firstOpen)
        {
            if (state is API_State.Ready or API_State.NotInstalled) return;
        }
        if (Type.GetType("Professions.Professions, Professions") == null)
        {
            state = API_State.NotInstalled;
            return;
        }

        state = API_State.Ready;

        Type actionsMO = Type.GetType("Professions.Professions, Professions");
        mShowProfessions = AccessTools.Field(actionsMO, "professionPanelInstance").GetValue(null) as GameObject;
        mUpdateSelected = actionsMO.GetMethod("UpdateSelectPanelSelections",BindingFlags.NonPublic | BindingFlags.Static);
        mRequestServerTime = actionsMO.GetMethod("requestServerTime",BindingFlags.NonPublic | BindingFlags.Static);
    }

    private static void forceInit()
    {
        if (Type.GetType("Professions.Professions, Professions") == null)
        {
            state = API_State.NotInstalled;
            return;
        }

        state = API_State.Ready;

        Type actionsMO = Type.GetType("Professions.Professions, Professions");
        mShowProfessions = AccessTools.Field(actionsMO, "professionPanelInstance").GetValue(null) as GameObject;
        mUpdateSelected = actionsMO.GetMethod("UpdateSelectPanelSelections",BindingFlags.NonPublic | BindingFlags.Static);
        mRequestServerTime = actionsMO.GetMethod("requestServerTime",BindingFlags.NonPublic | BindingFlags.Static);
    }
}