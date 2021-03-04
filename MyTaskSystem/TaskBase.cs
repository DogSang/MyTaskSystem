
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

        protected virtual void OnStart() { }
        protected virtual void OnGiveUp() { }
        protected virtual void OnSkip() { }
        protected virtual void OnFinish() { }
        protected virtual void OnLeave() { }
    }
}
