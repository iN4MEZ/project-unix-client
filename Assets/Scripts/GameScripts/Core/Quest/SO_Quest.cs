using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [Serializable]
    [CreateAssetMenu(fileName ="Quest",menuName ="Quest/Create Quest")]
    public class SO_Quest : ScriptableObject
    {
        public string QuestName;
        public string QuestDescription;

        public Objective[] Objectives;

        public List<SO_Item> ItemReward;

    }
}
