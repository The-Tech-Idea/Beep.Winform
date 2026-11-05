using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Vis.Modules.DataView;

[Flags]
public enum DataViewCapabilities
{
    None = 0,
    Sort = 1,
    Filter = 2,
    Group = 4,
    Summary = 8,
    Paging = 16,
    TotalCount = 32,
    StableKeys = 64,
    IncrementalChanges = 128,
    ServerSide = 256,
    Cancellation = 512
}

public enum DataViewChangeKind
{
    Reset,
    Added,
    Removed,
    Updated,
    State
}

public enum DataViewAggregation
{
    Sum,
    Count,
    Average,
    Minimum,
    Maximum
}

public interface IDataViewFilter
{
    bool Matches(object item);
}

public sealed class DelegateDataViewFilter : IDataViewFilter
{
    private readonly Func<object, bool> _predicate;

    public DelegateDataViewFilter(Func<object, bool> predicate)
        => _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

    public bool Matches(object item) => _predicate(item);
}

public sealed class DataViewSort
{
    public DataViewSort(string path, bool descending, Func<object, object> keySelector)
    {
        Path = !string.IsNullOrWhiteSpace(path) ? path : throw new ArgumentException("A sort path is required.", nameof(path));
        Descending = descending;
        KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
    }

    public string Path { get; }
    public bool Descending { get; }
    public Func<object, object> KeySelector { get; }
}

public sealed class DataViewGroup
{
    public DataViewGroup(string path, Func<object, object> keySelector)
    {
        Path = !string.IsNullOrWhiteSpace(path) ? path : throw new ArgumentException("A group path is required.", nameof(path));
        KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
    }

    public string Path { get; }
    public Func<object, object> KeySelector { get; }
}

public sealed class DataViewSummary
{
    public DataViewSummary(string path, DataViewAggregation aggregation, Func<object, object> valueAccessor)
    {
        Path = !string.IsNullOrWhiteSpace(path) ? path : throw new ArgumentException("A summary path is required.", nameof(path));
        Aggregation = aggregation;
        ValueAccessor = valueAccessor ?? throw new ArgumentNullException(nameof(valueAccessor));
    }

    public string Path { get; }
    public DataViewAggregation Aggregation { get; }
    public Func<object, object> ValueAccessor { get; }
}

public sealed class DataViewQuery
{
    public static DataViewQuery Empty { get; } = new();

    public DataViewQuery(
        IReadOnlyList<DataViewSort> sorts = null,
        IDataViewFilter filter = null,
        IReadOnlyList<DataViewGroup> groups = null,
        IReadOnlyList<DataViewSummary> summaries = null)
    {
        Sorts = sorts ?? Array.Empty<DataViewSort>();
        Filter = filter;
        Groups = groups ?? Array.Empty<DataViewGroup>();
        Summaries = summaries ?? Array.Empty<DataViewSummary>();
    }

    public IReadOnlyList<DataViewSort> Sorts { get; }
    public IDataViewFilter Filter { get; }
    public IReadOnlyList<DataViewGroup> Groups { get; }
    public IReadOnlyList<DataViewSummary> Summaries { get; }
}

public sealed class DataPageRequest
{
    public DataPageRequest(int offset, int pageSize, DataViewQuery query = null)
    {
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
        Offset = offset;
        PageSize = pageSize;
        Query = query ?? DataViewQuery.Empty;
    }

    public int Offset { get; }
    public int PageSize { get; }
    public DataViewQuery Query { get; }
}

public sealed record DataPage<T>(
    IReadOnlyList<T> Items,
    int? TotalCount,
    string ContinuationToken,
    long Version,
    bool IsPartial,
    DataViewCapabilities Applied);

public sealed class DataViewChangedEventArgs : EventArgs
{
    public DataViewChangedEventArgs(DataViewChangeKind kind, IReadOnlyList<object> keys, long version)
    {
        Kind = kind;
        Keys = keys ?? Array.Empty<object>();
        Version = version;
    }

    public DataViewChangeKind Kind { get; }
    public IReadOnlyList<object> Keys { get; }
    public long Version { get; }
}

public sealed class DataViewNotSupportedException : NotSupportedException
{
    public DataViewNotSupportedException(string message, DataViewCapabilities capability)
        : base(message) => Capability = capability;

    public DataViewCapabilities Capability { get; }
}

public interface IDataViewProvider<T> : IDisposable
{
    DataViewCapabilities Capabilities { get; }
    Func<T, object> KeySelector { get; }
    Task<DataPage<T>> GetPageAsync(DataPageRequest request, CancellationToken cancellationToken);
    event EventHandler<DataViewChangedEventArgs> Changed;
}

public interface IDataViewProvider : IDisposable
{
    DataViewCapabilities Capabilities { get; }
    Func<object, object> KeySelector { get; }
    Task<DataPage<object>> GetPageAsync(DataPageRequest request, CancellationToken cancellationToken);
    event EventHandler<DataViewChangedEventArgs> Changed;
}
