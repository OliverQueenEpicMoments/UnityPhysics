using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamComponent : MonoBehaviour {
    [SerializeField] private TeamIndex teamindex = TeamIndex.None;

    public TeamIndex TeamIndex { 
        set { 
            if (teamindex == value) return;

            teamindex = value;
        }
        get { return teamindex; }
    }
}

public enum TeamIndex : sbyte { 
    None = -1,
    Neutral = 0,
    Player,
    Enemy,
    Count
}