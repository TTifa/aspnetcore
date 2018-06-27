using Senparc.Weixin.Context;
using Senparc.Weixin.MP.Entities;

namespace apicore.Handlers
{
    public class CustomMessageContext : MessageContext<IRequestMessageBase, IResponseMessageBase>
    {
        public CustomMessageContext()
        {

        }
    }
}