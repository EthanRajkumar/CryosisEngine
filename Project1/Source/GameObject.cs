using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryosisEngine
{
    public class GameObject : IUpdate, IDraw
    {
        GameObject _parent;

        GameObjectCollection _parentCollection;

        /// <summary>
        /// The local name of this <see cref="GameObject"/> within a collection.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        /// <inheritdoc/>
        public bool IsVisible { get; set; }

        /// <inheritdoc/>
        public float Alpha { get; set; } = 1f;

        /// <summary>
        /// The 2D spacial representation of this <see cref="GameObject"/>.
        /// </summary>
        public Transform2D Transform { get; set; }

        /// <summary>
        /// Represents this object's parented <see cref="GameObject"/>.
        /// </summary>
        public GameObject Parent
        {
            get => _parent;

            set
            {
                _parent?.Children.Remove(this);
                _parent = value;

                if (value == null)
                    return;

                value.Children.Add(this);
                Transform.Parent = value.Transform;
            }
        }

        public GameObjectCollection ParentCollection
        {
            get => _parentCollection;

            set => _parentCollection = value;
        }

        /// <summary>
        /// Tells whether this <see cref="GameObject"/> is a "root" object, or doesn't have any parents.
        /// </summary>
        public bool IsChild => Parent == null;

        /// <summary>
        /// Represents each <see cref="GameObject"/> who is parented to this one.
        /// </summary>
        public List<GameObject> Children { get; } = new List<GameObject>();

        private List<GameComponent> Components { get; } = new List<GameComponent>();

        public GameObject(string name, Transform2D transform)
        {
            Name = name;
            Transform = transform;
            IsActive = IsVisible = true;
        }

        /// <inheritdoc/>
        public void Awake(GameServiceContainer services)
        {
            for (int i = 0; i < Components.Count; i++)
                Components[i].Awake(services);

            for (int i = 0; i < Children.Count; i++)
                Children[i].Awake(services);
        }

        /// <inheritdoc/>
        public void Sleep(GameServiceContainer services)
        {
            UnloadContent(services);

            for (int i = 0; i < Components.Count; i++)
                Components[i].Sleep(services);

            for (int i = 0; i < Children.Count; i++)
                Children[i].Sleep(services);
        }

        /// <inheritdoc/>
        public void EarlyUpdate(GameTime gameTime)
        {
            if (!IsActive)
                return;

            for (int i = 0; i < Components.Count; i++)
                Components[i].EarlyUpdate(gameTime);

            for (int i = 0; i < Children.Count; i++)
                Children[i].EarlyUpdate(gameTime);
        }

        /// <inheritdoc/>
        public void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            for (int i = 0; i < Components.Count; i++)
                Components[i].Update(gameTime);

            for (int i = 0; i < Children.Count; i++)
                Children[i].Update(gameTime);
        }

        /// <inheritdoc/>
        public void LateUpdate(GameTime gameTime)
        {
            if (!IsActive)
                return;

            for (int i = 0; i < Components.Count; i++)
                Components[i].LateUpdate(gameTime);

            for (int i = 0; i < Children.Count; i++)
                Children[i].LateUpdate(gameTime);
        }

        /// <inheritdoc/>
        public void LoadContent(GameServiceContainer services)
        {
            for (int i = 0; i < Components.Count; i++)
                Components[i].LoadContent(services);

            for (int i = 0; i < Children.Count; i++)
                Children[i].LoadContent(services);
        }

        /// <inheritdoc/>
        public void UnloadContent(GameServiceContainer services)
        {
            for (int i = 0; i < Components.Count; i++)
                Components[i].UnloadContent(services);

            for (int i = 0; i < Children.Count; i++)
                Children[i].UnloadContent(services);
        }

        /// <inheritdoc/>
        public void EarlyDraw(SpriteBatch spriteBatch, Vector2 offset, Rectangle camera, float viewportScale = 1f, float alpha = 1f)
        {
            if (!IsVisible || alpha == 0f)
                return;

            for (int i = 0; i < Components.Count; i++)
                Components[i].EarlyDraw(spriteBatch, offset, camera, viewportScale, Alpha * alpha);

            for (int i = 0; i < Children.Count; i++)
                Children[i].EarlyDraw(spriteBatch, offset, camera, viewportScale, Alpha * alpha);
        }

        /// <inheritdoc/>
        public void Draw(SpriteBatch spriteBatch, Vector2 offset, Rectangle camera, float viewportScale = 1f, float alpha = 1f)
        {
            if (!IsVisible || alpha == 0f)
                return;

            for (int i = 0; i < Components.Count; i++)
                Components[i].Draw(spriteBatch, offset, camera, viewportScale, Alpha * alpha);

            for (int i = 0; i < Children.Count; i++)
                Children[i].Draw(spriteBatch, offset, camera, viewportScale, Alpha * alpha);
        }

        /// <inheritdoc/>
        public void LateDraw(SpriteBatch spriteBatch, Vector2 offset, Rectangle camera, float viewportScale = 1f, float alpha = 1f)
        {
            if (!IsVisible || alpha == 0f)
                return;

            for (int i = 0; i < Components.Count; i++)
                Components[i].LateDraw(spriteBatch, offset, camera, viewportScale, Alpha * alpha);

            for (int i = 0; i < Children.Count; i++)
                Children[i].LateDraw(spriteBatch, offset, camera, viewportScale, Alpha * alpha);
        }

        public bool HasChild(GameObject gameObject)
        {
            for(int i = 0; i < Children.Count; i++)
            {
                if (Children[i] == gameObject || Children[i].HasChild(gameObject))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Parents a <see cref="GameObject"/> to this one. Fails if the chain of parenting causes a cyclic dependency.
        /// </summary>
        /// <param name="child"></param>
        public void AddObject(GameObject child)
        {
            if (HasChild(child) || child.HasChild(this))
                return;

            child.Parent = this;
            Children.Add(child);
        }

        /// <summary>
        /// Removes a <see cref="GameObject"/> from this one.
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(GameObject gameObject)
        {
            if(Children.Contains(gameObject))
                gameObject.Parent = null;

            Children.Remove(gameObject);
        }

        /// <summary>
        /// Adds a <see cref="GameComopnent"/> to this <see cref="GameObject"/>.
        /// </summary>
        /// <param name="child"></param>
        public void AddComponent(GameComponent component)
        {
            component.Remove();
            component.Parent = this;
            Components.Add(component);
        }

        /// <summary>
        /// Removes a <see cref="GameComponent"/> from this <see cref="GameObject"/>.
        /// </summary>
        /// <param name="child"></param>
        public void RemoveComponent(GameComponent component)
        {
            if (Components.Contains(component))
                component.Parent = null;

            Components.Remove(component);
        }

        /// <summary>
        /// Finds an owned <see cref="GameComponent"/> of a specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : GameComponent
            => Components.FirstOrDefault(x => x is T) as T;

        /// <summary>
        /// Removes this <see cref="GameObject"/> from its parent. This object then becomes a root object, by definition.
        /// </summary>
        public void Remove()
        {
            Parent?.RemoveChild(this);
        }
    }
}