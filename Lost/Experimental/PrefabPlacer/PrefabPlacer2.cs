////
//// using UnityEngine;
////
//// [ExecuteInEditMode]
//// public class PrefabPlacer : MonoBehaviour
//// {
////     public GameObject Prefab;
////     private GameObject prefabInstance;
////     private GameObject currentSelection;
////
////     #if UNITY_EDITOR
////     void Update()
////     {
////         if (Application.isPlaying == false && this.prefabInstance == null && this.Prefab != null)
////         {
////             this.prefabInstance = Pooler.Instantiate<GameObject>(this.Prefab);
////             this.prefabInstance.hideFlags = HideFlags.HideAndDontSave;
////             this.prefabInstance.transform.SetParent(this.transform);
////             this.prefabInstance.transform.localPosition = Vector3.zero;
////         }
////     }
////     #endif
////
////     void Awake()
////     {
////         if (Application.isPlaying)
////         {
////             var instance = Pooler.Instantiate<GameObject>(this.Prefab);
////             instance.name = this.name;
////             instance.transform.SetParent(this.transform.parent);
////             instance.transform.localPosition = this.transform.localPosition;
////             instance.transform.localRotation = this.transform.localRotation;
////             instance.transform.localScale = this.transform.localScale;
////
////             int index = this.transform.GetSiblingIndex();
////             Pooler.DestroyImmediate(this.gameObject);
////             instance.transform.SetSiblingIndex(index);
////         }
////     }
//// }
////
//// Child with hideFlags set to HideAndDontSave means the parent is no longer selectable
////
//// I'm trying to solve the problem with Nested Prefabs.  This script does a nice job at letting me reference the prefab and see how it looks in the secne.
//// It also uses the HideAndDontSave so it doesn't clutter up the hierarchy when it childs the prefab instance under it.  The only issue is that it's
//// impossible to select the PrefabPlacer object itself.  It seems that all selection code is hitting the hidden child and consuming the event.
//// What would be nice is if I can somehow get ahold of that event and test if it's hidden and if it is, then select the PrefabPlacer parent.
//// Anyone know how that might be possible.
////
//// Thanks in advance!
