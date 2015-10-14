using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ServedWhiteNoodlesFlowingInSmallFlume.RandomProvider;
using System.Reactive.Linq;
using System.IO;
using static System.Console;

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
            ReadKey();
        }
    }


    class Server : IDisposable
    {
        System.Timers.Timer timer = new System.Timers.Timer();
        internal Server() : this(15 * 1000) { }

        internal Server(double interval)
        {
            timer.Interval = interval;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) => RaiseServed(new NoodleServeEventArg());

        internal event EventHandler<NoodleServeEventArg> Served;
        protected virtual void RaiseServed(NoodleServeEventArg e) => Served?.Invoke(this, e);

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if(disposing)
                {
                    timer?.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
    class NoodleServeEventArg : EventArgs
    {
        public IReadOnlyList<WhiteNoodle> Noodles { get; } = Enumerable.Repeat(new WhiteNoodle(), GetThreadRandom().Next(75, 151)).ToArray();
        internal NoodleServeEventArg()
        {
            WriteLine($"そうめんを{ToString()}供給しました。");
        }

        public override string ToString() => $"{Noodles?.Sum(noodle => noodle.Weight) ?? 0:0.0}g";
    }

    class WhiteNoodle
    {
        internal double Weight { get; } = GetThreadRandom().Next(20, 36) / 100D;
        public override string ToString() => $"{Weight}g";

    }

    class Guest
    {
        internal string Name { get; }
        internal double Capacity { get; } = 270 * (GetThreadRandom().NextDouble() + 0.8);
        internal bool IsSatiety => Capacity <= AmountOfAte;
        private double AmountOfAte { get; set; }

        internal Guest() : this(nameof(Guest)) { }
        internal Guest(string name) { Name = name; }

        internal int Picking(IReadOnlyList<WhiteNoodle> noodles)
        {
            if(IsSatiety || GetThreadRandom().Next() % 7 == 0) return 0;
            return GetThreadRandom().Next(noodles.Count + 1);

        }

        internal void Eat(IEnumerable<WhiteNoodle> noodles)
        {
            var amountOfPick = noodles?.Sum(noodle => noodle.Weight) ?? 0;
            if(amountOfPick < 1) return;

            AmountOfAte += amountOfPick;
            WriteLine($"\t{Name}さんがそうめんを{amountOfPick:0.0}g食べました。");
        }
    }

    public static class RandomProvider
    {
        private static ThreadLocal<Random> randomWrapper = new ThreadLocal<Random>(() =>
        {
            using(var rng = new RNGCryptoServiceProvider())
            {
                var buffer = new byte[sizeof(int)];
                rng.GetBytes(buffer);
                var seed = BitConverter.ToInt32(buffer, 0);
                return new Random(seed);
            }
        });

        public static Random GetThreadRandom() => randomWrapper.Value;
    }
}


