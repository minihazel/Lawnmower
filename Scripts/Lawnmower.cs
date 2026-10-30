using BepInEx.Logging;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using EFT.UI;
using GPUInstancer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace hazelify.Lawnmower.Scripts
{
    public class Lawnmower : MonoBehaviour
    {
        //private static GameWorld gameWorld;
        //private Player player;
        //float _time;
        //private Vector3 playerPos;
        //float squareLimit = Plugin.maxDistance.Value * Plugin.maxDistance.Value;

        protected static ManualLogSource Logger
        {
            get; private set;
        }

        private List<GPUInstancerDetailManager> GrassObjects;
        //private List<GPUInstancerDetailManager> AllObjects;
        private bool grassWasMowed = false;

        //private void Start()
        //{
        //    gameWorld = Singleton<GameWorld>.Instance;
        //    if (gameWorld == null) return;

        //    player = gameWorld.MainPlayer;
        //    _time = 0f;
        //}

        private void Awake()
        {
            fetchObjects();
            if (Plugin.lawn_masterSwitch.Value) deleteObjects();
            Plugin.lawn_masterSwitch.SettingChanged += HandleSettingChanged;
        }

        private void OnDestroy()
        {
            Plugin.lawn_masterSwitch.SettingChanged -= HandleSettingChanged;
        }

        private void HandleSettingChanged(object a, EventArgs b)
        {
            if (Plugin.useHotkeys.Value)
            {
                if (Plugin.lawn_masterSwitch.Value)
                {
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
            if (GrassObjects == null) return;

            foreach (var item in GrassObjects)
            {
                if (item == null) continue;
                item.SetEnabledUniversal(true);
            }

            grassWasMowed = false;
        }

        //private void Update()
        //{
        //    if (!Plugin.masterSwitch.Value && grassWasMowed)
        //    {
        //        restoreObjects();
        //    }

        //    if (!Plugin.masterSwitch.Value) return;
        //    if (Plugin.maxDistance.Value <= 0) return;

        //    _time += Time.deltaTime;

        //    if (_time < Plugin.timeToCheck.Value) return;
        //    _time = 0f;

        //    deleteObjects();
        //}

        private void fetchObjects()
        {
            GrassObjects = FindObjectsOfType<GPUInstancerDetailManager>().Where(i => i.IsEnabledUniversal()).ToList();
        }

        private void deleteObjects()
        {
            if (GrassObjects == null) return;

            //float limit = Plugin.maxDistance.Value;
            //float squareLimit = limit * limit;

            //if (limit > 0)
            //{
            //    if (player == null) return;
            //    playerPos = player.Position;
            //}

            foreach (var item in GrassObjects)
            {
                if (item == null && !item.IsEnabledUniversal()) continue;
                item.SetEnabledUniversal(false);

                //if (limit <= 0)
                //{
                //    item.SetEnabledUniversal(false);
                //    continue;
                //}

                //bool isClose = (item.transform.position - playerPos).sqrMagnitude <= squareLimit;
                //item.SetEnabledUniversal(isClose);
            }

            grassWasMowed = true;
        }
    }
}
