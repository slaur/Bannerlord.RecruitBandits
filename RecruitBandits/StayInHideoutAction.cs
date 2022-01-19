using TaleWorlds.CampaignSystem;

namespace RecruitBandits
{
 public static class StayInHideoutAction
  {
    public static void Apply(Hero hero, Settlement settlement)
    {
      hero.StayingInSettlement = settlement;
    }
  }
}