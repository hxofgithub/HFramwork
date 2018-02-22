namespace HFramwork
{
    public interface IMessage
    {
        
    }


    public class GameMessage<T> where T : IMessage
    {
        public delegate void GameMesseageHandler(T arg);

        public static event GameMesseageHandler msgHandler;

        public static void Dispatch(T arg)
        {
            if (msgHandler != null)
                msgHandler(arg);
        }
    }
}
