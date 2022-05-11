using System;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.MiniGame.WordFinder
{
    /// <summary>
    /// Object pool for game objects with certain behaviours deriving from <see cref="IPoolObject"/>.
    /// </summary>
    /// <typeparam name="T">Mono behaviour to be pooled</typeparam>
    internal class ObjectPool<T> where T : MonoBehaviour, IPoolObject
    {
        /// <summary>
        /// Stack of all available pooled objects.
        /// </summary>
        private Stack<T> _availableObjects = new Stack<T>();

        /// <summary>
        /// List of all active objects.
        /// </summary>
        private List<T> _activeObjects = new List<T>();

        /// <summary>
        /// List of all active objects.
        /// </summary>
        internal List<T> ActiveObjects => _activeObjects;

        /// <summary>
        /// Creates an instance of the given prefab.
        /// Either gets an existing instance and resets it, or creates a new one
        /// if the pool is empty.
        /// </summary>
        /// <param name="prefab">Prefab to be created</param>
        /// <param name="parent">Parent transform</param>
        /// <param name="init">Called before the object is set to active</param>>
        /// <returns>Object of type <see cref="T"/></returns>
        internal T GetObject(T prefab, Transform parent, Action<T> init = null)
        {
            T obj;

            prefab.gameObject.SetActive(false);
            if (_availableObjects.Count == 0)
                obj = GameObject.Instantiate(prefab);
            else
                obj = _availableObjects.Pop();

            obj.transform.SetParent(parent, false);
            init?.Invoke(obj);
            _activeObjects.Add(obj);
            obj.gameObject.SetActive(true);
            obj.ResetObject();
            prefab.gameObject.SetActive(true);
            return obj;
        }

        /// <summary>
        /// Returns an active object to the pool.
        /// Only objects created by the pool can be returned to the pool.
        /// </summary>
        /// <param name="obj">Object to the returned</param>
        /// <param name="newParent">Parent the object should be returned to</param>
        internal void ReturnObject(T obj, Transform newParent = null)
        {
            if (!_activeObjects.Remove(obj))
                return;

            if (newParent != null)
                obj.transform.SetParent(newParent, false);

            obj.gameObject.SetActive(false);
            _availableObjects.Push(obj);
        }

        /// <summary>
        /// Returns all active objects to the object pool.
        /// </summary>
        /// <param name="newParent">Parent the object should be returned to</param>
        internal void ReturnAllObjects(Transform newParent = null)
        {
            for (int i = 0; i < _activeObjects.Count; i++)
            {
                if (newParent != null)
                    _activeObjects[i].transform.SetParent(newParent, false);

                _activeObjects[i].gameObject.SetActive(false);
                _availableObjects.Push(_activeObjects[i]);
            }
            _activeObjects.Clear();
        }
    }
}
