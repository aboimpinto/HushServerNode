using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HushServerNode.Services.NodeSettings
{
    public interface INodeSettingsService
    {
        NodeSettingsInfo GetNodeSettingsInfo();

        NodeStakerInfo GetNodeStakerInfo();
    }
}