using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServedWhiteNoodlesFlowingInSmallFlumeLibraries;
using static ServedWhiteNoodlesFlowingInSmallFlumeLibraries.RandomProvider;
using System.Reactive.Linq;
using System.IO;
using static System.Console;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ServedWhiteNoodlesFlowingInSmallFlume
{
    class Program
    {
        static void Main(string[] args)
        {
            var guests = Enumerable.Range(1, GetThreadRandom().Next(1, 11)).Select(i => new Guest($"ゲスト{i}")).ToArray();
            var sv = new Server(1000);
            Observable
                    .FromEvent<EventHandler<NoodleServeEventArg>, NoodleServeEventArg>(h => (sender, e) => h(e), h => sv.Served += h, h => sv.Served -= h)
                    .TakeWhile(_ => guests.Any(guest => !guest.IsSatiety))
                    .Subscribe(e => guests.Aggregate(e.Noodles, (noodles, guest) =>
                                                                {
                                                                    var pickedCount = guest.Picking(noodles);
                                                                    guest.Eat(noodles.Take(pickedCount));
                                                                    return noodles.Skip(pickedCount).ToArray();
                                                                }),
                               () =>
                               {
                                   WriteLine("全員満腹になりました。");
                                   sv.Dispose();
                               });

        }
    }
}


