using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
using static ServedWhiteNoodlesFlowingInSmallFlumeLibraries.RandomProvider;

namespace ServedWhiteNoodlesFlowingInSmallFlumeLibraries
{
    public class Server : IDisposable
    {
        System.Timers.Timer timer = new System.Timers.Timer();
        public Server() : this(15 * 1000) { }

        public Server(double interval)
        {
            
            timer.Interval = interval;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

         void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) => RaiseServed(new NoodleServeEventArg());

        public event EventHandler<NoodleServeEventArg> Served;
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
    public class NoodleServeEventArg : EventArgs
    {
        public IReadOnlyList<WhiteNoodle> Noodles { get; } = Enumerable.Repeat(new WhiteNoodle(), GetThreadRandom().Next(75, 151)).ToArray();
        public NoodleServeEventArg()
        {
            WriteLine($"そうめんを{ToString()}供給しました。");
        }

        public override string ToString() => $"{Noodles?.Sum(noodle => noodle.Weight) ?? 0:0.0}g";
    }

    public class WhiteNoodle
    {
        public double Weight { get; } = GetThreadRandom().Next(20, 36) / 100D;
        public override string ToString() => $"{Weight}g";

    }

    public class Guest
    {
        public string Name { get; }
        public double Capacity { get; } = 270 * (GetThreadRandom().NextDouble() + 0.8);
        public bool IsSatiety => Capacity <= AmountOfAte;
        private double AmountOfAte { get; set; }

        public Guest() : this(nameof(Guest)) { }
        public Guest(string name) { Name = name; }

        public int Picking(IReadOnlyList<WhiteNoodle> noodles)
        {
            if(IsSatiety || GetThreadRandom().Next() % 7 == 0) return 0;
            return GetThreadRandom().Next(noodles.Count + 1);
        }

        public void Eat(IEnumerable<WhiteNoodle> noodles)
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
