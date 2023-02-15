using FluentAssertions;
using NexusMods.Hashing.xxHash64;
using NexusMods.Paths;

namespace NexusMods.Networking.HttpDownloader.Tests;


public class HttpDownloaderTests
{
    private readonly IHttpDownloader _httpDownloader;
    private readonly TemporaryFileManager _temporaryFileManager;
    private readonly LocalHttpServer _localHttpServer;
    private readonly IHttpDownloader _advancedHttpDownloader;

    public HttpDownloaderTests(SimpleHttpDownloader httpDownloader, AdvancedHttpDownloader downloader, TemporaryFileManager temporaryFileManager, LocalHttpServer localHttpServer)
    {
        _advancedHttpDownloader = downloader;
        _httpDownloader = httpDownloader;
        _temporaryFileManager = temporaryFileManager;
        _localHttpServer = localHttpServer;
    }
    
    [Theory]
    [Trait("RequiresNetworking", "True")]
    [InlineData("100M", 0xBEEADB5B05BED390)]
    [InlineData("500M", 0x06EDA91B1A482A6B)]
    [InlineData("1G", 0x24C6A754B40D7798)]
    public async Task AdvancedDownloaderCanDownloadFromExternalSource(string size, ulong hash)
    {
        await using var path = _temporaryFileManager.CreateFile();

        var resultHash = await _advancedHttpDownloader.Download(new[]
        {
            "http://miami.nexus-cdn.com/" + size,
            "http://la.nexus-cdn.com/" + size,
            "http://paris.nexus-cdn.com/" + size,
            "http://chicago.nexus-cdn.com/" + size,
        }.Select(x => new HttpRequestMessage(HttpMethod.Get, new Uri(x)))
        .ToArray(), path);

        resultHash.Should().Be(Hash.From(hash));
    }

    [Theory]
    [MemberData(nameof(Downloaders))]
    public async Task CanDownloadFromLocalServer(string downloaderName)
    {
        var downloader = GetDownloader(downloaderName);
        await using var path = _temporaryFileManager.CreateFile();

        var resultHash = await downloader.Download(new[]
        {
            new HttpRequestMessage(HttpMethod.Get, _localHttpServer.Uri + "hello")
        }, path);
        
        resultHash.Should().Be(Hash.From(0xA52B286A3E7F4D91));
        (await path.Path.ReadAllTextAsync()).Should().Be("Hello World!");
    }
     
    private IHttpDownloader GetDownloader(string name)
    {
        return name switch
        {
            "Simple" => _httpDownloader,
            "Advanced" => _advancedHttpDownloader,
            _ => throw new NotImplementedException()
        };
    }
    public static IEnumerable<object[]> Downloaders = new []{new object[] {"Simple"}, new object[]{"Advanced"}};
}