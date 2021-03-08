using System.Threading;

namespace MyTaskSystem
{
    public class TaskWait : TaskBase
    {
        protected float fWaitTime=0;

        /// <summary>
        /// 创建一个等待事件
        /// </summary>
        /// <param name="waitTime">等待时间，单位秒</param>
        public TaskWait(float waitTime)
        {
            fWaitTime = waitTime;
        }

        protected override void OnStart()
        {
            base.OnStart();

            OnStartWait();
        }

        protected override void OnSkip()
        {
            waitThread?.Abort();
            base.OnSkip();
        }
        protected override void OnGiveUp()
        {
            waitThread?.Abort();
            base.OnGiveUp();
        }

        private Thread waitThread;
        protected virtual void  OnStartWait()
        {
            waitThread= new Thread(new ThreadStart(Wait));
            waitThread.Start();
        }

        private delegate void DelOnWaitOver();
        private void Wait()
        {
            try
            {
                Thread.Sleep((int)fWaitTime * 1000);
                OnWaitTimeOver();
            }
            catch (ThreadAbortException)
            { }
        }

        protected virtual void OnWaitTimeOver()
        {
            Finsih();
        }
    }
}
