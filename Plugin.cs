using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using hazelify.Lawnmower.Patches;
using hazelify.Lawnmower.Scripts;
using UnityEngine;

namespace hazelify.Lawnmower
{
    [BepInPlugin("hazelify.Lawnmower", "Lawnmower", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static string masterSwitchCategory = "Master switches";
        public static string generalCategory = "General";
        public static string hotkeysCategory = "Hotkeys";
        public static string valuesCategory = "Percentage values";

        public static ManualLogSource LogSource;

        public static ConfigEntry<bool> lawn_masterSwitch;
        public static ConfigEntry<bool> hedge_masterSwitch;
        public static ConfigEntry<bool> trees_masterSwitch;
        public static ConfigEntry<bool> useHotkeys;

        public static ConfigEntry<bool> hedge_useRemovePercentage;
        public static ConfigEntry<bool> trees_useRemovePercentage;

        public static ConfigEntry<int> hedge_removePercentage;
        public static ConfigEntry<int> trees_removePercentage;

        // public static ConfigEntry<float> maxDistance;
        // public static ConfigEntry<float> timeToCheck;

        public static ConfigEntry<KeyboardShortcut> hotkey_lawnMasterSwitch;
        public static ConfigEntry<KeyboardShortcut> hotkey_hedgeMasterSwitch;
        public static ConfigEntry<KeyboardShortcut> hotkey_treesMasterSwitch;

        private void Awake()
        {
            LogSource = Logger;
            InitConfiguration();
            new Patcher().Enable();
            LogSource.LogInfo($"[Lawnmower] Loaded!");
        }

        private void Update()
        {
            if (Plugin.useHotkeys.Value)
            {
                if (Plugin.hotkey_lawnMasterSwitch.Value.IsDown())
                {
                    Plugin.lawn_masterSwitch.Value = !Plugin.lawn_masterSwitch.Value;
                }

                if (Plugin.hotkey_hedgeMasterSwitch.Value.IsDown())
                {
                    Plugin.hedge_masterSwitch.Value = !Plugin.hedge_masterSwitch.Value;
                }

                if (Plugin.hotkey_treesMasterSwitch.Value.IsDown())
                {
                    Plugin.trees_masterSwitch.Value = !Plugin.trees_masterSwitch.Value;
                }
            }
        }

        private void InitConfiguration()
        {
            // toggles
            lawn_masterSwitch = Config.Bind(
                masterSwitchCategory, "Lawnmower",
                false,
                new ConfigDescription("Removes all the greass on the map; can be toggled in realtime and configured", null,
                new ConfigurationManagerAttributes { Order = 4 })
            );

            hedge_masterSwitch = Config.Bind(
                masterSwitchCategory, "Hedge trimmer",
                false,
                new ConfigDescription("Removes all the hedges (bushes) on the map; can be toggled in realtime and configured", null,
                new ConfigurationManagerAttributes { Order = 3 })
            );

            trees_masterSwitch = Config.Bind(
                masterSwitchCategory, "Tree feller",
                false,
                new ConfigDescription("Removes all the trees on the map; can be toggled in realtime and configured", null,
                new ConfigurationManagerAttributes { Order = 2 })
            );

            useHotkeys = Config.Bind(
                masterSwitchCategory, "Use hotkeys at all?",
                false,
                new ConfigDescription("If you don't want hotkeys from this mod to interfere with gameplay, turn this on.", null,
                new ConfigurationManagerAttributes { Order = 1 })
            );

            // hotkeys
            hotkey_lawnMasterSwitch = Config.Bind(
                hotkeysCategory, "Lawnmower hotkey",
                new KeyboardShortcut(KeyCode.F9),
                new ConfigDescription("What hotkey to use for the realtime Master Switch.", null,
                new ConfigurationManagerAttributes { Order = 3 })
            );

            hotkey_hedgeMasterSwitch = Config.Bind(
                hotkeysCategory, "Hedge trimmer hotkey",
                new KeyboardShortcut(KeyCode.F10),
                new ConfigDescription("What hotkey to use for the realtime Master Switch.", null,
                new ConfigurationManagerAttributes { Order = 2 })
            );

            hotkey_treesMasterSwitch = Config.Bind(
                hotkeysCategory, "Tree feller hotkey",
                new KeyboardShortcut(KeyCode.F11),
                new ConfigDescription("What hotkey to use for the realtime Master Switch.", null,
                new ConfigurationManagerAttributes { Order = 1 })
            );

            // percentage
            //lawn_useRemovePercentage = Config.Bind(
            //    generalCategory, "Enable percentage-based mowing",
            //    false,
            //    new ConfigDescription("Enables or disables if removing grass should be done in percentage (i.e 50% of all grass) instead of all of it.", null,
            //    new ConfigurationManagerAttributes { Order = 3 })
            //);

            hedge_useRemovePercentage = Config.Bind(
                generalCategory, "Enable percentage-based hedge trimming",
                false,
                new ConfigDescription("Enables or disables if removing bushes should be done in percentage (i.e 50% of all bushes) instead of all of it.", null,
                new ConfigurationManagerAttributes { Order = 2 })
            );

            trees_useRemovePercentage = Config.Bind(
                generalCategory, "Enable percentage-based tree felling",
                false,
                new ConfigDescription("Enables or disables if removing trees should be done in percentage (i.e 50% of all trees) instead of all of it.", null,
                new ConfigurationManagerAttributes { Order = 1 })
            );

            //lawn_removePercentage = Config.Bind(
            //    valuesCategory, "Grass mowing (in percent)",
            //    50f,
            //    new ConfigDescription(
            //        "How much of all grass should be mowed? Will not be used unless the percentage toggle is enabled. Also will not be used if value is 0\n\n" +
            //        "Percentage = 50% means half the grass on the map.",
            //    new AcceptableValueRange<float>(0f, 100f),
            //    new ConfigurationManagerAttributes { Order = 3 })
            //);

            hedge_removePercentage = Config.Bind(
                valuesCategory, "Hedge trimming (in percent)",
                50,
                new ConfigDescription(
                    "How much of all bushes should be mowed? Will not be used unless the percentage toggle is enabled. Also will not be used if value is 0\n\n" +
                    "Percentage = 50% means half the bushes on the map.",
                    new AcceptableValueRange<int>(0, 100),
                new ConfigurationManagerAttributes { Order = 2 })
            );

            trees_removePercentage = Config.Bind(
                valuesCategory, "Tree felling (in percent)",
                50,
                new ConfigDescription(
                    "How much of all trees should be mowed? Will not be used unless the percentage toggle is enabled. Also will not be used if value is 0\n\n" +
                    "Percentage = 50% means half the trees on the map.",
                    new AcceptableValueRange<int>(0, 100),
                new ConfigurationManagerAttributes { Order = 1 })
            );

            /*
            maxDistance = Config.Bind(
                "General",
                "Maximum distance before Lawnmowing (meters)",
                10f,
                new ConfigDescription(
                    "How far away in meters grass should render from the player before it all starts being Lawnmowed",
                    new AcceptableValueRange<float>(0f, 100f)
                )
            );
            */

            /*
            timeToCheck = Config.Bind(
                "General",
                "Grass scan interval (seconds",
                10f,
                new ConfigDescription(
                    "If `Minimum distance` is not 0, how often should the mod scan for grass?\n\nValue is in seconds",
                    new AcceptableValueRange<float>(1f, 100f)
                )
            );
            */
        }
    }
}
