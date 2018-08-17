using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace CustomsClearance.Utils
{
    public interface IFilterEvent
    {
         string Url { get; set; }

        Task Execute(SessionEventArgs e);


    }
}