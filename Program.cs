using System.Net;
using System.Net.Security;
using System.Net.Quic;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

[System.Runtime.Versioning.SupportedOSPlatform("macOS")]
[System.Runtime.Versioning.SupportedOSPlatform("linux")]
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public static class Program
{
    public static async Task Main()
    {
        await ListenAsync();
        await ListenAsync();
    }

    private static async Task ListenAsync() =>
        await QuicListener.ListenAsync(
            new QuicListenerOptions
            {
                ListenEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9090),
                ApplicationProtocols = new List<SslApplicationProtocol>
                {
                    new SslApplicationProtocol("foo")
                },
                ConnectionOptionsCallback = GetConnectionOptionsAsync
            },
            CancellationToken.None);


    private static ValueTask<QuicServerConnectionOptions> GetConnectionOptionsAsync(
        QuicConnection connection,
        SslClientHelloInfo sslInfo,
        CancellationToken cancellationToken) =>
        new(new QuicServerConnectionOptions
        {
            ServerAuthenticationOptions = new SslServerAuthenticationOptions()
            {
                ServerCertificate = new X509Certificate2("server.p12", "password"),
                ApplicationProtocols = new List<SslApplicationProtocol> // Mandatory with Quic
                {
                    new SslApplicationProtocol("foo")
                }
            },
            DefaultStreamErrorCode = 0,
            DefaultCloseErrorCode = 0,
        });
}
