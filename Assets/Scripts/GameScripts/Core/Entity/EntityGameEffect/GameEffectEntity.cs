using NMX.Protocal;
using UnityEngine;

namespace NMX
{
    public class GameEffectEntity : Entity
    {

        public GameEffectEntity(EntityInfo info, Vector3 pos) {
            EntityId = info.Id;
            Position = pos;
            EntityType = EntityType.EFFOBJECT;
        }

        public override void Load(Transform transform, EntityInfo entityInfo)
        {
            base.Load(transform, entityInfo);

            var resourceGameObject = Resources.Load("NMX/Assets/Entitys/Avatar/Prefabs/Models/Elysia/Avatar_Elysia_C2_MC_OW") as GameObject;

            OnLoadResourceGameObject = Instantiate(resourceGameObject, transform);

            OnLoadResourceGameObject.transform.position = Position;

            Debug.Log("Effect Has Loaded!");
        }
    }
}
