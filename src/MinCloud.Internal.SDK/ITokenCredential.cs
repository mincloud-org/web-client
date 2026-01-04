using System;

namespace MinCloud.Internal.SDK;

public interface ITokenCredential
{
    Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);
}
