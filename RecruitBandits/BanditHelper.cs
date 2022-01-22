using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace RecruitBandits
{
  public static class BanditHelper
  {
    public static int GetRelationWithBandits(Hero hero, IFaction mapFaction)
    {
      if (hero.Clan.Kingdom != null || !hero.Clan.Settlements.IsEmpty()) return -100;
      
      var relation = Math.Max(hero.Clan.Tier, 6) * 10;
      var honorLevel = hero.GetTraitLevel(DefaultTraits.Honor);
      relation += -honorLevel * 10;
      return relation;
    }
    
    public static void SpawnHideoutNotables(Settlement settlement)
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
        bandit.StringId += "Recruit_Bandits";
        bandit.CharacterObject.StringId += "Recruit_Bandits";
        bandit.SetNewOccupation(Occupation.GangLeader);
        System.Diagnostics.Debug.WriteLine(bandit.Name + " (" + bandit.Age + ") created at " + settlement.Name);
        StayInHideoutAction.Apply(bandit, settlement);
        if (settlement.Notables.Contains(bandit))
          System.Diagnostics.Debug.WriteLine(bandit.Name + " is a notable at " + settlement.Name);
        
        // Set clan leader
        if (settlement.OwnerClan.Leader == null && !settlement.Notables.IsEmpty())
        {
          settlement.OwnerClan.SetLeader(settlement.Notables.First());
        }
      }
    }
    
    public static void RemoveHideoutNotables(Settlement settlement)
    {
      if (!settlement.IsHideout) return;

      var toRemove = settlement.Notables.Where(n => n.Occupation == Occupation.GangLeader).ToList();
      foreach (var notable in toRemove)
      {
        QuitHideoutAction.Apply(notable, settlement);
      }

      settlement.OwnerClan.SetLeader(null);
    }

    public static void SetHideoutNotablesRelations(Settlement settlement)
    {
      if (!settlement.IsHideout) return;

      var factionRelationWithPlayer = GetRelationWithBandits(Hero.MainHero, settlement.Owner.Clan);

      foreach (var notable in settlement.Notables)
      {
        notable.SetPersonalRelation(Hero.MainHero, factionRelationWithPlayer);
      }
    }

    public static void UpdateHideoutVolunteers(Settlement settlement)
    {
      if (!settlement.IsHideout) return;

      //System.Diagnostics.Debug.WriteLine("Volunteers at " + settlement.Name);
      foreach (var notable in settlement.Notables)
      {
        if (!notable.CanHaveRecruits) continue;
        var flag = false;
        var basicVolunteer = Campaign.Current.Models.VolunteerProductionModel.GetBasicVolunteer(notable);
        for (var index = 0; index < 6; ++index)
          if ((double) MBRandom.RandomFloat < 0.75f)
          {
            var volunteerType = notable.VolunteerTypes[index];
            if (volunteerType == null)
            {
              //System.Diagnostics.Debug.WriteLine("Volunteer type NULL");
              notable.VolunteerTypes[index] = basicVolunteer;
              //System.Diagnostics.Debug.WriteLine("Volunteer = " + basicVolunteer.Name);
              flag = true;
            }
            else
            {
              //System.Diagnostics.Debug.WriteLine("Volunteer type = " + volunteerType.Name);
              var num = (float) (40000.0 / (MathF.Max(50f, notable.Power) *
                                            (double) MathF.Max(50f, notable.Power)));
              if (MBRandom.RandomInt((int) MathF.Max(2f, volunteerType.Tier * num)) == 0 &&
                  volunteerType.UpgradeTargets.Length != 0 && volunteerType.Tier <= 3)
              {
                notable.VolunteerTypes[index] =
                  volunteerType.UpgradeTargets[
                    MBRandom.RandomInt(volunteerType.UpgradeTargets.Length)];
                flag = true;
              }
            }
          }

        if (flag)
        {
          var volunteerTypes = notable.VolunteerTypes;
          for (var index1 = 1; index1 < 6; ++index1)
          {
            var characterObject1 = volunteerTypes[index1];
            if (characterObject1 != null)
            {
              //
              //System.Diagnostics.Debug.WriteLine("characterObject1 = " + characterObject1.Name);
              var num = 0;
              var index2 = index1 - 1;
              var characterObject2 = volunteerTypes[index2];
              while (index2 >= 0 && (characterObject2 == null ||
                                     characterObject1.Level +
                                     (characterObject1.IsMounted ? 0.5 : 0.0) <
                                     characterObject2.Level +
                                     (characterObject2.IsMounted ? 0.5 : 0.0)))
                if (characterObject2 == null)
                {
                  --index2;
                  ++num;
                  if (index2 >= 0)
                    characterObject2 = volunteerTypes[index2];
                }
                else
                {
                  volunteerTypes[index2 + 1 + num] = characterObject2;
                  --index2;
                  num = 0;
                  if (index2 >= 0)
                    characterObject2 = volunteerTypes[index2];
                }

              volunteerTypes[index2 + 1 + num] = characterObject1;
            }
          }
        }
      }
    }
  }
}