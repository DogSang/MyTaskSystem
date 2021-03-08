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

    public class TaskEventData
    {
        /// <summary>
        /// 任务开始后触发的事件
        /// </summary>
        public event DelOnTaskEvent EventOnTaskStart;
        /// <summary>
        /// 任务被放弃时触发的事件
        /// </summary>
        public event DelOnTaskEvent EventOnTaskGiveUp;
        /// <summary>
        /// 任务被跳过后触发的事件
        /// </summary>
        public event DelOnTaskEvent EventOnTaskSkip;
        /// <summary>
        /// 任务完成后触发的事件
        /// </summary>
        public event DelOnTaskEvent EventOnTaskFinish;
        /// <summary>
        /// 任务离开触发，包括放弃、跳过、完成
        /// </summary>
        public event DelOnTaskEvent EventOnTaskLeave;

        public enum EMEventType
        {
            None,
            OnStart,
            OnGiveUp,
            OnSkip,
            OnFinish,
            OnLeave,
        }

        public void OnEvent(ITask task, EMEventType eventType)
        {
            if (task == null) return;

            switch (eventType)
            {
                case EMEventType.OnStart:
                    EventOnTaskStart?.Invoke(task);
                    break;
                case EMEventType.OnGiveUp:
                    EventOnTaskGiveUp?.Invoke(task);
                    break;
                case EMEventType.OnSkip:
                    EventOnTaskSkip?.Invoke(task);
                    break;
                case EMEventType.OnFinish:
                    EventOnTaskFinish?.Invoke(task);
                    break;
                case EMEventType.OnLeave:
                    EventOnTaskLeave?.Invoke(task);
                    break;
            }
        }

        public static TaskEventData operator +(TaskEventData dataA, TaskEventData dataB)
        {
            TaskEventData eventData = new TaskEventData();
            if (dataA != null)
            {
                eventData.EventOnTaskStart += dataA.EventOnTaskStart;
                eventData.EventOnTaskSkip += dataA.EventOnTaskSkip;
                eventData.EventOnTaskGiveUp += dataA.EventOnTaskGiveUp;
                eventData.EventOnTaskFinish += dataA.EventOnTaskFinish;
                eventData.EventOnTaskLeave += dataA.EventOnTaskLeave;
            }
            if (dataB != null)
            {
                eventData.EventOnTaskStart += dataB.EventOnTaskStart;
                eventData.EventOnTaskSkip += dataB.EventOnTaskSkip;
                eventData.EventOnTaskGiveUp += dataB.EventOnTaskGiveUp;
                eventData.EventOnTaskFinish += dataB.EventOnTaskFinish;
                eventData.EventOnTaskLeave += dataB.EventOnTaskLeave;
            }
            return eventData;
        }
    }

    public interface ITask
    {
        void Execute(DelOnTaskFinish onTaskFinish, TaskEventData eventData = null);
        bool Skip(DelCheckCanSkip canSkipCheck);
        void GiveUp();
    }
}
