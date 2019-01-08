namespace SFA.DAS.Commitments.Api.Types.v2.Enums
{
    public enum RecordStatus
    {
        NoActionNeeded = 0,
        ChangesPending = 1,
        ChangesForReview = 2,
        ChangeRequested = 3,
        IlrDataMismatch = 4,
        IlrChangesPending = 5
    }
}