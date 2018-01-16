### CalledByUnityEvent Attribute
--------------------------------
* Make [CalledByUnityEvent] attribute
  * At build time, find all UnityEvent referneces, and then make sure that all
    functions that have the CalledByUnityEvent attribute are in fact called.
    Throw a build warning if they are not called.
    * https://forum.unity.com/threads/meta-files-and-the-fileid.366576/
    * https://forum.unity.com/threads/yaml-fileid-hash-function-for-dll-scripts.252075/#post-1695479

* This combo finds the Method Name
```
      - m_Target: {fileID: 114566219751628992}
        m_MethodName: SetFriends
```

* This combo finds the component it belonged (m_Script)
```
--- !u!114 &114566219751628992
MonoBehaviour:
  m_ObjectHideFlags: 1
  m_PrefabParentObject: {fileID: 0}
  m_PrefabInternal: {fileID: 100100000}
  m_GameObject: {fileID: 1121525888519158}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 5a44f06cf8da8cf468044b62dfbb350e, type: 3}
```

* GameObject
```
--- !u!1 &1121525888519158
GameObject:
  m_ObjectHideFlags: 0
  m_PrefabParentObject: {fileID: 0}
  m_PrefabInternal: {fileID: 100100000}
  serializedVersion: 5
  m_Component:
  - component: {fileID: 224247408571323358}
  - component: {fileID: 223027526400628014}
  - component: {fileID: 114871896982914564}
  - component: {fileID: 114068816805200002}
  - component: {fileID: 114010525240991064}
  - component: {fileID: 95921545359007722}
  - component: {fileID: 114797355569949316}
  - component: {fileID: 114566219751628992}
  m_Layer: 0
  m_Name: Main
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
```

* RectTransform
```
--- !u!224 &224247408571323358
RectTransform:
  m_ObjectHideFlags: 1
  m_PrefabParentObject: {fileID: 0}
  m_PrefabInternal: {fileID: 100100000}
  m_GameObject: {fileID: 1121525888519158}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 0, y: 0, z: 0}
  m_Children:
  - {fileID: 224058974130773256}
  m_Father: {fileID: 0}
  m_RootOrder: 0
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
  m_AnchorMin: {x: 0, y: 0}
  m_AnchorMax: {x: 0, y: 0}
  m_AnchoredPosition: {x: 0, y: 0}
  m_SizeDelta: {x: 0, y: 0}
  m_Pivot: {x: 0, y: 0}
```

