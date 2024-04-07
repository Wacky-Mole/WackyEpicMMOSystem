using System;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace EpicMMOSystem;

public static class PlayerFVX
{

    public static void levelUp()
    {
        Transform parent = Player.m_localPlayer.transform.Find("Visual/Armature/Hips/Spine/Spine1/Spine2");
        Transform vfx = null;
        if (!EpicMMOSystem.altLevelUpSound.Value)
             vfx = EpicMMOSystem.Instantiate(ZNetScene.instance.GetPrefab("LevelUpVFX"), parent).transform;
        else
            vfx = EpicMMOSystem.Instantiate(ZNetScene.instance.GetPrefab("LevelUpVFX2"), parent).transform;

        vfx.localPosition = Vector3.zero;
        vfx.localScale = new Vector3(0.01352632f, 0.01352632f, 0.01352632f);
    }
}

public class CritDmgVFX
{

    public void CriticalVFX(Vector3 position, float damage)
    {
        string text = "Crit "+ Math.Round(damage);
        float random = UnityEngine.Random.Range(1.2f, 1.6f);
        float random2 = UnityEngine.Random.Range(1.2f, 1.6f);

        DamageText.WorldTextInstance worldTextInstance = new DamageText.WorldTextInstance
        {
            m_worldPos = position,
            m_gui = UnityEngine.Object.Instantiate(DamageText.instance.m_worldTextBase, DamageText.instance.transform)
        };

        worldTextInstance.m_gui.GetComponent<RectTransform>().sizeDelta *= 2;
        worldTextInstance.m_textField = worldTextInstance.m_gui.GetComponent<TMP_Text>();
        DamageText.instance.m_worldTexts.Add(worldTextInstance);
        worldTextInstance.m_textField.fontSize = 30;
        Color tempC = Color.cyan;
        worldTextInstance.m_textField.color = tempC;
        worldTextInstance.m_textField.text = text;
        // worldTextInstance.m_textField.font = new Font().
        worldTextInstance.m_timer = -1f;

        //var hitfx = GameObject.Find("fx_seeker_hurt");
        var hitfx = EpicMMOSystem.fx_seeker_crit;
       GameObject obj = UnityEngine.Object.Instantiate(hitfx, position, Quaternion.identity);
       UnityEngine.Object.Destroy(obj, 5f);

    }
}
