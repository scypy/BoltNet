namespace BoltNET.Messages
{
    public interface ISerializableMessage<T>
    {
        T DeSerialize(Message message);
        Message Serialize();
    }
}
