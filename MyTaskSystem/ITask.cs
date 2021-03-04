using System;

namespace MyTaskSystem
{
    /// <summary>
    /// 任务结束的委托
    /// </summary>
    public delegate void DelOnTaskFinish();
    /// <summary>
    /// 任务触发的事件的委托
    /// </summary>
    /// <param name="task"></param>
    public delegate void DelOnTaskEvent(ITask task);
   /// <summary>
   /// 检查能否跳过当前任务的检测委托
   /// </summary>
   /// <param name="task"></param>
   /// <returns></returns>
    public delegate bool DelCheckCanSkip(ITask task);

    public interface ITask
    {
        void Execute(DelOnTaskFinish onTaskFinish);
        bool Skip(DelCheckCanSkip canSkipCheck);
        void GiveUp();
    }
}
