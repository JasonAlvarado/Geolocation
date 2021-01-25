using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationHelper.RabbitMq
{
    public interface IRabbitMqHelper
    {
        Task PublishMessage<T>(T messageEntity) where T : class;
    }
}
