using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace RecruitBandits
{
  // See UrbanCharactersCampaignBehavior for companions management
  public class BanditCharactersCampaignBehavior : CampaignBehaviorBase
  {
    // todo : use this to compute available recruits
    private Dictionary<Settlement, float> _settlementLastVisit;

    public BanditCharactersCampaignBehavior()
    {
      _settlementLastVisit = new Dictionary<Settlement, float>();
    }

    public override void RegisterEvents()
    {
      
      //CampaignEvents.SettlementEntered.AddNonSerializedListener(this, OnSettlementEntered);
      //CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, OnSettlementLeft);
    }

    public override void SyncData(IDataStore dataStore)
    {
    }

    // private void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
    // {
    //   if (settlement == null || !settlement.IsHideout) return;
    //   if (mobileParty?.LeaderHero == null || mobileParty.LeaderHero != Hero.MainHero) return;
    //   
    //   BanditHelper.SpawnHideoutNotables(settlement);
    //   BanditHelper.SetNotablesRelations(settlement);
    //   BanditHelper.UpdateVolunteers(settlement);
    // }
    //
    // private void OnSettlementLeft(MobileParty mobileParty, Settlement settlement)
    // {
    //   if (settlement == null || !settlement.IsHideout) return;
    //   if (mobileParty?.LeaderHero == null || mobileParty.LeaderHero != Hero.MainHero) return;
    //
    //   BanditHelper.RemoveHideoutNotables(settlement);
    //   _settlementLastVisit[settlement] = Campaign.CurrentTime;
    // }
  }
}