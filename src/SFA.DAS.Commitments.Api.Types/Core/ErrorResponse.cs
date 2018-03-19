namespace SFA.DAS.Commitments.Api.Types.Core
{
    public class ErrorResponse
    {
        public string Code { get; set; }
        public int? DomainExceptionId { get; set; }
        public string Message { get; set; }
    }
}
