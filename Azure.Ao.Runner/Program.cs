using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Ao.Runner
{
    class Program
    {

        private static readonly RxClient Client = new RxClient();
        static void Main(string[] args)
        {
            var q = (from headerBytes in Client.Buffer.Buffer(4)
                     let header = new Header(headerBytes.ToArray())
                     let bodyBytes = Client.Buffer.Take(header.Length)
                     let packetFactory = new PacketFactory()
                     let messageFactory = packetFactory.Create(header.PacketType)
                     select messageFactory.Create(bodyBytes.ToEnumerable().ToArray()));
            var shared = q.ObserveOn(TaskPoolScheduler.Default).Publish();

            var login = shared.Where(m => m is LoginSeed)
                .Select(m => m as LoginSeed)
                .Subscribe(m =>
                {
                    Console.WriteLine("LoginSeed arrived");
                    Client.SendAsync(new LoginResponse(ConfigurationManager.AppSettings["Account"],
                        ConfigurationManager.AppSettings["Password"], m.Value));
                });
            var loginCharacterList =
                shared.Where(m => m is LoginCharacterList).Select(m => m as LoginCharacterList).Subscribe(m =>
                {
                    Console.WriteLine("LoginCharacterList arrived");
                    Client.SendAsync(new LoginSelectCharacter(m.First(c => c.Name == ConfigurationManager.AppSettings["Character"]).Id));
                });
            var loginOk =
                shared.Where(m => m is LoginOk)
                    .Select(m => m as LoginOk)
                    .Subscribe(m => Console.WriteLine("Connected"));
            var privateMessages =
                shared.Where(m => m is PrivateMessage)
                    .Select(m => m as PrivateMessage)
                    .Subscribe(m => Console.WriteLine("{0}: {1}", m.CharacterId, m.Message));
            shared.Connect();
            Client.ConnectAsync("chat.d1.funcom.com", 7105).Wait();

            Console.ReadLine();
            login.Dispose();
            loginCharacterList.Dispose();
            loginOk.Dispose();
            privateMessages.Dispose();
            Client.Dispose();
        }
    }
}
