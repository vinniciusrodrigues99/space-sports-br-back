using System.Text.Json.Serialization;

namespace FSP.Api.Domain.Common;

public class ResponseBase<T> : ResponseBase
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    private ResponseBase(T data, IEnumerable<string> messages)
    {
        Data = data;
        BaseData = data;
        Messages = messages;
    }

    private ResponseBase(IEnumerable<string> errors)
    {
        Errors = errors;
        Messages = errors;
    }

    public ResponseBase() { }

    public static ResponseBase<T> Failure()
    {
        return new ResponseBase<T>([]);
    }

    public static ResponseBase<T> Failure(string error)
    {
        return new ResponseBase<T>([error]);
    }

    public static new ResponseBase<T> Failure(IEnumerable<string> errors)
    {
        return new ResponseBase<T>(errors);
    }

    public static ResponseBase<T> Success(T data)
    {
        return new ResponseBase<T>(data, []);
    }

    public static ResponseBase<T> Success(T data, IEnumerable<string> messages)
    {
        return new ResponseBase<T>(data, messages);
    }

    public static ResponseBase<T> Warning(IEnumerable<string> warning, T? data = default)
    {
        return new ResponseBase<T>
        {
            Data = data,
            BaseData = data,
            Warnings = warning,
            Errors = [],
            Messages = []
        };
    }
}

public class ResponseBase
{
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess => !Errors.Any();

    [JsonPropertyName("errors")]
    public IEnumerable<string> Errors { get; internal set; } = [];

    [JsonPropertyName("messages")]
    public IEnumerable<string> Messages { get; internal set; } = [];

    [JsonIgnore]
    public object? BaseData { get; internal set; } = null;

    [JsonPropertyName("warnings")]
    public IEnumerable<string> Warnings { get; set; } = [];

    public static ResponseBase Success(object? data, IEnumerable<string> messages)
    {
        return new ResponseBase { BaseData = data, Messages = messages };
    }
    public static ResponseBase Failure(IEnumerable<string> errors)
    {
        return new ResponseBase { Errors = errors, Messages = errors };
    }
    public static ResponseBase Warning(IEnumerable<string> warnings, object? data = null)
    {
        return new ResponseBase
        {
            BaseData = data,
            Warnings = warnings,
            Errors = [],
            Messages = []
        };
    }
}

