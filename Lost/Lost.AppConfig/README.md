# Lost.AppConfig Package

### Description
----------------
This package contains the Lost Signal AppConfig system.  It's a simple system for creating configuration files that you can switch between (like Dev and Prod).

### To Do
----------
* Remove as many Helper classes as I can.
  * EditorGUILayout.HorizontalScope will replace BeginHorizontalHelper
  * EditorGUILayout.VerticalScope will replace BeginVerticalHelper
  * Look for more
  
* Put all files in the Lost.AppConfig namespace

### Dependencies
-----------------
No Dependencies

### Bugs
* If you have a PlayFab config in Root, then child configs (Dev/Live) can't create a PlayFab config to override it.

