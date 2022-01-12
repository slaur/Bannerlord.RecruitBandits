using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace RecruitBandits
{
  public class BanditRecruitmentCampaignBehavior : CampaignBehaviorBase
  {
    public override void SyncData(IDataStore dataStore)
    {
    }

    public override void RegisterEvents()
    {
      CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, DailyTickHideout);
    }

    private void DailyTickHideout(Settlement settlement)
    {
      UpdateVolunteersOfNotablesInHideout(settlement);
    }

    public void UpdateVolunteersOfNotablesInHideout(Settlement settlement)
    {
      if (!settlement.IsHideout) return;

      System.Diagnostics.Debug.WriteLine("Volunteers at " + settlement.Name);
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
              System.Diagnostics.Debug.WriteLine("Volunteer type NULL");
              notable.VolunteerTypes[index] = basicVolunteer;
              System.Diagnostics.Debug.WriteLine("Volunteer = " + basicVolunteer.Name);
              flag = true;
            }
            else
            {
              System.Diagnostics.Debug.WriteLine("Volunteer type = " + volunteerType.Name);
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
              System.Diagnostics.Debug.WriteLine("characterObject1 = " + characterObject1.Name);
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