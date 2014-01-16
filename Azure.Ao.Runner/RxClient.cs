using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Ao.Runner
{
    public class RxClient : IDisposable
    {
        private TcpClient _client;
        private bool _disposed;
        private readonly BlockingCollection<byte> _receiver = new BlockingCollection<byte>();
        private IDisposable _readSubscription;
        private readonly ISubject<Unit> _receiverTermination = new Subject<Unit>();
        private readonly object _synchronizer = new object();
        public IObservable<byte> Buffer { get; private set; }

        public RxClient()
        {
            Buffer = _receiver.GetConsumingEnumerable()
                .ToObservable(TaskPoolScheduler.Default)
                .TakeUntil(_receiverTermination);
        }

        public async Task ConnectAsync(string hostname, int port)
        {
            var client = new TcpClient { NoDelay = true };
            var hosts = await Dns.GetHostAddressesAsync(hostname);
            await client.ConnectAsync(hosts, port)
                .ContinueWith(t => _client = client, TaskContinuationOptions.OnlyOnRanToCompletion)
                .ContinueWith(t => BeginReadAsync(), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private void BeginReadAsync()
        {
            var stream = _client.GetStream();
            _readSubscription = Observable.Defer(async () =>
            {
                var buffer = new byte[1024];
                var readBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                return buffer.Take(readBytes).ToObservable();
            })
                .Repeat()
                .Subscribe(x => _receiver.Add(x), ex => Trace.TraceError(ex.Message),
                    () => Console.WriteLine("Subscription ended"));
        }

        public Task SendAsync(byte[] data)
        {
            Console.WriteLine("Sending data");
            var stream = _client.GetStream();

            try
            {
                Monitor.Enter(_synchronizer);
                return stream.WriteAsync(data, 0, data.Length);
            }
            finally
            {
                Monitor.Exit(_synchronizer);
                Console.WriteLine("Finished sending data");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                if (_readSubscription != null)
                    _readSubscription.Dispose();
                //_receiver.Dispose();
                _receiverTermination.OnNext(new Unit());
                if (_client != null)
                {
                    _client.Close();
                    (_client as IDisposable).Dispose();
                }
            }
            _disposed = true;
        }

        ~RxClient()
        {
            Dispose(false);
        }
    }
}
