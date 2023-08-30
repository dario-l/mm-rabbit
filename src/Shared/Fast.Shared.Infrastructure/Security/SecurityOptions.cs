namespace Fast.Shared.Infrastructure.Security;

internal sealed class SecurityOptions
{
    public EncryptionOptions? Encryption { get; set; }

    internal sealed class EncryptionOptions
    {
        public bool Enabled { get; set; }
        public string? Key { get; set; }
    }
}