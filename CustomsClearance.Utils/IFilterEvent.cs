namespace CustomsClearance.Utils
{
    public interface IFilterEvent
    {
         string Url { get; set; }

        void Execute();


    }
}