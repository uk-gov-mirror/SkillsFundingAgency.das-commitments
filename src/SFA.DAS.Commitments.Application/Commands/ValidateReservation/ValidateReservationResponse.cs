using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.Reservations.Api.Types;

namespace SFA.DAS.Commitments.Application.Commands.ValidateReservation
{
    public class ValidateReservationResponse 
    {
        public ReservationValidationResult ReservationValidationResult { get; set; }
    }
}