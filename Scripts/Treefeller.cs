using BepInEx.Logging;
using Comfort.Common;
using EFT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace hazelify.Lawnmower.Scripts
{
    public class Treefeller : MonoBehaviour
    {
        protected static ManualLogSource Logger
        {
            get; private set;
        }

        private static GameWorld gameWorld;
        private Player player;

        private List<string> availableMaps = new List<string>()
        {
            "woods",
            "bigmap",
            "tarkovstreets",
            "lighthouse",
            "rezervbase",
            "shoreline",
            "interchange",
            "sandbox",
            "sandbox_high",
        };

        private List<GameObject> AllObjects = new List<GameObject>();
        private List<GameObject> TreeObjects = new List<GameObject>();
        private GameWorld _gameWorld;
        private bool treesWereFelled = false;
        private string currentMap = string.Empty;

        private void Awake()
        {
            _gameWorld = Singleton<GameWorld>.Instance;
            if (_gameWorld == null) return;
            player = _gameWorld.MainPlayer;

            fetchObjects();
            if (Plugin.trees_masterSwitch.Value) deleteObjects();
            Plugin.trees_masterSwitch.SettingChanged += HandleSettingChanged;
        }

        private void OnDestroy()
        {
            Plugin.trees_masterSwitch.SettingChanged -= HandleSettingChanged;
        }

        private void HandleSettingChanged(object a, EventArgs b)
        {
            if (Plugin.trees_masterSwitch.Value)
            {
                fetchObjects();
                deleteObjects();
            }
            else
            {
                restoreObjects();
            }
        }

        private void restoreObjects()
        {
            if (TreeObjects == null) return;

            foreach (var item in TreeObjects)
            {
                item.SetActive(true);
            }

            treesWereFelled = false;
        }

        private void fetchObjects()
        {
            currentMap = _gameWorld.LocationId.ToLower();

            if (availableMaps.Contains(currentMap))
            {
                var currentMapObjects = FindObjectsOfType<GameObject>()
                    .Where(x => !x.name.ToLower().Contains("human") &&
                    (x.name.ToLower().Contains("pine") ||
                    x.name.ToLower().Contains("birch") ||
                    x.name.ToLower().Contains("tree"))).ToList();

                AllObjects = currentMapObjects;

                if (Plugin.trees_useRemovePercentage.Value && Plugin.trees_removePercentage.Value > 0)
                {
                    int currentPercentage = Plugin.trees_removePercentage.Value;
                    int amountToRemove = Mathf.RoundToInt(currentMapObjects.Count * currentPercentage / 100f);

                    TreeObjects = currentMapObjects
                        .OrderBy(_ => UnityEngine.Random.value)
                        .Skip(amountToRemove)
                        .ToList();
                } 
                else
                {
                    TreeObjects = AllObjects;
                }

                /*
                foreach (var item in currentMapObjects)
                {
                    if (item == null) continue;
                    TreeObjects.Add(item);
                }
                */
            }
        }

        private void deleteObjects()
        {
            if (TreeObjects == null) return;

            foreach (var item in TreeObjects)
            {
                if (item == null && !item.activeSelf) continue;
                item.SetActive(false);
            }

            treesWereFelled = true;
        }
    }
}
