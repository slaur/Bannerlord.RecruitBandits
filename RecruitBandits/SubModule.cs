using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace RecruitBandits
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (!(game.GameType is Campaign))
            {
                return;
            }

            ((CampaignGameStarter) gameStarterObject).AddBehavior(new BanditCharactersCampaignBehavior());
            ((CampaignGameStarter) gameStarterObject).AddBehavior(new BanditRecruitmentCampaignBehavior());
            ((CampaignGameStarter) gameStarterObject).AddBehavior(new HideoutVisitCampaignBehavior());
        }
    }
}