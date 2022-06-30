using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using static WebApplication1.HubService.ClientObject;

namespace WebApplication1.HubService
{
    public class HubMessage : Hub
    {       
        private ClientObject _ClientObject;
        public HubMessage(ClientObject clientObject)
        {
            _ClientObject = clientObject;
        }
        /// <summary>
        /// 客户端连接请求
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"建立连接{Context.ConnectionId}");                        
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
            var o = await _ClientObject.AddModel(Context.ConnectionId);
            //await Task.Delay(50);
            //RegisterClient(Context.ConnectionId, o);
        }
        /// <summary>
        /// 建立连接后对指定客户端 推送 连接ID 和 token /guid
        /// </summary>
        /// <param name="obj"></param>
        private async void RegisterClient(string ConnectId, CollectionClient obj)
        {
            await Clients.Client(ConnectId).SendAsync("RegisterToken", obj);
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"断开连接{Context.ConnectionId}");
            var flg = await _ClientObject.DelModel(Context.ConnectionId);
            if (flg == true)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
                await base.OnDisconnectedAsync(exception);
            }
            Console.WriteLine(JsonConvert.SerializeObject(_ClientObject._CollectionClient));
        }

        public async Task sendmessage(string name, string message)
        {
            Console.WriteLine($"用户名称：{name},收到消息：{message}");
            //await Task.Delay(50);
            await Clients.All.SendAsync("receivemessage", $"{name}", $"{ message}");           
        }
        /// <summary>
        /// 服务端方法返利
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public async Task<string> GetSample(string str)
        {
            var n = new
            {
                aa=20,
                bb=30,
                cc=40
            };
            return JsonConvert.SerializeObject(n);
        }
        /// <summary>
        /// 心跳
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public async Task<bool> HeartFlg(CollectionClient Obj)
        {
            return true;
        }
        /// <summary>
        /// 更新客户端信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<bool> UpConnectObj(CollectionClient obj)
        {
            Console.WriteLine(JsonConvert.SerializeObject(obj));
            var flg = await _ClientObject.UpConnectObj(obj, Context.ConnectionId);
            Console.WriteLine($"list=>{JsonConvert.SerializeObject(_ClientObject._CollectionClient)}");
            return flg;
        }
        
    }

}
