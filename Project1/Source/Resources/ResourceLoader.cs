using System.Collections.Generic;
using System.Linq;

namespace CryosisEngine
{
    /// <summary>
    /// Manages the lifetime of a game asset/dataset via the number of references it has.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResourceReference<T>
    {
        /// <summary>
        /// The game asset / dataset to load into memory.
        /// </summary>
        public T Content { get; set; }

        /// <summary>
        /// The amount of references this resource has. Upon reaching 0, this resource will be removed from memory.
        /// </summary>
        public int ReferenceCount { get; set; } = 1;

        public ResourceReference(T content)
        {
            Content = content;
        }
    }

    public abstract class ResourceLoader<T>
    {
        /// <summary>
        /// Sets the root folder to load content from.
        /// </summary>
        public string RootDirectory { get; }

        /// <summary>
        /// Sets the subfolder of the root folder to load content from.
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Stores all references to any loaded resources, as well as their reference count. Resources are unloaded once they no longer hold any references.
        /// </summary>
        protected Dictionary<string, ResourceReference<T>> Resources { get; }

        public ResourceLoader(string rootDirectory)
        {
            RootDirectory = rootDirectory;
            Resources = new Dictionary<string, ResourceReference<T>>();
        }

        /// <summary>
        /// Sets the <see cref="WorkingDirectory"/> based on a file path's structure. If no working directory is specified, no changes are made.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string SetFilePath(string filePath)
        {
            string[] args = filePath.Split('/');

            if (args.Length == 0)
                return "";

            if (args.Length == 1)
                return args[0];

            WorkingDirectory = args[0];
            return args[1];
        }

        /// <summary>
        /// Adds and loads an item into the <see cref="Resources"/> dictionary. Sets its reference count to 1 by default;
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected virtual T AddItem(string path)
        {
            string fileName = SetFilePath(path);

            T resource = LoadItem(path);

            Resources.Add(path, new ResourceReference<T>(resource));

            return resource;
        }

        /// <summary>
        /// Loads a peice of content based on a file path.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected abstract T LoadItem(string filePath);

        /// <summary>
        /// Unloads a peice of content stored in memory.
        /// </summary>
        /// <param name="value"></param>
        protected abstract void UnloadItem(T value);

        /// <summary>
        /// Gets a peice of content from the reference list, incrementing its reference count. Loads a new peice of content if no match is found.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public T LoadContent(string path)
        {
            if (Resources.ContainsKey(path))
            {
                ResourceReference<T> value = Resources[path];
                value.ReferenceCount++;

                return value.Content;
            }
            else
                return AddItem(path);
        }

        /// <summary>
        /// Removes a reference to a peice of content, decrementing its reference count. If this value hits 0, the content is removed from the reference list, and unloaded from memory.
        /// </summary>
        /// <param name="content"></param>
        public virtual void UnloadContent(T content)
        {
            string key = Resources.Keys.FirstOrDefault(x => Resources[x].Content.Equals(content));

            ResourceReference<T> reference = Resources[key];

            if (reference.ReferenceCount == 1)
            {
                Resources.Remove(key);
                UnloadItem(reference.Content);
            }
            else
                Resources[key].ReferenceCount++;
        }
    }
}

/*

using System;
using System.Collections.Generic;
using System.Linq;

namespace CobaltEngine
{
    public abstract class ResourceLoader<T> : IContentLoader<T> where T : IDisposable
    {
        public string RootDirectory { get; }

        public string WorkingDirectory { get; set; }

        public Dictionary<string, ResourceReference<T>> Content { get; set; }

        public virtual void SetWorkingDirectory(string directory)
            => WorkingDirectory = directory;

        public ResourceLoader(string rootDirectory, string workingDirectory)
        {
            RootDirectory = rootDirectory;
            WorkingDirectory = workingDirectory;
            Content = new Dictionary<string, ResourceReference<T>>();
        }

        public virtual bool LoadContent(string contentPath, out T content)
        {
            if (Content.ContainsKey(contentPath))
            {
                Content[contentPath].AddReference();
                content = Content[contentPath].Content;
                return true;
            }
            else
            {
                content = LoadItem(contentPath);

                if (content != null)
                {
                    Content.Add(contentPath, new ResourceReference<T>(content));
                    Content[contentPath].Depleted += OnResourceDepletion;
                    return true;
                }
                else
                    return false;
            }
        }

        public abstract T LoadItem(string contentPath);

        public void UnloadContent(T content)
        {
            ResourceReference<T> reference = Content.Values.FirstOrDefault(x => x.Content.Equals(content));
            reference?.RemoveReference();
        }

        public void OnResourceDepletion(object info, ResourceReference<T> reference)
        {
            string key = Content.Keys.FirstOrDefault(x => Content[x] == reference);

            if (Content.ContainsKey(key))
            {
                Content.Remove(key);
                reference.Content.Dispose();
            }
        }

        public void UnloadContent(string contentPath)
        {
            if (Content.ContainsKey(contentPath))
                Content[contentPath].RemoveReference();
        }
    }
}
*/