using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HushServerNode;

public static class RegisterGrpcServer
{
    public static IHostBuilder RegisterGrpc(this IHostBuilder builder)
    {
        builder.ConfigureServices((hostContext, services) =>
        {
            var rcpServer = new Grpc.Core.Server
            {
                Services = { Greeter.BindService(new GreeterService()) },
                Ports = {new ServerPort("localhost", 5000, ServerCredentials.Insecure)}
            };

            rcpServer.Start();

            services.AddSingleton<Grpc.Core.Server>(rcpServer);
        });

        return builder;
    }
}
