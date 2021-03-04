using System;

namespace MyTaskSystem
{
    public delegate void DelOnTaskFinish();
    public delegate bool DelCheckCanSkip(ITask task);
    public interface ITask
    {
        void Execute(DelOnTaskFinish onTaskFinish);
        bool Skip(DelCheckCanSkip canSkipCheck);
        void GiveUp();
    }
}
