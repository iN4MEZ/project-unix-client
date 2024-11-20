using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public class AvatarManager : AvatarEntity
    {

        [field: Header("Base Profile Data")]

        [field: SerializeField] public AvatarSO SOData;

        [field: Header("Skill Data")]

        public AvatarSkill Skill;

        public AvatarSkillTree SkillTree;

        [field: Header("Skill Behavior")]

        public AvatarSkillBehavior Behavior;

        public Animator Animator { get; private set; }

        public Rigidbody Rigidbody { get; private set; }

        public int CurrentHp {  get; private set; }


        private void Awake()
        {
            CurrentHp = SOData.Data.AvatarMaxHealth;

            Rigidbody = GetComponent<Rigidbody>();  

            Behavior = GetComponent<AvatarSkillBehavior>();
        }

        private void Update()
        {

        }

        public void InitializeAvatarAnimation()
        {
            Animator = GetComponentInChildren<Animator>();

            Debug.Log(SOData.Data.AvatarName+" Animation Loaded!!");
        }
    }
}
