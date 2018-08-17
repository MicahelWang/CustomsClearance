using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomsClearance.Utils
{

    public interface IMessager
    {

    }

    public class Messager<T>: IMessager
    {
        public Messager()
        {
            Thread vThread = new Thread(Notification) {Name = "Message"};
            vThread.IsBackground = true;
            vThread.Start(); //开始执行线程
        }
        public Action<T> ReciveHandler { get; set; }
        private readonly Queue _queue = new Queue();
        public void Send(T source) {
            _queue.Enqueue(source);
        }

        public void Notification()
        {
           
            while (true)
            {
                if (_queue.Count == 0)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    Excute();
                }
            }
        }

        private void Excute() {

            var message = (T)_queue.Dequeue();
            ReciveHandler?.Invoke(message);
        }



    }
}
