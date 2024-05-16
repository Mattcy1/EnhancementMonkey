using BTD_Mod_Helper.Api.ModOptions;
using EnhancementMonkey;
using EnhancementMonkey.Api.Ui.Submenues;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Simulation.Objects;
using MelonLoader;
using System.Collections.Generic;
using System.Text;

[assembly: MelonInfo(typeof(EnhancementMonkey.EnhancementMonkey), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
[assembly: MelonOptionalDependencies("CardMonkey")]

namespace EnhancementMonkey;

public class EnhancementMonkey : BloonsTD6Mod
{
    public static bool MIB = false;

    public static void Debug(object message, LogLevel level)
    {
        if (DebugMode & level <= DebugLevel)
        {
            if (level == LogLevel.Info)
            {
                Log<EnhancementMonkey>(message);
            }
            else if (level == LogLevel.Warn | level == LogLevel.Dependency)
            {
                Warning<EnhancementMonkey>(message);
            }
            else if (level == LogLevel.Error)
            {
                Error<EnhancementMonkey>(message);
            }
        }
    }

    public static string Space(string string_)
    {
        if (string.IsNullOrWhiteSpace(string_))
            return "";
        StringBuilder newString = new(string_.Length * 2);

        for (int i = 0; i < string_.Length; i++)
        {
            if ((char.IsUpper(string_, i) | char.IsNumber(string_, i)) & i > 0)
            {
                newString.Append(' ');
            }
            newString.Append(string_[i]);
        }

        return newString.ToString();
    }

    public static bool menuOpen = false;

    public static List<EnhancementLevel> UnlockedLevels = [EnhancementLevel.Basic];

    public static List<ModEnhancement> NormalEnhancements = [];

    public static List<ModSubmenu> EnhancementSubmenus = [];

    public static readonly ModSettingBool DebugMode = new(false)
    {
        icon = VanillaSprites.SettingsIcon,
        description = "Print extra information into the log (uneeded for the average user)"
    };

    public enum LogLevel
    {
        /// <summary>
        /// Basic Info
        /// </summary>
        Info,
        /// <summary>
        /// Warnings except for dependencies
        /// </summary>
        Warn,
        /// <summary>
        /// All Errors
        /// </summary>
        Error,
        /// <summary>
        /// Show Dependency Warnings
        /// </summary>
        Dependency
    }

    public static readonly ModSettingEnum<LogLevel> DebugLevel = new(LogLevel.Error)
    {
        description = "Highest level displayed in debug mode"
    };

    public static readonly ModSettingBool LevelsUnlocked = new(false)
    {
        icon = VanillaSprites.UpgradeIcon,
        description = "Are all of the enhancement levels pre-unlocked?"
    };

    public static readonly ModSettingBool ShowHidden = new(false)
    {
        icon = VanillaSprites.EnhancedEyesightUpgradeIcon,
        description = "Show hidden enhancements? (By default there are none) "
    };

    public static readonly ModSettingBool ShowBuyPopups = new(false)
    {
        icon = VanillaSprites.WarningSign2,
        description = "Does a popup show saying you bought the enhancement after buying the enhancement? (Super annoying so false by default)",
        requiresRestart = true
    };

    public static readonly ModSettingInt EnhancementsLoadedPerFrame = new(200)
    {
        description = "How many enhancements to load per frame during loading, change this number if there's a lot of enhancements to load. Maxes at 1000/frame. \n\nOnly affects when the game first loads",
        max = 1000,
        min = 200
    };

    /// <summary>
    /// Clears everything.
    /// </summary>
    public static void Reset()
    {
        UnlockedLevels = [EnhancementLevel.Basic];

        foreach (var enhancement in GetContent<ModEnhancement>())
        {
            enhancement.Cost = enhancement.BaseCost;
            enhancement.TimesBought = 0;

            if (enhancement.LockedByDefault || !enhancement.HasDependencies)
            {
                enhancement.Locked = true;
            }
            else
            {
                enhancement.Locked = false;
            }

            enhancement.Added = false;
        }
        if (LevelsUnlocked)
        {
            foreach (ModEnhancement enhancement in ModEnhancement.Enhancements)
            {
                if (enhancement.Modifies == ModifyType.Unlock)
                {
                    enhancement.ModifyOther();
                    enhancement.Locked = true;
                }
            }
        }
        MIB = false;
    }

    public override void OnGameModelLoaded(GameModel model)
    {
        Reset();
        foreach ((string name, bool value) in ModSubmenu.Filters)
        {
            ModSubmenu.Filters[name] = true;
        }
    }

    public override void OnTowerSold(Il2CppAssets.Scripts.Simulation.Towers.Tower tower, float amount)
    {
        if (tower.towerModel.name.Contains("EnhancementMonkey"))
        {
            Reset();
            if (MainUi.instance != null)
            {
                MainUi.instance.Close();
            }
        }
    }

    public override void OnTowerCreated(Il2CppAssets.Scripts.Simulation.Towers.Tower tower, Entity target, Model modelToUse)
    {
        if (tower.towerModel.baseId.Contains(TowerID<Tower>()))
        {
            Reset();
        }
    }

    public override void OnTowerSelected(Il2CppAssets.Scripts.Simulation.Towers.Tower tower)
    {
        if (MainUi.instance != null)
        {
            MainUi.instance?.Close();
        }

        if (tower.towerModel.baseId == TowerID<Tower>()) { MainUi.CreateMainPanel(GetContent<ModSubmenu>(), tower); }
    }

    public override void OnTowerDeselected(Il2CppAssets.Scripts.Simulation.Towers.Tower tower)
    {
        if (MainUi.instance != null)
        {
            if (tower.towerModel.name.Contains("EnhancementMonkey"))
            {
                MainUi.instance.Close();
            }
        }
    }
}