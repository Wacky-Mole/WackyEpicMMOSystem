using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace EpicMMOSystem.Gui
{
    public class Util
    {

        public static void FloatingText(string text, string text2=null)
        {
            float random = Random.Range(1.2f, 1.6f);
            float random2 = Random.Range(1.2f, 1.6f);

            DamageText.WorldTextInstance worldTextInstance = new DamageText.WorldTextInstance
            {
                m_worldPos = Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.right * random +
                             Vector3.up * random2,
                m_gui = UnityEngine.Object.Instantiate(DamageText.instance.m_worldTextBase, DamageText.instance.transform),
            };
            //random = +.5f;
            DamageText.WorldTextInstance worldTextInstance2 = new DamageText.WorldTextInstance
            {
                m_worldPos = Player.m_localPlayer.transform.position + (Player.m_localPlayer.transform.right * random)  +
                 Vector3.up * random2,
                m_gui = UnityEngine.Object.Instantiate(DamageText.instance.m_worldTextBase, DamageText.instance.transform),
            };
            worldTextInstance.m_gui.GetComponent<RectTransform>().sizeDelta *= 2;
            worldTextInstance.m_textField = worldTextInstance.m_gui.GetComponent<TMP_Text>();
            DamageText.instance.m_worldTexts.Add(worldTextInstance);
            worldTextInstance.m_textField.fontSize = 30;
            Color tempC = Color.yellow;
            if (ColorUtility.TryParseHtmlString(EpicMMOSystem.XPColor.Value, out tempC))
                worldTextInstance.m_textField.color = tempC;
            worldTextInstance.m_textField.text = text;
           // worldTextInstance.m_textField.font = new Font().
            worldTextInstance.m_timer = -1f;

            if (text2 != null)
            {
                worldTextInstance2.m_gui.GetComponent<RectTransform>().sizeDelta *= 2;
                worldTextInstance2.m_textField = worldTextInstance2.m_gui.GetComponent<TMP_Text>();
                worldTextInstance2.m_worldPos += new Vector3(0,5,0);
                DamageText.instance.m_worldTexts.Add(worldTextInstance2);
                worldTextInstance2.m_textField.fontSize = 20;
                worldTextInstance.m_textField.color = Color.blue;
                worldTextInstance2.m_textField.text = text2;
                worldTextInstance2.m_timer = -1f;

            }
            //$"<color=yellow>+ {exp}</color><color=#00FFFF> exp</color>Utils.FloatingText($"<color=yellow>+ {exp}</color><color=#00FFFF> exp</color>");
        }
    }
}
