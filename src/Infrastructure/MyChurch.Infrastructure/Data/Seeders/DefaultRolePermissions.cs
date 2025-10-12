using MyChurch.Domain.Enum;

namespace MyChurch.Infrastructure.Data.Seeders
{
    /// <summary>
    /// Configuração de permissões padrão para cada role
    /// </summary>
    public static class DefaultRolePermissions
    {
        public static Dictionary<UserRole, List<Permission>> GetDefaultPermissions()
        {
            return new Dictionary<UserRole, List<Permission>>
            {
                // ============================================
                // ?? PLATFORM ADMIN - Acesso total
                // ============================================
                [UserRole.PlatformAdmin] = GetAllPermissions(),

                // ============================================
                // ????? ADMIN - Administrador da igreja
                // ============================================
                [UserRole.Admin] = new List<Permission>
                {
                    // Membros
                    Permission.ViewMembers,
                    Permission.CreateMembers,
                    Permission.EditMembers,
                    Permission.DeleteMembers,
                    Permission.ExportMembers,
                    Permission.ImportMembers,
                    Permission.ApproveMemberRegistration,
                    Permission.ViewMemberDocuments,
                    Permission.ViewMemberSensitiveData,
                    Permission.ManageMemberRoles,
                    Permission.ResetMemberPasswords,

                    // Finanças
                    Permission.ViewFinances,
                    Permission.ViewDonations,
                    Permission.ManageDonations,
                    Permission.ExportFinancialReports,
                    Permission.ViewCampaigns,
                    Permission.CreateCampaigns,
                    Permission.EditCampaigns,
                    Permission.DeleteCampaigns,
                    Permission.ViewCashFlow,
                    Permission.ManageCashFlow,
                    Permission.CreateCashFlowEntry,
                    Permission.EditCashFlowEntry,
                    Permission.DeleteCashFlowEntry,
                    Permission.ViewBankingInfo,
                    Permission.ManageBankingInfo,
                    Permission.RequestTransfers,

                    // Eventos
                    Permission.ViewEvents,
                    Permission.CreateEvents,
                    Permission.EditEvents,
                    Permission.DeleteEvents,
                    Permission.ManageEventParticipants,
                    Permission.SendEventNotifications,
                    Permission.ViewEventReports,
                    Permission.ManageEventRecurrence,

                    // Cultos
                    Permission.ViewWorshipServices,
                    Permission.CreateWorshipServices,
                    Permission.EditWorshipServices,
                    Permission.DeleteWorshipServices,
                    Permission.ManageWorshipSchedule,
                    Permission.ManagePrayerRequests,
                    Permission.ViewPrayerRequests,
                    Permission.ManagePresenceTracking,
                    Permission.ViewPresenceReports,

                    // Grupos
                    Permission.ViewGroups,
                    Permission.CreateGroups,
                    Permission.EditGroups,
                    Permission.DeleteGroups,
                    Permission.ManageGroupMembers,
                    Permission.ManageGroupMeetings,
                    Permission.ViewGroupReports,
                    Permission.ManageGroupResources,

                    // Planos de Leitura
                    Permission.ViewBibleReadingPlans,
                    Permission.CreateBibleReadingPlans,
                    Permission.EditBibleReadingPlans,
                    Permission.DeleteBibleReadingPlans,
                    Permission.AssignBibleReadingPlans,
                    Permission.ViewMemberBibleProgress,
                    Permission.ManagePublicPlans,

                    // Jornadas
                    Permission.ViewJourneys,
                    Permission.CreateJourneys,
                    Permission.EditJourneys,
                    Permission.DeleteJourneys,
                    Permission.AssignJourneys,
                    Permission.VerifyJourneyProgress,
                    Permission.ViewJourneyReports,

                    // Visitantes
                    Permission.ViewVisitors,
                    Permission.CreateVisitors,
                    Permission.EditVisitors,
                    Permission.DeleteVisitors,
                    Permission.ManageVisitorStatus,
                    Permission.ViewVisitorTimeline,
                    Permission.ManageVisitorFollowUp,
                    Permission.ConvertVisitorToMember,

                    // Relatórios
                    Permission.ViewDashboard,
                    Permission.ViewEngagementReports,
                    Permission.ViewFinancialDashboard,
                    Permission.ViewMembershipGrowth,
                    Permission.ExportAllReports,
                    Permission.ViewPastoralAlerts,

                    // Configurações
                    Permission.ViewChurchSettings,
                    Permission.EditChurchSettings,
                    Permission.ManageChurchPhotos,
                    Permission.ManageChurchSchedules,
                    Permission.ViewSubscriptionInfo,
                    Permission.ManageSubscription,
                    Permission.ViewOnboardingQRCode,
                    Permission.ManageSocialMediaLinks,

                    // Apresentações
                    Permission.ViewPresentations,
                    Permission.CreatePresentations,
                    Permission.EditPresentations,
                    Permission.DeletePresentations,
                    Permission.ControlLivePresentations,

                    // Feed Social
                    Permission.ViewFeed,
                    Permission.CreatePosts,
                    Permission.EditOwnPosts,
                    Permission.EditAllPosts,
                    Permission.DeleteOwnPosts,
                    Permission.DeleteAllPosts,
                    Permission.ManageFeedImages,

                    // Gamificação
                    Permission.ViewAchievements,
                    Permission.ManageAchievements,
                    Permission.ViewFaithLevels,
                    Permission.ManageFaithLevels,
                    Permission.ViewDailyChallenges,
                    Permission.ManageDailyChallenges,

                    // Avaliações
                    Permission.ViewReviews,
                    Permission.CreateReviews,
                    Permission.EditOwnReviews,
                    Permission.DeleteOwnReviews,
                    Permission.ModerateReviews,
                    Permission.RespondToReviews,

                    // Famílias
                    Permission.ViewFamilies,
                    Permission.CreateFamilies,
                    Permission.EditFamilies,
                    Permission.DeleteFamilies,
                    Permission.ManageFamilyMembers,
                    Permission.ManageChildren,

                    // Sistema
                    Permission.ManageUserRoles,
                    Permission.ManagePermissions,
                    Permission.ViewAuditLogs,
                    Permission.ManageIntegrations,
                    Permission.AccessAdminPanel,
                },

                // ============================================
                // ?? MINISTER - Ministro/Pastor
                // ============================================
                [UserRole.Minister] = new List<Permission>
                {
                    // Membros
                    Permission.ViewMembers,
                    Permission.CreateMembers,
                    Permission.EditMembers,
                    Permission.ViewMemberDocuments,
                    Permission.ViewMemberSensitiveData,
                    Permission.ApproveMemberRegistration,

                    // Finanças (apenas visualização)
                    Permission.ViewFinances,
                    Permission.ViewDonations,
                    Permission.ViewCampaigns,
                    Permission.ViewCashFlow,

                    // Eventos
                    Permission.ViewEvents,
                    Permission.CreateEvents,
                    Permission.EditEvents,
                    Permission.ManageEventParticipants,
                    Permission.SendEventNotifications,

                    // Cultos
                    Permission.ViewWorshipServices,
                    Permission.CreateWorshipServices,
                    Permission.EditWorshipServices,
                    Permission.ManageWorshipSchedule,
                    Permission.ManagePrayerRequests,
                    Permission.ViewPrayerRequests,
                    Permission.ManagePresenceTracking,
                    Permission.ViewPresenceReports,

                    // Grupos
                    Permission.ViewGroups,
                    Permission.CreateGroups,
                    Permission.EditGroups,
                    Permission.ManageGroupMembers,
                    Permission.ViewGroupReports,

                    // Jornadas
                    Permission.ViewJourneys,
                    Permission.AssignJourneys,
                    Permission.VerifyJourneyProgress,
                    Permission.ViewJourneyReports,

                    // Visitantes
                    Permission.ViewVisitors,
                    Permission.CreateVisitors,
                    Permission.EditVisitors,
                    Permission.ManageVisitorStatus,
                    Permission.ViewVisitorTimeline,
                    Permission.ManageVisitorFollowUp,
                    Permission.ConvertVisitorToMember,

                    // Relatórios
                    Permission.ViewDashboard,
                    Permission.ViewEngagementReports,
                    Permission.ViewMembershipGrowth,
                    Permission.ViewPastoralAlerts,

                    // Feed
                    Permission.ViewFeed,
                    Permission.CreatePosts,
                    Permission.EditOwnPosts,
                    Permission.DeleteOwnPosts,
                    Permission.EditAllPosts,
                    Permission.DeleteAllPosts,

                    // Apresentações
                    Permission.ViewPresentations,
                    Permission.CreatePresentations,
                    Permission.EditPresentations,
                    Permission.ControlLivePresentations,

                    // Famílias
                    Permission.ViewFamilies,
                    Permission.CreateFamilies,
                    Permission.EditFamilies,
                    Permission.ManageFamilyMembers,
                },

                // ============================================
                // ?? LEADER - Líder de ministério/departamento
                // ============================================
                [UserRole.Leader] = new List<Permission>
                {
                    // Membros
                    Permission.ViewMembers,
                    Permission.ViewMemberDocuments,

                    // Eventos
                    Permission.ViewEvents,
                    Permission.CreateEvents,
                    Permission.EditEvents,
                    Permission.ManageEventParticipants,

                    // Cultos
                    Permission.ViewWorshipServices,
                    Permission.ViewPrayerRequests,
                    Permission.ManagePresenceTracking,

                    // Grupos
                    Permission.ViewGroups,
                    Permission.CreateGroups,
                    Permission.EditGroups,
                    Permission.ManageGroupMembers,
                    Permission.ManageGroupMeetings,
                    Permission.ManageGroupResources,

                    // Jornadas
                    Permission.ViewJourneys,
                    Permission.AssignJourneys,
                    Permission.VerifyJourneyProgress,

                    // Feed
                    Permission.ViewFeed,
                    Permission.CreatePosts,
                    Permission.EditOwnPosts,
                    Permission.DeleteOwnPosts,

                    // Apresentações
                    Permission.ViewPresentations,
                    Permission.ControlLivePresentations,
                },

                // ============================================
                // ?? WORKER - Obreiro/Colaborador
                // ============================================
                [UserRole.Worker] = new List<Permission>
                {
                    Permission.ViewMembers,
                    Permission.ViewEvents,
                    Permission.ViewWorshipServices,
                    Permission.ManagePresenceTracking,
                    Permission.ViewGroups,
                    Permission.ViewFeed,
                    Permission.CreatePosts,
                    Permission.EditOwnPosts,
                    Permission.DeleteOwnPosts,
                    Permission.ViewPresentations,
                },

                // ============================================
                // ?? DEACON - Diácono
                // ============================================
                [UserRole.Deacon] = new List<Permission>
                {
                    Permission.ViewMembers,
                    Permission.ViewEvents,
                    Permission.ViewWorshipServices,
                    Permission.ManagePresenceTracking,
                    Permission.ViewDonations,
                    Permission.ViewVisitors,
                    Permission.CreateVisitors,
                    Permission.ViewGroups,
                    Permission.ViewFeed,
                    Permission.CreatePosts,
                    Permission.EditOwnPosts,
                    Permission.DeleteOwnPosts,
                },

                // ============================================
                // ?? ELDER - Presbítero
                // ============================================
                [UserRole.Elder] = new List<Permission>
                {
                    Permission.ViewMembers,
                    Permission.ViewMemberDocuments,
                    Permission.ViewEvents,
                    Permission.CreateEvents,
                    Permission.EditEvents,
                    Permission.ViewWorshipServices,
                    Permission.ViewPrayerRequests,
                    Permission.ManagePrayerRequests,
                    Permission.ViewGroups,
                    Permission.ViewVisitors,
                    Permission.CreateVisitors,
                    Permission.EditVisitors,
                    Permission.ViewDonations,
                    Permission.ViewFeed,
                    Permission.CreatePosts,
                    Permission.EditOwnPosts,
                    Permission.DeleteOwnPosts,
                    Permission.ViewPresentations,
                },

                // ============================================
                // ?? MEMBER - Membro comum
                // ============================================
                [UserRole.Member] = new List<Permission>
                {
                    Permission.ViewEvents,
                    Permission.ViewWorshipServices,
                    Permission.ViewBibleReadingPlans,
                    Permission.ViewJourneys,
                    Permission.ViewGroups,
                    Permission.ViewFeed,
                    Permission.CreatePosts,
                    Permission.EditOwnPosts,
                    Permission.DeleteOwnPosts,
                    Permission.ViewAchievements,
                    Permission.ViewFaithLevels,
                    Permission.ViewDailyChallenges,
                    Permission.ViewReviews,
                    Permission.CreateReviews,
                    Permission.EditOwnReviews,
                    Permission.DeleteOwnReviews,
                    Permission.ViewFamilies, // Apenas sua própria família
                },

                // ============================================
                // ?? VISITOR - Visitante
                // ============================================
                [UserRole.Visitor] = new List<Permission>
                {
                    Permission.ViewEvents,
                    Permission.ViewWorshipServices,
                    Permission.ViewFeed,
                    Permission.ViewReviews,
                    Permission.CreateReviews,
                },
            };
        }

        private static List<Permission> GetAllPermissions()
        {
            return Enum.GetValues<Permission>().ToList();
        }
    }
}
