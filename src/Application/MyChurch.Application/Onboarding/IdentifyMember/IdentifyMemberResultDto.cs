namespace MyChurch.Application.Onboarding.IdentifyMember
{
    public class IdentifyMemberResultDto
    {
        public string Status { get; set; } // NotFound, AlreadyActive, ActivationRequired
        public string? MaskedName { get; set; }
    }
}