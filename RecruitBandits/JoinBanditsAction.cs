using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace RecruitBandits
{
  public static class JoinBanditsAction
  {
    public static void ApplyForHero()
    {
      var settlement = Hero.MainHero.CurrentSettlement;
      if (settlement == null || !settlement.IsHideout) return;
      
      PartyTemplateObject partyTemplateObject = null;
      foreach (var banditFaction in Clan.BanditFactions)
      {
        if (settlement.Culture != banditFaction.Culture) continue;
        partyTemplateObject = banditFaction.DefaultPartyTemplate;
      }
      if (partyTemplateObject == null) return;

      var currentParty = MobileParty.MainParty;
      // Current party must be cleared before the main hero is affected to a new one
      currentParty.MemberRoster.Clear();
      // Replace clear + Destroy with Disband ?
      // DisbandPartyAction.ApplyDisband(currentParty);
      
      var banditParty = BanditPartyComponent.CreateBanditParty("Recruit_Bandits", Hero.MainHero.Clan, settlement.Hideout, false);
      banditParty.ActualClan = Clan.PlayerClan;
      banditParty.InitializeMobilePartyAtPosition(partyTemplateObject, settlement.Position2D);
      banditParty.SetCustomName(MobileParty.MainParty.Name);
      banditParty.MemberRoster.Clear();
      
      banditParty.AddElementToMemberRoster(Hero.MainHero.CharacterObject, 1);
      banditParty.ChangePartyLeader(Hero.MainHero);
      
      Campaign.Current.OnPlayerCharacterChanged();
      DestroyPartyAction.Apply(null, currentParty);

      // Hero.MainHero.Clan.Culture = settlement.Culture;
      // foreach (Clan banditFaction in Clan.BanditFactions)
      // {
      //   if (banditFaction.IsBanditFaction && !banditFaction.IsMinorFaction)
      //   {
      //     FactionManager.SetStanceTwoSided(Hero.MainHero.Clan, banditFaction, 30);
      //   }
      // }
    }
  }
}