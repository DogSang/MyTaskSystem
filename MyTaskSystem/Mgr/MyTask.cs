using System;
using System.Collections.Generic;
using System.Text;

namespace MyTaskSystem
{
    public static class MyTask
    {
        public static ITask WaitTask(float waitTime)
        {
            return new TaskWait(waitTime);
        }
    }
}
