namespace MinCloud.Internal.SDK;

public interface ITokenCredential
{
    Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);
    Task<string?> GetUserTokenAsync(CancellationToken cancellationToken = default);
}
