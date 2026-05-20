namespace TaskManagement.Application.Common.Wrappers;

public class ApiResponse<T>
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = [];

    public static ApiResponse<T> Success(T data, string message = "")
        => new() { Succeeded = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string error)
        => new() { Succeeded = false, Errors = [error] };

    public static ApiResponse<T> Fail(List<string> errors)
        => new() { Succeeded = false, Errors = errors };
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse SuccessResult(string message = "")
        => new() { Succeeded = true, Message = message };

    public static new ApiResponse Fail(string error)
        => new() { Succeeded = false, Errors = [error] };
}
