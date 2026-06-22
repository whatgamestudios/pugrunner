// Copyright (c) Whatgame Studios 2026
using UnityEngine;
using TMPro;

namespace PugRunner {

    public class GameStatsBoard : MonoBehaviour {
        public TextMeshProUGUI stats;

        public void SetInfo(string info)
        {
            stats.text = info;
        }
    }
}
