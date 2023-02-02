

using BenchmarkDotNet.Attributes;
using NexusMods.DataModel.RateLimiting;
using NexusMods.Paths;

namespace NexusMods.DataModel.Benchmarks;

[MemoryDiagnoser]
public class ResourceBenchmarks
{
    private readonly Resource<ResourceBenchmarks,Size> _resourceOne;
    private readonly IJob<ResourceBenchmarks,Size> _jobOne;

    public ResourceBenchmarks()
    {
        _resourceOne = new Resource<ResourceBenchmarks, Size>("Resource 1", 4, Size.Zero);
        _jobOne = _resourceOne.Begin("Job 1", Size.Zero, CancellationToken.None).Result;
        _resourceOne = new Resource<ResourceBenchmarks, Size>("Resource 2", 4, Size.Zero);
    }
    
    [Benchmark]
    public async Task JobSetup()
    {
        using var job = await _resourceOne.Begin("Job 1", Size.Zero, CancellationToken.None);
    }
    [Benchmark]
    public async Task JobReport()
    {
        await _jobOne.Report(Size.Zero, CancellationToken.None);
    }
    
    [Benchmark]
    public async Task JobReportNoWait()
    {
        _jobOne.ReportNoWait(Size.Zero);
    }
    
    
}

/*

History:
2023 - 02 - 01: Initial version
|          Method |      Mean |     Error |    StdDev |
|---------------- |----------:|----------:|----------:|
|        JobSetup |  89.23 ns |  1.407 ns |  1.316 ns |
|       JobReport | 866.46 ns | 20.645 ns | 60.873 ns |
| JobReportNoWait |  12.56 ns |  0.156 ns |  0.146 ns |
2023 - 02 - 01: After removing use of channels
|          Method |      Mean |    Error |   StdDev |   Gen0 | Allocated |
|---------------- |----------:|---------:|---------:|-------:|----------:|
|        JobSetup | 112.81 ns | 2.274 ns | 2.233 ns | 0.0081 |     136 B |
|       JobReport | 263.39 ns | 3.231 ns | 2.698 ns |      - |         - |
| JobReportNoWait |  12.57 ns | 0.174 ns | 0.163 ns |      - |         - |




*/