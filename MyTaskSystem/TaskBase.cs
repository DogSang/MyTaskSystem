
namespace MyTaskSystem
{
    public enum EM_TaskStage
    {
        Idle,
        OnTask,
        GiveUp,
        Finish,
    }

    public class TaskBase : ITask
    {
        public EM_TaskStage TaskStage { get; private set; }

        protected DelOnTaskFinish actionOnTaskFinish;

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

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="onTaskFinish">任务完成回调</param>
        public void Execute(DelOnTaskFinish onTaskFinish)
        {
            if (TaskStage == EM_TaskStage.OnTask)
            {
                TaskErrorMgr.SendTaskErrorData(this, TaskErrorMgr.EMTaskErrorType.StartWhenOnTask, null);
                return;
            }

            actionOnTaskFinish = onTaskFinish;

            OnStart();
        }

        /// <summary>
        /// 放弃任务
        /// </summary>
        public virtual void GiveUp()
        {
            if (TaskStage != EM_TaskStage.OnTask) return;
            TaskStage = EM_TaskStage.GiveUp;

            OnGiveUp();
            OnLeave();
        }

        /// <summary>
        /// 跳过任务
        /// </summary>
        /// <param name="canSkipCheck">检查能不能跳过的委托</param>
        /// <returns></returns>
        public virtual bool Skip(DelCheckCanSkip canSkipCheck)
        {
            if (TaskStage != EM_TaskStage.OnTask) return true;

            if (canSkipCheck == null || canSkipCheck(this))
            {
                //判定通过
                OnSkip();
                OnLeave();
                return true;
            }
            else
                return false;
        }

        protected void Finsih()
        {
            TaskStage = EM_TaskStage.Finish;

            OnFinish();
            OnLeave();

            actionOnTaskFinish?.Invoke();
        }

        protected virtual void OnStart()
        {
            EventOnTaskStart?.Invoke(this);
        }
        protected virtual void OnGiveUp()
        {
            EventOnTaskGiveUp?.Invoke(this);
        }
        protected virtual void OnSkip()
        {
            EventOnTaskSkip?.Invoke(this);
        }
        protected virtual void OnFinish()
        {
            EventOnTaskFinish?.Invoke(this);
        }
        protected virtual void OnLeave()
        {
            EventOnTaskLeave?.Invoke(this);
        }
    }
}
