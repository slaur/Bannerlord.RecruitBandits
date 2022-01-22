using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace RecruitBandits.Patches
{
  public class RecruitBanditsPatches
  {
    [HarmonyPatch(typeof(MobileParty), "IsBandit", MethodType.Getter)]
    public static class MobilePartyIsBanditPatch
    {
      public static void Postfix(MobileParty __instance , ref bool __result)
      {
        if (__instance.LeaderHero == null || __instance.LeaderHero != Hero.MainHero) return;
        
        if (__instance.BanditPartyComponent != null)
          __result = true;
      }
    }
    
    [HarmonyPatch(typeof(Clan), "IsBanditFaction", MethodType.Getter)]
    public static class ClanIsBanditFaction
    {
      public static void Postfix(Clan __instance , ref bool __result)
      {
        if (__instance.Leader == null || __instance.Leader != Hero.MainHero) return;
        
        // var partyCulture = __instance.Leader?.Clan?.Culture;
        // if (partyCulture != null && partyCulture.IsBandit)
        //   __result = true;
        
        if (MobileParty.MainParty.BanditPartyComponent != null)
          __result = true;
      }
    }
  }
}