//-----------------------------------------------------------------------
// <copyright file="ThreadUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public enum TaskState
    {
        Waiting,
        Running,
        Done
    }

    //// / <summary>
    //// / Meant to be run on the UI thread.  You call Start to have the BackgroundAction be run.  You
    //// / then wait around till the State == Done is true.  Then you call Finish so the OnCompleteAction
    //// / is run (which isn't run on a background thread).
    //// / </summary>
    //// public class Task
    //// {
    ////     public Action BackgroundAction { get; private set; }
    ////     public Action<Exception> OnCompleteAction { get; private set; }
    ////     public TaskState State { get; private set; }
    ////     public Exception ErrorException { get; private set; }
    //// 
    ////     /// <summary>
    ////     /// 
    ////     /// </summary>
    ////     /// <param name="backgroundAction"></param>
    ////     /// <param name="onCompleteAction"></param>
    ////     public Task(Action backgroundAction, Action<Exception> onCompleteAction)
    ////     {
    ////         if (backgroundAction == null)
    ////         {
    ////             throw new ArgumentNullException("backgroundAction can not be null");
    ////         }
    //// 
    ////         BackgroundAction = backgroundAction;
    ////         OnCompleteAction = onCompleteAction;
    ////         State = TaskState.Waiting;
    ////         ErrorException = null;
    ////     }
    //// 
    ////     /// <summary>
    ////     /// 
    ////     /// </summary>
    ////     public void Start()
    ////     {
    ////         this.State = TaskState.Running;
    //// 
    ////         new Thread(new ThreadStart(() =>
    ////             {
    ////                 try
    ////                 {
    ////                     this.BackgroundAction.Invoke();
    ////                 }
    ////                 catch (Exception ex)
    ////                 {
    ////                     this.ErrorException = ex;
    ////                 }
    //// 
    ////                 this.State = TaskState.Done;
    ////             })).Start();
    ////     }
    //// 
    ////     /// <summary>
    ////     /// 
    ////     /// </summary>
    ////     public void Finish()
    ////     {
    ////         if (this.OnCompleteAction != null)
    ////         {
    ////             this.OnCompleteAction.Invoke(this.ErrorException);
    ////         }
    ////     }
    //// }
    
    public static class ThreadUtil
    {
        //// public static Task StartNewTask(Action backgroundAction)
        //// {
        ////     Task t = new Task(backgroundAction, null);
        ////     t.Start();
        ////     return t;
        //// }
        
        //// public static Task StartNewTask(Action backgroundAction, Action<Exception> onCompleteAction)
        //// {
        ////     Task t = new Task(backgroundAction, onCompleteAction);
        ////     t.Start();
        ////     return t;
        //// }
        
        public static void SleepInMillis(int milliSeconds)
        {
            Platform.Instance.Sleep(milliSeconds);
        }
        
        public static void SleepInSeconds(float seconds)
        {
            SleepInMillis((int)(seconds * 1000f));
        }
    }
}
