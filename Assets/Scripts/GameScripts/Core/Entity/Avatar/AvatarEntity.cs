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


        public override void Load(Transform transform, EntityInfo entityInfo)
        {
            base.Load(transform, entityInfo);

            var resourceGameObject = Resources.Load(EntityData.ModelPath) as GameObject;

            if (resourceGameObject != null) {

                OnLoadResourceGameObject = Instantiate(resourceGameObject, transform);

                if (OnLoadResourceGameObject.GetComponent<AvatarEntity>() == null)
                {
                    var acompo = OnLoadResourceGameObject.AddComponent<AvatarEntity>();
                    acompo.EntityId = this.EntityId;
                }

            }

            //Debug.Log(EntityData.ModelPath);

        }
    }
}
