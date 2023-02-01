using BenchmarkDotNet.Attributes;
using NexusMods.FileExtractor.FileSignatures;

namespace NexusMods.DataModel.Benchmarks;

[MemoryDiagnoser]
public class SignatureCheckerBenchmarks
{
    private readonly SignatureChecker _checker;
    private readonly byte[] _data;
    private readonly MemoryStream _stream;

    public SignatureCheckerBenchmarks()
    {
        _checker = new SignatureChecker(Enum.GetValues<FileType>().ToArray());
        _data = new byte[_checker.LongestSignature];
        Random.Shared.NextBytes(_data);
        _stream = new MemoryStream(_data);
    }
    
    [Benchmark]
    public async Task MatchesAsync()
    {
        await _checker.MatchesAsync(_stream);
    }
}

/*
History:
    2023-02-01: Initial version
    |       Method |     Mean |    Error |   StdDev |   Gen0 | Allocated |
    |------------- |---------:|---------:|---------:|-------:|----------:|
    | MatchesAsync | 755.3 ns | 13.45 ns | 12.58 ns | 0.2222 |   3.64 KB |

    2023-02-01: Switch to Span, remove use of IEnumerable
    |       Method |     Mean |    Error |   StdDev |   Gen0 | Allocated |
    |------------- |---------:|---------:|---------:|-------:|----------:|
    | MatchesAsync | 75.96 ns | 1.403 ns | 1.313 ns | 0.0048 |      80 B |




*/