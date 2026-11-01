using BepInEx.Logging;
using Comfort.Common;
using EFT;
using EFT.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;

namespace hazelify.Lawnmower.Scripts
{
    public class Hedgemower : MonoBehaviour
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
        private List<GameObject> BushObjects = new List<GameObject>();
        private GameWorld _gameWorld;
        private bool hedgesWereTrimmed = false;
        private string currentMap = string.Empty;

        private void Awake()
        {
            _gameWorld = Singleton<GameWorld>.Instance;
            if (_gameWorld == null) return;
            player = _gameWorld.MainPlayer;

            fetchObjects();
            if (!Plugin.trees_masterSwitch.Value && Plugin.hedge_masterSwitch.Value) deleteObjects();
            Plugin.hedge_masterSwitch.SettingChanged += HandleSettingChanged;
        }

        private void OnDestroy()
        {
            Plugin.hedge_masterSwitch.SettingChanged -= HandleSettingChanged;
        }

        private void HandleSettingChanged(object a, EventArgs b)
        {
            if (!Plugin.trees_masterSwitch.Value)
            {
                if (Plugin.hedge_masterSwitch.Value)
                {
                    fetchObjects();
                    deleteObjects();
                }
                else
                {
                    restoreObjects();
                }
            }
        }

        private void restoreObjects()
        {
            if (BushObjects == null) return;

            foreach (var item in BushObjects)
            {
                item.SetActive(true);
            }

            hedgesWereTrimmed = false;
        }

        private static int reboundNumber(int slider)
        {
            return Mathf.RoundToInt(Mathf.Lerp(51f, 99f, slider / 100f));
        }

        private void fetchObjects()
        {
            currentMap = _gameWorld.LocationId.ToLower();

            if (availableMaps.Contains(currentMap))
            {
                string[] exclude = { "human", "dead", "abandoned", "treatment" };
                string[] include = { "filbert", "plant", "fern" };

                var currentMapObjects = FindObjectsOfType<GameObject>()
                    .Where(x =>
                    {
                        string n = x.name.ToLower();
                        return !exclude.Any(n.Contains) && include.Any(n.Contains);
                    })
                    .ToList();

                AllObjects = currentMapObjects;

                if (Plugin.hedge_useRemovePercentage.Value && Plugin.hedge_removePercentage.Value > 0)
                {
                    int currentPercentage = reboundNumber(Plugin.hedge_removePercentage.Value);
                    int amountToRemove = Mathf.RoundToInt(currentMapObjects.Count * currentPercentage / 100f);

                    BushObjects = currentMapObjects
                        .OrderBy(_ => UnityEngine.Random.value)
                        .Skip(amountToRemove)
                        .ToList();
                }
                else
                {
                    BushObjects = AllObjects;
                }

                /*
                foreach (var item in currentMapObjects)
                {
                    if (item == null) continue;
                    BushObjects.Add(item);
                }
                */
            }
        }

        private void deleteObjects()
        {
            if (BushObjects == null) return;

            foreach (var item in BushObjects)
            {
                if (item == null && !item.activeSelf) continue;
                item.SetActive(false);
            }

            hedgesWereTrimmed = true;
        }
    }
}
