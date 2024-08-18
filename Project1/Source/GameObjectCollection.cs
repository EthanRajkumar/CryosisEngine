using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CryosisEngine
{
    public class GameObjectCollection
    {
        /// <summary>
        /// A collection of all <see cref="GameObject"/> instances present.
        /// </summary>
        public List<GameObject> GameObjects { get; set; } = new List<GameObject>();

        /// <summary>
        /// A collection of all <see cref="GameObject"/> instances that have no parent.
        /// </summary>
        public List<GameObject> RootObjects { get; set; } = new List<GameObject>();

        /// <summary>
        /// Finds a <see cref="GameObject"/> via its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject GetGameObject(string name)
            => GameObjects.FirstOrDefault(x => x.Name == name);

        /// <summary>
        /// Appends a new <see cref="GameObject"/> to the collection and all elements under it.
        /// </summary>
        /// <param name="obj"></param>
        public void AddGameObject(GameObject obj)
        {
            if (GameObjects.Contains(obj))
                return;

            List<GameObject> objects = new List<GameObject>();
            obj.GetAllChildren(objects);

            for (int i = 0; i < objects.Count; i++)
            {
                GameObjects.Add(objects[i]);
                objects[i].ParentCollection = this;
            }

            RootObjects.Add(obj);
        }

        /// <summary>
        /// Removes a <see cref="GameObject"/> from the collection and all elements under it.
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveGameObject(GameObject obj)
        {
            if (!GameObjects.Contains(obj))
                return;

            List<GameObject> objects = new List<GameObject>();
            obj.GetAllChildren(objects);

            for (int i = 0; i < objects.Count; i++)
                GameObjects.Remove(objects[i]);

            RootObjects.Remove(obj);
        }
    }
}