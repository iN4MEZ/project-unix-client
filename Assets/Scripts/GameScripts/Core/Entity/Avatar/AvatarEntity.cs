using NMX.Protocal;
using UnityEngine;

namespace NMX
{
    public class AvatarEntity : Entity
    {
        [field: SerializeField] public AvatarEntityData EntityData {  get; private set; }

        public AvatarEntity()
        {
        }

        public AvatarEntity(string modelPath)
        {
            EntityData = new AvatarEntityData();
            EntityData.ModelPath = modelPath;
        }
        public AvatarEntity(string modelPath,EntityInfo entityInfo)
        {
            EntityData = new AvatarEntityData();

            EntityId = entityInfo.Id;

            EntityType = (NMX.EntityType)(int)entityInfo.EType;

            EntityData.ModelPath = modelPath;
        }


        public override void Load(Transform transform)
        {
            base.Load(transform);

            var resourceGameObject = Resources.Load(EntityData.ModelPath) as GameObject;

            if (resourceGameObject != null) {

                OnLoadResourceGameObject = Instantiate(resourceGameObject, transform);

            }

            //Debug.Log(EntityData.ModelPath);

        }
    }
}
