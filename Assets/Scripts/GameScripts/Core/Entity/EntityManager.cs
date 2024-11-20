using NMX.Protocal;
using System;
using System.Collections.Generic;
using UnityEngine;
using QFSW.QC;
using System.Linq;
using QFSW.QC.Actions;

namespace NMX
{

    public class EntityManager : MonoBehaviour
    {
        [field: SerializeField] public List<GameObject> entitysFactory;

        [field: SerializeField] public Dictionary<uint,Entity> entities;

        public EntityManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            entities = new();

        }

        public enum SetPositionMode
        {
            Transform,
            Ridgidbody
        }

        private void Update()
        {
 
        }

        private void Start()
        {
            //for(uint i = 0; i< 2 ; i++)
            //{
            //    AppearEntityWithDuration(new GameEffectEntity(new EntityInfo { Id = i}, new Vector3(0, i + 10, 0)), 30);
            //}
        }

        public void AddEntityGameObjectToFactory(GameObject entity)
        {
            try
            {
                entitysFactory.Add(entity);
            } catch(Exception ex) {
                Debug.Log(ex);
            }
        }

        public void AppearEntityWithDuration(Entity entity,float duration)
        {
            entity.Load(gameObject.transform);

            Destroy(entity.OnLoadResourceGameObject, duration);
        }

        public void AppearEntity()
        {
            GameObject tmp = Resources.Load<GameObject>("NMX/Assets/Entitys/Player/GameObjects/Coop/COOP1");

            GameObject ent = Instantiate(tmp,GameSceneManager.Instance.SceneEntityTransform);

            DontDestroyOnLoad(ent);

            entitysFactory.Add(ent);

            LoadFactoryEntityIntroGame();
        }

        public void SetEntityPosition(uint id,Vector3 pos,SetPositionMode setPositionMode)
        {
            switch (setPositionMode) {
                case SetPositionMode.Transform:
                    entities[id].Position = pos;
                    break;
                case SetPositionMode.Ridgidbody:
                    entities[id].Rigidbody.MovePosition(pos);
                    break;
            }
        }

        [Command("setentitypos",MonoTargetType.All)]
        public void SetEPosCmd(uint id, Vector3 pos)
        {
            var target = FindEntityById(id);

            SetEntityPosition(id, pos,SetPositionMode.Transform);
            Debug.Log("set id:" + target.EntityId +" Name:" + target.gameObject.name + " To :" + pos);
        }

        [Command("iseexist", MonoTargetType.All)]
        public void isEntityExist(uint id)
        {
            Debug.Log(entities.ContainsKey(id));
            Debug.Log(entities[id].gameObject.name);

        }

        public Entity FindEntityById(uint id)
        {
            return entities[id];

        }



        public void LoadFactoryEntityIntroGame()
        {
            foreach (var entity in entitysFactory)
            {
                if (entity != null)
                {
                    try
                    {
                        IEntity ent = entity.GetComponent<IEntity>();

                        ent.Load(entity.transform);

                        Entity entcompo = entity.GetComponent<Entity>();

                        entities[entcompo.EntityId] = entcompo;

                        Debug.Log("Added To memory" + entcompo.gameObject.name);

                    } catch (NullReferenceException nullex)
                    {
                        Debug.LogError(nullex);
                        continue;
                    }
                }
            }

            // Finish Load Clear Factory

            entitysFactory.Clear();
        }


    }
}
