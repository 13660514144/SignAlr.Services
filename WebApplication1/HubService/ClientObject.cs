using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.HubService
{
    public class ClientObject
    {
        public List<CollectionClient> _CollectionClient;
        public ClientObject()
        {
            _CollectionClient = new List<CollectionClient>();
        }
        public async Task<CollectionClient> AddModel(string  ConnectId)
        {
            CollectionClient O = new CollectionClient();
            O.ConnectId = ConnectId;
            _CollectionClient.Add(O);
            return O;
        }
        public async Task<bool> UpConnectObj(CollectionClient Model,string ConnectId)
        {
            var found = _CollectionClient.Find((CollectionClient p) => p.ConnectId == ConnectId);            
            if (found != null)
            {
                found.ClientType = Model.ClientType;
                found.ClientIp = Model.ClientIp;
                found.ClientPort = Model.ClientPort;
                found.ServerHost = Model.ServerHost;
                found.ServerPort = Model.ServerPort;
                found.GUID = Model.GUID;
                found.ToKen = Model.ToKen;
                return true;
            }
            return false;
        }
        public async Task<bool> DelModel(string ConnectId)
        {
            var o = _CollectionClient.Find(P => P.ConnectId == ConnectId);
            if (o != null)
            {
                _CollectionClient.Remove(o);
                return true;
            }
            return false;
        }
        public class CollectionClient
        {
            public string ConnectId { get; set; } = string.Empty;
            public string ClientType { get; set; } = string.Empty;
            public string ClientIp { get; set; } = string.Empty;
            public string ClientPort { get; set; } = string.Empty;
            public string ServerHost { get; set; } = string.Empty;
            public string ServerPort { get; set; } = string.Empty;
            public string ToKen { get; set; } = string.Empty;
            public string GUID { get; set; } = string.Empty;
        }
    }
}
