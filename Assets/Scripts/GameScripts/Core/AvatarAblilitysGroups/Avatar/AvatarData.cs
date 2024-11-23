using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [Serializable]
    public class AvatarData : EntityData
    {
        [field: SerializeField] public string AvatarName { get; set; }


        [field: SerializeField] public int AvatarBaseDamage { get; set; }


        [field: SerializeField] public int AvatarMaxHealth { get; set; }


        // Default 6000

        [field: SerializeField] public int AvatarDefensive { get; set; }

        [field: SerializeField] public float AvatarCritRate { get; set; }

        [field: SerializeField] public float AvatarCritDamage { get; set; }

        [field: SerializeField] public int RealId { get; set; }

        public string ResourcePath { get; set; }



        [field: SerializeField] public static GameObject AvatarObjectInstance;

        //Profile Config
        //NOTE: Level 0 1 2
        private int DamageReactionTypeBonusLevel = 0;

        [field: SerializeField] public uint AvatarID { get; set; }


    }
}
