using System.Diagnostics;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;

namespace RecruitBandits
{
  // See TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.CharacterRelationCampaignBehavior
  // See TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.CrimeCampaignBehavior
  // See TaleWorlds.CampaignSystem.SandBox.GameComponents.Map.DefaultCrimeModel
  // See TaleWorlds.CampaignSystem.SandBox.GameComponents.Map.DefaultSettlementSecurityModel
  // See TaleWorlds.CampaignSystem.Actions.ChangeCrimeRatingAction
  public class BanditRelationCampaignBehavior : CampaignBehaviorBase
  {
    public override void RegisterEvents()
    {
      CampaignEvents.ForceSuppliesCompletedEvent.AddNonSerializedListener(this, OnSettlementHostileAction);
      CampaignEvents.ForceVolunteersCompletedEvent.AddNonSerializedListener(this, OnSettlementHostileAction);
      CampaignEvents.RaidCompletedEvent.AddNonSerializedListener(this, OnSettlementHostileAction);
      CampaignEvents.CharacterDefeated.AddNonSerializedListener(this, OnCharacterDefeated);
      CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, OnHeroPrisonerTaken);
      CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, OnHeroKilled);
      CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, OnClanDestroyed);
    }

    public override void SyncData(IDataStore dataStore)
    {
    }

    private void OnSettlementHostileAction(BattleSideEnum winnerSide, MapEvent mapEvent)
    {
      // var attackerLeaderParty = mapEvent.AttackerSide.LeaderParty;
      // var attackerLeaderHero = attackerLeaderParty?.LeaderHero;
      // var defenderLeaderParty = mapEvent.DefenderSide.LeaderParty;
      // if (attackerLeaderParty == null || attackerLeaderParty.MapFaction == defenderLeaderParty.Settlement.MapFaction ||
      //     winnerSide != BattleSideEnum.Attacker || attackerLeaderHero == null || defenderLeaderParty == null ||
      //     !defenderLeaderParty.IsSettlement || !defenderLeaderParty.Settlement.IsVillage ||
      //     defenderLeaderParty.Settlement.OwnerClan == Clan.PlayerClan)
      //   return;
      // int relationChange1 = -MathF.Ceiling(6f * mapEvent.RaidDamage);
      // int relationChange2 = -MathF.Ceiling((float) (6.0 * (double) mapEvent.RaidDamage * 0.5));
      // if (relationChange1 < 0)
      //   ChangeRelationAction.ApplyRelationChangeBetweenHeroes(attackerLeaderHero, defenderLeaderParty.Settlement.OwnerClan.Leader,
      //     relationChange1);
      // if (relationChange2 >= 0)
      //   return;
      // foreach (Hero notable in leaderParty2.Settlement.Notables)
      //   ChangeRelationAction.ApplyRelationChangeBetweenHeroes(attackerLeaderHero, notable, relationChange2);

      // foreach (Settlement settlement in Settlement.All.Where<Settlement>((Func<Settlement, bool>) (t => t.IsTown && (double) t.GatePosition.DistanceSquared(hideout.GatePosition) < (double) model.HideoutClearedSecurityEffectRadius * (double) model.HideoutClearedSecurityEffectRadius)).ToList<Settlement>())
      //   settlement.Town.Security += (float) model.HideoutClearedSecurityGain;
    }

    private void OnCharacterDefeated(Hero victim, Hero winner)
    {
      addRelationWithHideoutsFromEncounter(winner, victim.MapFaction, 5);
    }

    // Criminal hero in prison : small reputation / relation boost with local bandits / other criminals
    private void OnHeroPrisonerTaken(PartyBase partyBase, Hero prisoner)
    {
      addRelationWithHideoutsFromEncounter(partyBase.LeaderHero, prisoner.MapFaction, 10);
      addRelationWithHideoutsFromEncounter(prisoner, partyBase.MapFaction, 10);
    }

    private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail actionDetail,
      bool showNotification)
    {
      if (victim == null || killer == null || actionDetail != KillCharacterAction.KillCharacterActionDetail.Executed)
        return;

      addRelationWithHideoutsFromEncounter(killer, victim.MapFaction, 20);
    }

    private void OnClanDestroyed(Clan obj)
    {
      // todo : implement
    }

    // Add a criminal reputation bonus
    // For each lord, keep a track of the relation with this hideout
    // Add a bonus for each settlement that has a negative relation with this lord
    private void addRelationWithHideoutsFromEncounter(Hero hero, IFaction mapFaction, int relationChange)
    {
      if (hero == null || mapFaction == null || !hero.Clan.IsOutlaw) return;

      // todo : all heroes
      if (!hero.IsHumanPlayerCharacter) return;

      foreach (var settlement in Settlement.All.Where(t => t.IsHideout))
      {
        foreach (var notable in settlement.Notables)
        {
          ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero, notable, relationChange, false);
        }
      }
    }
  }
}