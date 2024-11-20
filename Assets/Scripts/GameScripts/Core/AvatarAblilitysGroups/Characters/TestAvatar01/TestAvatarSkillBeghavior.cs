using UnityEngine;

namespace NMX
{
    public class TestAvatarSkillBeghavior : AvatarSkillBehavior
    {

        private void Start()
        {
            Debug.Log("Skill Has Been Loaded!  " + this.gameObject.name);
        }
    }
}
