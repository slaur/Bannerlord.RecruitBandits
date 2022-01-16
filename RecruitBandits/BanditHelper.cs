using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace RecruitBandits
{
  public class BanditHelper
  {
    public int GetRelationWithBandits(Hero hero, IFaction mapFaction)
    {
      if (hero.Clan.Kingdom != null || !hero.Clan.Settlements.IsEmpty()) return -100;
      
      var baseRelation = Math.Max(hero.Clan.Tier, 6) * 10;
      if (hero.GetTraitLevel(DefaultTraits.Honor) < 0) baseRelation += 10;
      return baseRelation;
    }
  }
}