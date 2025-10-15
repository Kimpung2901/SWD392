namespace BLL.DTO;

public class PaymentStartResponse
{
    public int PaymentId { get; set; }
    public string? PayUrl { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
