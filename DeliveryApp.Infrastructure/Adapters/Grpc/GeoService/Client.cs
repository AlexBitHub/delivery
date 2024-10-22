using System;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Ports;
using GeoApp.Api;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Grpc.GeoService;

public class Client : IGeoClient
{

    private readonly MethodConfig _methodConfig;
    private readonly SocketsHttpHandler _socketsHttpHandler;
    private readonly string _url;

    public Client(string url)
    {        
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentException(nameof(url));
        }
        _url = url;

        _socketsHttpHandler = new SocketsHttpHandler
        {
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true
        };

        _methodConfig = new MethodConfig
        {
            Names = {MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts= 5,
                InitialBackoff = TimeSpan.FromSeconds(1),
                MaxBackoff = TimeSpan.FromSeconds(5),
                BackoffMultiplier = 1.5,
                RetryableStatusCodes = { StatusCode.Unavailable }
            }
        };
    }

    public async Task<Result<Core.Domain.SharedKernel.Location, Error>> GetGeolocationAsync(string street, CancellationToken token)
    {
        using var chanel = GrpcChannel.ForAddress(_url, new GrpcChannelOptions
        {
            HttpHandler = _socketsHttpHandler,
            ServiceConfig = new ServiceConfig{MethodConfigs = {_methodConfig }}
        });
        
        var client = new GeoApp.Api.Geo.GeoClient(chanel);
        var reply = await client.GetGeolocationAsync(new GetGeolocationRequest
        {
            Street = street
        });

        var result = Core.Domain.SharedKernel.Location.Create(reply.Location.X, reply.Location.Y);
        return result;
    }
}
