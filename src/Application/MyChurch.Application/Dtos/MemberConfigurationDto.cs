namespace MyChurch.Application.Dtos
{
    public class MemberConfigurationDto
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int? PreferredBibleVersionId { get; set; }
        public string PreferredBibleVersionName { get; set; }
        public string ThemePreference { get; set; }
        public string FontSize { get; set; }
        public bool EnableNotifications { get; set; }
        public DateTime LastUpdated { get; set; }

        public static MemberConfigurationDto FromEntity(Domain.Entities.MemberConfiguration config, string versionName = null)
        {
            return new MemberConfigurationDto
            {
                Id = config.Id,
                MemberId = config.MemberId,
                PreferredBibleVersionId = config.PreferredBibleVersionId,
                PreferredBibleVersionName = versionName ?? "Versão desconhecida",
                ThemePreference = config.ThemePreference,
                FontSize = config.FontSize,
                EnableNotifications = config.EnableNotifications,
                LastUpdated = config.LastUpdated
            };
        }
    }
}