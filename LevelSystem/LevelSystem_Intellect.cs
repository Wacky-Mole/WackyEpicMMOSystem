using HarmonyLib;

namespace EpicMMOSystem;

public partial class LevelSystem
{
    public float getAddMagicDamage(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Intellect) + pointpending;
        var multiplayer = EpicMMOSystem.magicDamage.Value;
        return parameter * multiplayer;
    }
    
    public float getEitrRegen(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Intellect) + pointpending;
        var multiplayer = EpicMMOSystem.magicEitrRegen.Value;
        return parameter * multiplayer;
    }

    public float getAddEitr(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Intellect) + pointpending;
        var multiplayer = EpicMMOSystem.addEitr.Value;
        return parameter * multiplayer;
    }

    [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
    public static class AddEitrFood_Path
    {
        public static void Postfix(ref float eitr)
        {
            if (eitr > 2 || EpicMMOSystem.addDefaultEitr.Value > 0f)
            {
                var addeitr = Instance.getAddEitr();
                eitr += addeitr + EpicMMOSystem.addDefaultEitr.Value;
            }
        }
    }

    [HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetDamage), new[] { typeof(int), typeof(float) })]
    public class AddDamageIntellect_Path
    {
        public static void Postfix(ref ItemDrop.ItemData __instance, ref HitData.DamageTypes __result)
        {
            if (Player.m_localPlayer == null) return;
            if (!Player.m_localPlayer.m_inventory.ContainsItem(__instance)) return;
            

            float add = Instance.getAddMagicDamage();
            var value = add / 100 + 1;

            __result.m_fire *= value;
            __result.m_frost *= value;
            __result.m_lightning *= value;
            __result.m_poison *= value;
            __result.m_spirit *= value;
        }
    }

    [HarmonyPatch(typeof(SEMan), nameof(SEMan.ModifyEitrRegen))]
    public static class RegenEitr_Patch
    {
        public static void Postfix(SEMan __instance, ref float eitrMultiplier)
        {
            if (__instance.m_character.IsPlayer())
            {
                float add = Instance.getEitrRegen();
                eitrMultiplier += add / 100;
            }
        }
    }
}