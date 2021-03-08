using System.Collections.Generic;

namespace MyTaskSystem
{
    public class TaskFactory : ITask,ITaskFactory
    {
        private enum EMTaskType
        {
            /// <summary>
            /// 单个
            /// </summary>
            Normal,
            /// <summary>
            /// 队列
            /// </summary>
            Queue,
            /// <summary>
            /// 并行
            /// </summary>
            Parallel,
        }
        private class STTaskData :ITaskFactory
        {
            public EMTaskType eMTaskType;
            public List<STTaskData> listTaskData;
            public ITask task;

            public ITask GetTask()
            {
                ITask tmpTask=null;
                switch (eMTaskType)
                {
                    case EMTaskType.Normal:
                        tmpTask=task;
                        break;
                    case EMTaskType.Queue:
                    case EMTaskType.Parallel:
                        List<ITask> listTmp = new List<ITask>();
                        listTaskData?.ForEach((data) => listTmp.Add(data.GetTask()));

                        if (eMTaskType == EMTaskType.Queue)
                            tmpTask = new TaskQueue(listTmp.ToArray());
                        else if (eMTaskType == EMTaskType.Parallel)
                            tmpTask = new TaskParallel(listTmp.ToArray());
                        break;
                }

                return tmpTask;
            }
        }

        private List<STTaskData> listCtrlTaskData;
        /// <summary>
        /// 正在操作的任务信息
        /// </summary>
        private STTaskData CurCtrlTaskData => listCtrlTaskData[listCtrlTaskData.Count - 1];

        private ITask _curTask;
        protected ITask CurTask
        {
            get
            {
                if (_curTask == null)
                    _curTask = GetTask();
                return _curTask;
            }
        }

        public TaskFactory()
        {
            listCtrlTaskData = new List<STTaskData>();
            //开局默认给个队列
            listCtrlTaskData.Add(new STTaskData() { eMTaskType = EMTaskType.Queue });
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(ITask task)
        {
            System.Console.WriteLine(CurCtrlTaskData.eMTaskType);

            if (CurCtrlTaskData.listTaskData == null)
                CurCtrlTaskData.listTaskData = new List<STTaskData>();

            CurCtrlTaskData.listTaskData.Add(new STTaskData()
            {
                eMTaskType=EMTaskType.Normal,
                task=task,
            });
        }
        private void AddTaskData(STTaskData taskData)
        {
            if (CurCtrlTaskData.listTaskData == null)
                CurCtrlTaskData.listTaskData = new List<STTaskData>();
            CurCtrlTaskData.listTaskData.Add(taskData);
        }

        public void StartAddQueueTask()
        {
            //已经是队列模式了
            if (CurCtrlTaskData.eMTaskType == EMTaskType.Queue) return;

            //添加一个队列的taskData用来添加新task
            STTaskData taskData = new STTaskData() { eMTaskType = EMTaskType.Queue };
            AddTaskData(taskData);
            //加入列表，作为当前的操作对象
            listCtrlTaskData.Add(taskData);
        }
        public void EndAddQueueTask()
        {
            //当前不是队列模式
            if (CurCtrlTaskData.eMTaskType != EMTaskType.Queue) return;
            //最后一个队列不允许删除
            if (listCtrlTaskData.Count <= 1) return;

            //剔除当前操作的对象，改为上一个对象
            listCtrlTaskData.RemoveAt(listCtrlTaskData.Count - 1);
        }
        public void StartAddParallelTask()
        {
            //已经是并行模式了
            if (CurCtrlTaskData.eMTaskType == EMTaskType.Parallel) return;

            //添加一个队列的taskData用来添加新task
            STTaskData taskData = new STTaskData() { eMTaskType = EMTaskType.Parallel};
            AddTaskData(taskData);
            //加入列表，作为当前的操作对象
            listCtrlTaskData.Add(taskData);
        }
        public void EndAddParallelTask()
        {
            //当前不是队列模式
            if (CurCtrlTaskData.eMTaskType != EMTaskType.Parallel) return;

            //剔除当前操作的对象，改为上一个对象
            listCtrlTaskData.RemoveAt(listCtrlTaskData.Count - 1);
        }

        #region 实现接口
        public ITask GetTask()
        {
            //取第一个，后续的递归
            if (listCtrlTaskData != null && listCtrlTaskData.Count > 0)
                return listCtrlTaskData[0]?.GetTask();
            else
                return null;
        }

        public void Execute(DelOnTaskFinish onTaskFinish,TaskEventData eventData=null)
        {
            if (CurTask == null)
                onTaskFinish?.Invoke();
            else
                CurTask?.Execute(onTaskFinish,eventData);
        }
        public void GiveUp()
        {
            CurTask?.GiveUp();
        }

        public bool Skip(DelCheckCanSkip canSkipCheck)
        {
            return CurTask?.Skip(canSkipCheck) ?? true;
        }
        #endregion
    }
}
