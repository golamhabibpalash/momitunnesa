namespace School.Application.DTOs.Common;

public class ResponseDto<T>
{
    public bool Success { get; set; } = true;
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public int? TotalCount { get; set; }

    public static ResponseDto<T> Ok(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public static ResponseDto<T> OkWithCount(T data, int totalCount, string? message = null) =>
        new() { Success = true, Data = data, TotalCount = totalCount, Message = message };

    public static ResponseDto<T> Fail(string message, List<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors };

    public static ResponseDto<T> Fail(Exception ex) =>
        new() { Success = false, Message = ex.Message };
}
