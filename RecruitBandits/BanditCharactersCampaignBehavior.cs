using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;

namespace RecruitBandits
{
  // See UrbanCharactersCampaignBehavior for companions management
  public class BanditCharactersCampaignBehavior : CampaignBehaviorBase
  {
    private Dictionary<Settlement, int> _settlementPassedDaysForWeeklyTick;

    public BanditCharactersCampaignBehavior()
    {
      _settlementPassedDaysForWeeklyTick = new Dictionary<Settlement, int>();
    }

    public override void RegisterEvents()
    {
      CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, OnNewGameCreated);
      CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, OnGameLoaded);
      CampaignEvents.OnHideoutSpottedEvent.AddNonSerializedListener(this, OnHideoutSpotted);
      CampaignEvents.OnHideoutDeactivatedEvent.AddNonSerializedListener(this, OnHideoutDeactivated);
      CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, DailyTickSettlement);
    }

    public override void SyncData(IDataStore dataStore)
    {
      dataStore.SyncData("_settlementPassedDaysForWeeklyTick", ref _settlementPassedDaysForWeeklyTick);
    }

    private void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
    {
      SpawnBanditNotablesEverywhere();
    }

    private void OnGameLoaded(CampaignGameStarter campaignGameStarter)
    {
      SpawnBanditNotablesEverywhere();
    }

    private void OnHideoutSpotted(PartyBase pp, PartyBase pps)
    {
      SpawnBanditNotablesEverywhere();
    }

    private void OnHideoutDeactivated(Settlement hideout)
    {
      foreach (var notable in hideout.Notables.Where(n => n.Occupation == Occupation.Bandit))
      {
        KillCharacterAction.ApplyByRemove(notable);
      }
    }

    private void DailyTickSettlement(Settlement settlement)
    {
      if (!settlement.IsHideout) return;
      
      if (_settlementPassedDaysForWeeklyTick.ContainsKey(settlement))
      {
        _settlementPassedDaysForWeeklyTick[settlement]++;
        if (_settlementPassedDaysForWeeklyTick[settlement] != 7)
          return;
        SpawnBanditNotablesIfNeeded(settlement);
        _settlementPassedDaysForWeeklyTick[settlement] = 0;
      }
      else
      {
        _settlementPassedDaysForWeeklyTick.Add(settlement, 0);
      }
    }

    private void SpawnBanditNotablesEverywhere()
    {
      foreach (var settlement in Settlement.All.Where(s => s.IsHideout))
      {
        SpawnBanditNotablesIfNeeded(settlement);
      }
    }

    private void SpawnBanditNotablesIfNeeded(Settlement settlement)
    {
      if (!settlement.IsHideout) return;
      
      var countForSettlementLeaders = 3;
      if (settlement.Prosperity >= 3000.0)
        countForSettlementLeaders = settlement.Prosperity >= 6000.0 ? 5 : 4;
      var currentNotables = settlement.Notables.Where(n => n.Occupation == Occupation.GangLeader);
      var missingNotablesCount = countForSettlementLeaders - currentNotables.Count();

      for (var index = 0; index < missingNotablesCount; ++index)
      {
        var bandit = HeroCreator.CreateSpecialHero(settlement.Culture.BanditBoss, settlement, settlement.OwnerClan);
        bandit.SetNewOccupation(Occupation.GangLeader);
        System.Diagnostics.Debug.WriteLine(bandit.Name + " (" + bandit.Age + ") created at " + settlement.Name);
        EnterHideoutAction.Apply(bandit, settlement);
        if (settlement.Notables.Contains(bandit))
          System.Diagnostics.Debug.WriteLine(bandit.Name + " is a notable at " + settlement.Name);
      }
      
      // Set clan leader ?
      if (settlement.OwnerClan.Leader == null && !settlement.Notables.IsEmpty())
      {
        settlement.OwnerClan.SetLeader(settlement.Notables.First());
      }
    }
  }
}