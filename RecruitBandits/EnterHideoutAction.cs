using TaleWorlds.CampaignSystem;

namespace RecruitBandits
{
 public static class EnterHideoutAction
  {
    public static void Apply(Hero hero, Settlement settlement)
    {
      hero.StayingInSettlement = settlement;
    }
  }
}