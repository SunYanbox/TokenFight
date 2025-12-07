namespace TokenFight.Core.Interfaces.Bases;

/// <summary>
/// 可序列化的接口
/// </summary>
public interface ISerializable<out T>
{
    /// <summary>
    /// 将自己序列化为字符串
    /// </summary>
    string ToJson();
    /// <summary>
    /// 将字符串序列化为新实例
    /// </summary>
    T FromJson(string json);
    /// <summary>
    /// 将自己序列化为二进制数据
    /// </summary>
    byte[] ToBytes();
    /// <summary>
    /// 将二进制数据序列化为新实例
    /// </summary>
    T FromBytes(byte[] data);
}