
namespace MyTaskSystem
{
    public class TaskSameTime : TaskBase
    {
        //要执行的任务数组
        protected ITask[] arrTask;
        //当前等待完成任务数量
        protected int nWaitTaskCount;

        public TaskSameTime(ITask[] arrTask)
        {
            this.arrTask = arrTask;
        }

        public override bool Skip(DelCheckCanSkip canSkipCheck)
        {
            if (TaskStage != EM_TaskStage.OnTask) return true;

            //不能跳过自己
            if (canSkipCheck != null && !canSkipCheck(this)) return false;

            //可以跳过自己，需要先跳过子任务
            if (arrTask != null)
                for (int i = 0; i < arrTask.Length; i++)
                {
                    //有一个子任务不能跳过
                    if (arrTask[i] != null && !arrTask[i].Skip(canSkipCheck))
                        return false;
                }

            //全跳过
            OnSkip();
            OnLeave();

            return true;
        }

        public override void GiveUp()
        {
            if (TaskStage != EM_TaskStage.OnTask) return;

            //先放弃子任务
            if (arrTask!=null)
                for (int i = 0; i < arrTask.Length; i++)
                    arrTask[i]?.GiveUp();

            base.GiveUp();
        }

        protected override void OnStart()
        {
            if (arrTask == null || arrTask.Length == 0)
            {
                Finsih();
            }
            else
            {
                base.OnStart();

                nWaitTaskCount = arrTask.Length;

                for (int i = 0; i < arrTask.Length; i++)
                {
                    StartChildTask(arrTask[i]);
                }
            }
        }

        protected override void OnGiveUp()
        {
            if(arrTask!=null)
                for (int i = 0; i < arrTask.Length; i++)
                {
                    arrTask[i]?.GiveUp();
                }
            base.OnGiveUp();
        }

        //开启子任务
        private void StartChildTask(ITask task)
        {
            if (task == null)
                OnChildTaskFinish();
            else
                task.Execute(OnChildTaskFinish);  
        }
        //子任务完成回调
        private void OnChildTaskFinish()
        {
            nWaitTaskCount--;
            if (nWaitTaskCount <= 0)
                Finsih();
        }
    }
}
