namespace Core
{
    public interface ITimeAction
    {
        bool IsEnable(float currentTime);
        void Trigger(float currentTime);
    }
}