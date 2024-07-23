using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace EpicMMOSystem.OtherApi;

public static class DonatShop_API
{
    private static API_State state = API_State.NotReady;
    private static MethodInfo openShop;
    private static MethodInfo hideShop;
    
    private enum API_State
    {
        NotReady, NotInstalled, Ready,
    }
    
    public static bool IsInstalled()
    {
        Init();
        return state is API_State.Ready;
    }

    public static void OpenShop()
    {
        Init();
        openShop?.Invoke(null, null);
    }
    
    public static void HidePanel()
    {
        Init();
        hideShop?.Invoke(null, null);
    }
 
    private static void Init()
    { 
        if (state is API_State.Ready or API_State.NotInstalled) return;
        if (Type.GetType("Shop.DonatShop, DonatShop") == null)
        {
            state = API_State.NotInstalled;
            return;
        }

        state = API_State.Ready;

        Type actionsMO = Type.GetType("API.Shop_API, DonatShop");
        openShop = actionsMO.GetMethod("OpenPanel", BindingFlags.Public | BindingFlags.Static);
        hideShop = actionsMO.GetMethod("HidePanel", BindingFlags.Public | BindingFlags.Static);
    }
}