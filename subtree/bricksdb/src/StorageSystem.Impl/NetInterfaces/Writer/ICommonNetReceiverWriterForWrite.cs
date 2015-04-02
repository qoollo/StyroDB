using System.ServiceModel;

namespace Qoollo.Impl.NetInterfaces.Writer
{
    [ServiceContract]
    public interface ICommonNetReceiverWriterForWrite : IRemoteNet, ICommonCommunicationNet
    {
    }
}
