using TaleWorlds.CampaignSystem;

namespace RecruitBandits
{
 public static class QuitHideoutAction
  {
    public static void Apply(Hero hero, Settlement settlement)
    {
      hero.StayingInSettlement = null;
    }
  }
}