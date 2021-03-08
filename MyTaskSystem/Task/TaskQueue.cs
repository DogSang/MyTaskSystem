
namespace MyTaskSystem
{
    public class TaskQueue : TaskBase
    {
        //要执行的任务数组
        protected ITask[] arrTask;
        //当前正在执行的任务
        protected int nCurTaskIndex;

        public TaskQueue(ITask[] arrTask)
        {
            this.arrTask = arrTask;
        }

        public override bool Skip(DelCheckCanSkip canSkipCheck)
        {
            if (TaskStage != EM_TaskStage.OnTask) return true;

            //不能跳过自己
            if (canSkipCheck != null && !canSkipCheck(this)) return false;

            //可以跳过自己，需要先跳过子任务
            //队列中，先前已经完成的任务stage不是OnTask，直接return true
            //当前执行中的任务跳过后会调用完成回调OnChildTaskFinish，开启后面的任务
            //所以每次for循环一次，当前任务也会跟着切换，直到无法跳过，或者结束
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

            //只需要处理当前在执行的任务就可以了
            if (arrTask != null && nCurTaskIndex >= 0 && nCurTaskIndex < arrTask.Length)
                arrTask[nCurTaskIndex]?.GiveUp();

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

                StartChildTask(0);
            }
        }

        //开启子任务
        private void StartChildTask(int index)
        {
            nCurTaskIndex = index;
            //理论上不会出现-1或者超过size的情况
            ITask task = arrTask[nCurTaskIndex];
            if (task == null)
                OnChildTaskFinish();
            else
                task.Execute(OnChildTaskFinish);
        }
        //子任务完成回调
        private void OnChildTaskFinish()
        {
            if (nCurTaskIndex >= arrTask.Length-1)
                Finsih();
            else
            {
                nCurTaskIndex++;
                StartChildTask(nCurTaskIndex);
            }
        }
    }
}
