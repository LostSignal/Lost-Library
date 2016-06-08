//-----------------------------------------------------------------------
// <copyright file="Pooler.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    
    public static class Pooler
    {
        public static GameObject Instantiate(GameObject gameObj)
        {
            return PrivateInstantiate(gameObj);
        }
        
        public static GameObject Instantiate(GameObject gameObj, Vector3 position)
        {
            GameObject newGameObject = PrivateInstantiate(gameObj);
            newGameObject.transform.position = position;
            return newGameObject;
        }
        
        public static GameObject Instantiate(GameObject gameObj, Vector3 position, Quaternion rotation)
        {
            GameObject newGameObject = PrivateInstantiate(gameObj);
            newGameObject.transform.position = position;
            newGameObject.transform.rotation = rotation;
            return newGameObject;
        }
        
        public static GameObject Instantiate(GameObject gameObj, Transform transform)
        {
            GameObject newGameObject = PrivateInstantiate(gameObj);
            newGameObject.transform.position = transform.position;
            newGameObject.transform.rotation = transform.rotation;
            newGameObject.transform.localScale = transform.localScale;
            return newGameObject;
        }

        public static void Destroy(this GameObject obj)
        {
            GameObject.Destroy(obj);
        }

        private static GameObject PrivateInstantiate(GameObject gameObj)
        {
            return GameObject.Instantiate(gameObj) as GameObject;
        }
    }
}
