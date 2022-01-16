using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace RecruitBandits
{
  public static class JoinBanditsAction
  {
    public static void ApplyForClan(Clan clan, Settlement settlement)
    {
      foreach (var notable in settlement.Notables)
      {
        ChangeRelationAction.ApplyPlayerRelation(notable, 100, false, false);
      }
    }
  }
}