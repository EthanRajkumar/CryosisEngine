using System;
using System.Collections.Generic;
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
        /// Appends a new <see cref="GameObject"/> to the collection. Fails if another object with a duplicate name exists.
        /// </summary>
        /// <param name="obj"></param>
        public void AddGameObject(GameObject obj)
        {
            if (GetGameObject(obj.Name) != null)
                return;

            GameObjects.Add(obj);
            obj.ParentCollection = this;

            if (obj.Parent == null)
                RootObjects.Add(obj);

            for(int i = 0; i < obj.Children.Count; i++)
                AddGameObject(obj.Children[i]);
        }

        /// <summary>
        /// Removes a <see cref="GameObject"/> from the collection.
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveGameObject(GameObject obj)
        {
            if (GetGameObject(obj.Name) == null)
                return;

            GameObjects.Remove(obj);
            RootObjects.Remove(obj);

            obj.ParentCollection = null;
        }
    }
}