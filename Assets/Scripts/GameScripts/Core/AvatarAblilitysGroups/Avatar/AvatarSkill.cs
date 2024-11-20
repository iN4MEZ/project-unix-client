using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [Serializable]
    public class AvatarSkill
    {
        [field: Header("Tactic Skill Profile")]
        [field: SerializeField] public float TacticCooldownTime { get; protected set; }
        [field: SerializeField] public int TacticSkillId { get; protected set; }
        [field: SerializeField] public string TacticSkillName { get; protected set; }
        [field: SerializeField] public float TacticDuration { get; protected set; }


        [field: Header("Unix Skill Profile")]
        [field: SerializeField] public float UnixCooldownTime { get; protected set; }
        [field: SerializeField] public int UnixSkillId { get; protected set; }
        [field: SerializeField] public string UnixSkillName { get; protected set; }


        [field: Header("Umix Skill Profile")]
        [field: SerializeField] public float UmixCooldownTime { get; protected set; }
        [field: SerializeField] public float UmixSkillId{ get; protected set; }
        [field: SerializeField] public string UmixSkillName { get; protected set; }
    }
}
