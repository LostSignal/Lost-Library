# Lost.Analytics Package

### Description
----------------
This package wraps all of Unity's Standard Analytics calls.  The reason for doing that is so that I can provide a callback.
So whenver an analytic event is fired, I can forward it to anyone who registers as an IAnalyticsProvider.  It also allows you
to turn off/on Unity Analytics without having to comment out any code.

### To Do
----------
* Need to move the Lost Analytics AppConfig class to this project, and it needs to let you toggle Unity Analytics on/off.

### Dependencies
-----------------
Lost.AppConfig
