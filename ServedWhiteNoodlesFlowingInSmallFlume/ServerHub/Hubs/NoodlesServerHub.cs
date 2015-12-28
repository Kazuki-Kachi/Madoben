using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using ServedWhiteNoodlesFlowingInSmallFlumeLibraries;
using System.Reactive.Linq;
using System.Threading;
using Newtonsoft.Json;
using System.Diagnostics;
using static ServedWhiteNoodlesFlowingInSmallFlumeLibraries.RandomProvider;
namespace ServerHub.Hubs
{
    public class NoodlesServerHub : Hub
    {
        /// <summary>
        /// <para>クライアント管路用</para>
        /// <para>順番に到達させるために使用する</para>
        /// </summary>
        volatile static List<string> clientIds = new List<string>();
        /// <summary>
        /// サーバのコネクションId
        /// </summary>
        volatile static string serverId;
        public override Task OnConnected()
        {
            clientIds.Add(Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if(Context.ConnectionId == serverId)
                serverId = null;
            else
                clientIds.Remove(Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            clientIds.Add(Context.ConnectionId);
            return base.OnReconnected();
        }

        /// <summary>
        /// サーバ接続後の初期設定
        /// </summary>
        public void SetupServer()
        {
            serverId = Context.ConnectionId;
            clientIds.Remove(Context.ConnectionId);
        }

        /// <summary>
        /// 供給する
        /// </summary>
        /// <param name="type">供給する <see cref="INoodle"/>の型</param>
        /// <remarks>復元がうまくいかなかったため、ここでインスタンスを作成する</remarks>
        public void Serve(Type type)
        {

            var noodle = Activator.CreateInstance(type) as INoodle;
            if(noodle == null) return;
            var noodles = Enumerable.Repeat(noodle, GetThreadRandom().Next(75, 151)).ToArray();
            
            Clients.Client(serverId).info($"{noodle.Name}を{noodles.Sum(n => n.Weight):0.0}g供給しました。");

            Debug.WriteLine(noodles.First().GetType());
            
            foreach(var id in clientIds)
            {
                Clients.Client(id).serve(JsonConvert.SerializeObject(noodles), type);
                Thread.Sleep(7000);
                noodles = noodles.Skip(pick).ToArray();
            }

            Clients.Client(serverId).completed();
        }

        static int pick;
        public void Picking(int picked) => pick = picked;

     }
}