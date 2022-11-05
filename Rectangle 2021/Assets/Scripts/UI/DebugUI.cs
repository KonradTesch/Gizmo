using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Rectangle.Player;

namespace Rectangle.UI
{
    public class DebugUI : MonoBehaviour
    {
        public PlayerController modeController;

        [Space]

        public TextMeshProUGUI db_Grounded;
        public TextMeshProUGUI db_OnWall;
        public TextMeshProUGUI db_OnRamp;
        public TextMeshProUGUI db_Rectangle;
        public TextMeshProUGUI db_Sphere;
        public TextMeshProUGUI db_Spikey;
        public TextMeshProUGUI db_Little;


        private PlayerBase activePlayer;
        // Start is called before the first frame update
        void Start()
        {
            activePlayer = modeController.activePlayer;
        }

        // Update is called once per frame
        void Update()
        {
            if(activePlayer.TryGetComponent(out PlayerSpikey spikey))
            {
                db_OnWall.color = TextColor(spikey.onWall);
            }
            else
            {
                db_OnWall.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            }
            //db_Grounded.color = TextColor(activePlayer.grounded);
            //db_OnRamp.color = TextColor(activePlayer.onRamp);
            //db_Rectangle.color = TextColor(activePlayer.playerMode.name == "Rectangle");
            //db_Sphere.color = TextColor(activePlayer.playerMode.name == "Sphere");
            //db_Spikey.color = TextColor(activePlayer.playerMode.name == "Spikey");
            //db_Little.color = TextColor(activePlayer.playerMode.name == "Little");

        }

        private Color TextColor(bool condition)
        {
            if (condition)
                return Color.black;
            else
                return new Color(0, 0, 0, 0.3f);
        }
    }
}