﻿
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity;

namespace EnhancementMonkey.Api.Enhancements.Ability
{
    internal class TurboCharge : ModEnhancement
    {
        public override string Icon => VanillaSprites.TurbochargeUpgradeIcon;

        public override string Description => "Gives the Turbocharge Ability";

        public override int BaseCost => 5500;

        public override bool LockAfterBuy => true;

        public override EnhancementLevel EnhancementLevel => EnhancementLevel.Basic;

        public override ModifyType Modifies => ModifyType.Tower;

        public override EnhancementType EnhancementGroup => EnhancementType.Ability;

        public override void ModifyTower(Il2CppAssets.Scripts.Simulation.Towers.Tower tower)
        {
            var towerModel = tower.rootModel.Duplicate().Cast<TowerModel>();

            towerModel.AddBehavior(Game.instance.model.GetTowerFromId("BoomerangMonkey-040").GetAbility().Duplicate());

            tower.UpdateRootModel(towerModel);
            tower.UpdatedModel(towerModel);
        }
    }
}
