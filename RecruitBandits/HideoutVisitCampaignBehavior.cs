using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;

namespace RecruitBandits
{
  public class HideoutVisitCampaignBehavior : CampaignBehaviorBase
  {
    public override void SyncData(IDataStore dataStore)
    {
    }

    public override void RegisterEvents()
    {
      CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, AddGameMenus);
    }

    protected void AddGameMenus(CampaignGameStarter campaignGameStarter)
    {
      campaignGameStarter.AddGameMenuOption("hideout_place", "recruit_volunteers", "{=E31IJyqs}Recruit troops", game_menu_recruit_volunteers_on_condition, game_menu_recruit_volunteers_on_consequence);
      campaignGameStarter.AddGameMenuOption("hideout_place", "join_bandits", "Join the bandits", game_menu_join_bandits_on_condition, game_menu_join_bandits_on_consequence);
    }

    private static bool game_menu_recruit_volunteers_on_condition(MenuCallbackArgs args)
    {
      args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
      return true;
    }
    
    private static void game_menu_recruit_volunteers_on_consequence(MenuCallbackArgs args)
    {
    }
        
    [GameMenuEventHandler("hideout_place", "recruit_volunteers", GameMenuEventHandler.EventType.OnConsequence)]
    private static void game_menu_ui_recruit_volunteers_on_consequence(MenuCallbackArgs args) => args.MenuContext.OpenRecruitVolunteers();
    
    private static bool game_menu_join_bandits_on_condition(MenuCallbackArgs args)
    {
      args.optionLeaveType = GameMenuOption.LeaveType.Mission;
      return true;
    }
    
    private static void game_menu_join_bandits_on_consequence(MenuCallbackArgs args)
    {
      JoinBanditsAction.ApplyForClan(Clan.PlayerClan, MobileParty.MainParty.LastVisitedSettlement);
    }
  }
}