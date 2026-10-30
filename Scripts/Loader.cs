using Comfort.Common;
using EFT;
using EFT.UI;
using System.Threading.Tasks;
using UnityEngine;

namespace hazelify.Lawnmower.Scripts
{
    public class Loader : MonoBehaviour
    {
        private GameWorld _gameWorld;
        private Player _player;
        public Lawnmower _lawnMowerScript;
        public Hedgemower _hedgeMowerScript;
        public Treefeller _treeFellerScript;

        private void Awake()
        {
            _gameWorld = Singleton<GameWorld>.Instance;
            
            if (_gameWorld == null)
            {
                ConsoleScreen.LogError("[Lawnmower] GameWorld not found");
                Destroy(this);
                return;
            }

            _player = _gameWorld.MainPlayer;

            if (_player == null)
            {
                ConsoleScreen.LogError("[Lawnmower] Player not found");
                Destroy(this);
                return;
            }

            if (_player.Location.ToLower() == "hideout")
            {
                ConsoleScreen.LogError("[Lawnmower] Cannot run in Hideout");
                Destroy(this);
                return;
            }

            setupScript();
        }

        private void setupScript()
        {
            _lawnMowerScript = _gameWorld.gameObject.AddComponent<Lawnmower>();
            _hedgeMowerScript = _gameWorld.gameObject.AddComponent<Hedgemower>();
            _treeFellerScript = _gameWorld.gameObject.AddComponent<Treefeller>();
        }
    }
}
