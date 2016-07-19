//// using UnityEngine;
//// 
//// [ExecuteInEditMode]
//// public class PrefabPlacer : MonoBehaviour, ISerializationCallbackReceiver
//// {
////     public GameObject Prefab;
////     private GameObject prefabInstance;
//// 
////     void ISerializationCallbackReceiver.OnAfterDeserialize()
////     {
////         this.Initialize();
////     }
//// 
////     void ISerializationCallbackReceiver.OnBeforeSerialize()
////     {
////         if (this.prefabInstance != null)
////         {
////             DestroyImmediate(prefabInstance);
////             this.prefabInstance = null;
////         }
////     }
////     
////     void Update()
////     {
////         this.Initialize();
////     }
//// 
////     void Initialize()
////     {
////         if (this.prefabInstance == null && this.Prefab != null)
////         {
////             this.prefabInstance = Instantiate<GameObject>(this.Prefab);
////             this.prefabInstance.hideFlags = HideFlags.HideAndDontSave;
////             this.prefabInstance.transform.SetParent(this.transform);
////             this.prefabInstance.transform.localPosition = Vector3.zero;
////         }
////     }
//// }
//// 