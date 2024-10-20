using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Ports;

public interface IGeoClient
{
    public Task<Result<Location, Error>> GetGeolocationAsync(string street, CancellationToken token);
}
