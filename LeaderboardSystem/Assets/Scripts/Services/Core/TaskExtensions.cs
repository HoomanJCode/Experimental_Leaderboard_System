using System.Threading.Tasks;
using UnityEngine;

namespace Services
{
    public static class TaskExtensions
    {
        public static WaitUntil UntileComplete(this Task task)
        {
            return new WaitUntil(() => task.IsCompleted);
        }
    }
}