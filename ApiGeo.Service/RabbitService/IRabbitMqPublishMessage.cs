using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGeo.Service.RabbitService
{
    public interface IRabbitMqPublishMessage
    {
        Task PublishMessage<T>(T messageEntity) where T : class;
    }
}
