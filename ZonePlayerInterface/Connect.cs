using Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ZonePlayerInterface
{
    /// <summary>
    /// Implementation of <see cref="Connect"/> to allow remote interface to connect to ZonePlayer
    /// </summary>    
    public class Connect<T>
    {
        /// <summary>
        /// Get client to connect to ZonePlayer interface
        /// </summary>
        /// <param name="argCommand"></param>
        /// <returns>The command converted into <see cref="Commands"/>. Null if commabd is not recognized</returns>
        public static T GetClient(Uri remoteService)
        {
            Checks.NotNull("remoteService", remoteService);

            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress(remoteService);
            var channelFactory = new ChannelFactory<T>(binding, endpoint);

            T client = default(T);

            try
            {
                client = channelFactory.CreateChannel();
            }
            catch
            {
                if (client != null)
                {
                    ((ICommunicationObject)client).Abort();
                }
            }

            return client;
        }
    }
}
