using System;

namespace MyTaskSystem
{
    public static class TaskErrorMgr
    {
        public enum EMTaskErrorType
        {
           None,
           /// <summary>
           /// 开启了一个正在执行中的任务
           /// </summary>
           StartWhenOnTask,
           /// <summary>
           /// 任务参数错误
           /// </summary>
           TaskArgumentError,
        }
        public struct STTaskErrorData
        {
            /// <summary>
            /// 出错的任务
            /// </summary>
            public ITask task;
            /// <summary>
            /// 错误的类型
            /// </summary>
            public EMTaskErrorType eMTaskErrorType;
            /// <summary>
            /// 错误的描述
            /// </summary>
            public string strErrorDes;
        }

        public delegate void DelOnTaskError(STTaskErrorData data);
        /// <summary>
        /// taskSystem报错时的事件
        /// </summary>
        public static event DelOnTaskError EventOnTaskError;
        /// <summary>
        /// 发送报错信息的接口
        /// </summary>
        /// <param name="task"></param>
        /// <param name="errorType"></param>
        /// <param name="errorDes"></param>
        internal static void SendTaskErrorData(ITask task, EMTaskErrorType errorType, string errorDes)
        {
            try
            {
                EventOnTaskError?.Invoke(new STTaskErrorData()
                {
                    task = task,
                    eMTaskErrorType = errorType,
                    strErrorDes = errorDes,
                });
            }
            catch (Exception) { }
        }
    }
}
