namespace PlayerCharacter
{
    public enum MesssageType
    {
        Damage,Death,Respawn
    }
    public interface IMessageReceiver//接口
    {
        void OnMessageReceive(MesssageType type, object sender, object obj);
    }
    
}
