using System;
using System.Runtime.Serialization;


/// <summary>
/// Chaser例外クラスの基底
/// </summary>
[Serializable()]
public class ChaserException : System.Exception
{
    public ChaserException()
        : base()
    {
    }

    public ChaserException(string message)
        : base(message)
    {
    }
    public ChaserException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    protected ChaserException(SerializationInfo info, StreamingContext context)
    : base(info, context)
{
}

}

/// <summary>
/// Clientの不適切なメッセージフォーマットによる例外
/// </summary>
[Serializable()]
public class ClientMessageFormatException : ChaserException
{
    public ClientMessageFormatException()
        : base()
    {
    }

    public ClientMessageFormatException(string message)
        : base(message)
    {
    }

    public ClientMessageFormatException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected ClientMessageFormatException(SerializationInfo info, StreamingContext context)
    : base(info, context)
    {
    }
}

/// <summary>
/// Clientとの通信に関する例外
/// </summary>

[Serializable()]
public class NetworkErrorException: ChaserException{
        public NetworkErrorException()
        : base()
    {
    }

    public NetworkErrorException(string message)
        : base(message)
    {
    }

    public NetworkErrorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected NetworkErrorException(SerializationInfo info, StreamingContext context)
    : base(info, context)
    {
    }
}