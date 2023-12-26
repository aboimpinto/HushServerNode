using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Olimpo;

namespace HushServerNode.Services.NodeSettings
{
    public class NodeSettingsService : IBootstrapper, INodeSettingsService
    {
        public int Priority { get; set; } = 5;

        public NodeSettingsInfo GetNodeSettingsInfo()
        {
            throw new NotImplementedException();
        }

        public NodeStakerInfo GetNodeStakerInfo()
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public Task Startup()
        {
            throw new NotImplementedException();
        }
    }
}