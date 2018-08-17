using System;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace CustomsClearance.Utils
{
    public class BaseFilterEvent : IFilterEvent
    {
        private readonly Messager<HttpMessage> _messager;

        public BaseFilterEvent()
        {
            _messager = Messagers.Default[MessageNames.HttpMessage] as Messager<HttpMessage>;
        }

        public string Url { get; set; }

        public async Task Execute(SessionEventArgs e)
        {
            string bodyString = await e.GetRequestBodyAsString();
            var request = e.WebSession.Request;
            await Task.Run(() =>
            {
                
                string msg = $"网址:{e.WebSession.Request.Url}" +
                             $"\r\n协议：{(e.WebSession.IsHttps ? "Https" : "http")}" +
                             $"\r\nMethod：{e.WebSession.Request.Method}" +
                             $"\r\n PostData：{bodyString}\r\n\r\n";
                Console.WriteLine(msg);
                Send(msg);
                Console.WriteLine("-------------- Headers------------------》");
                Send("-------------- Headers------------------》");
                foreach (var item in request.Headers)
                {
                    var header = $"Name:{item.Name},Value:{item.Value}";
                    Console.WriteLine(header);

                    Send(header);
                }
                Send("----------------- Headers---------------》");
                Console.WriteLine("----------------- Headers---------------》");
            });
        }


        public void Send(string msg)
        {
            var httpMessage = new HttpMessage();
            httpMessage.OnCreated = DateTime.Now;
            httpMessage.Body = msg;
            _messager.Send(httpMessage);
        }
    }
}
