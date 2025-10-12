namespace MyChurch.Domain.Enum
{
    /// <summary>
    /// Sistema de permissões granulares por módulo
    /// </summary>
    public enum Permission
    {
        // ============================================
        // ?? MEMBROS
        // ============================================
        ViewMembers = 1000,
        CreateMembers = 1001,
        EditMembers = 1002,
        DeleteMembers = 1003,
        ExportMembers = 1004,
        ImportMembers = 1005,
        ApproveMemberRegistration = 1006,
        ViewMemberDocuments = 1007,
        ViewMemberSensitiveData = 1008, // Endereço, telefone, etc
        ManageMemberRoles = 1009,
        ResetMemberPasswords = 1010,

        // ============================================
        // ?? FINANÇAS
        // ============================================
        ViewFinances = 2000,
        ViewDonations = 2001,
        ManageDonations = 2002,
        ExportFinancialReports = 2003,
        ViewCampaigns = 2004,
        CreateCampaigns = 2005,
        EditCampaigns = 2006,
        DeleteCampaigns = 2007,
        ViewCashFlow = 2008,
        ManageCashFlow = 2009,
        CreateCashFlowEntry = 2010,
        EditCashFlowEntry = 2011,
        DeleteCashFlowEntry = 2012,
        ViewBankingInfo = 2013,
        ManageBankingInfo = 2014,
        RequestTransfers = 2015,
        ApproveTransfers = 2016, // PlatformAdmin apenas
        ViewPlatformFees = 2017,
        ManagePlatformFees = 2018, // PlatformAdmin apenas

        // ============================================
        // ?? EVENTOS
        // ============================================
        ViewEvents = 3000,
        CreateEvents = 3001,
        EditEvents = 3002,
        DeleteEvents = 3003,
        ManageEventParticipants = 3004,
        SendEventNotifications = 3005,
        ViewEventReports = 3006,
        ManageEventRecurrence = 3007,

        // ============================================
        // ?? CULTOS E ATIVIDADES
        // ============================================
        ViewWorshipServices = 4000,
        CreateWorshipServices = 4001,
        EditWorshipServices = 4002,
        DeleteWorshipServices = 4003,
        ManageWorshipSchedule = 4004,
        ManagePrayerRequests = 4005,
        ViewPrayerRequests = 4006,
        ManagePresenceTracking = 4007,
        ViewPresenceReports = 4008,

        // ============================================
        // ??????????? GRUPOS
        // ============================================
        ViewGroups = 5000,
        CreateGroups = 5001,
        EditGroups = 5002,
        DeleteGroups = 5003,
        ManageGroupMembers = 5004,
        ManageGroupMeetings = 5005,
        ViewGroupReports = 5006,
        ManageGroupResources = 5007,

        // ============================================
        // ?? PLANOS DE LEITURA BÍBLICA
        // ============================================
        ViewBibleReadingPlans = 6000,
        CreateBibleReadingPlans = 6001,
        EditBibleReadingPlans = 6002,
        DeleteBibleReadingPlans = 6003,
        AssignBibleReadingPlans = 6004,
        ViewMemberBibleProgress = 6005,
        ManagePublicPlans = 6006,

        // ============================================
        // ?? JORNADAS ESPIRITUAIS
        // ============================================
        ViewJourneys = 7000,
        CreateJourneys = 7001,
        EditJourneys = 7002,
        DeleteJourneys = 7003,
        AssignJourneys = 7004,
        VerifyJourneyProgress = 7005,
        ViewJourneyReports = 7006,

        // ============================================
        // ?? VISITANTES
        // ============================================
        ViewVisitors = 8000,
        CreateVisitors = 8001,
        EditVisitors = 8002,
        DeleteVisitors = 8003,
        ManageVisitorStatus = 8004,
        ViewVisitorTimeline = 8005,
        ManageVisitorFollowUp = 8006,
        ConvertVisitorToMember = 8007,

        // ============================================
        // ?? RELATÓRIOS E ANALYTICS
        // ============================================
        ViewDashboard = 9000,
        ViewEngagementReports = 9001,
        ViewFinancialDashboard = 9002,
        ViewMembershipGrowth = 9003,
        ExportAllReports = 9004,
        ViewPastoralAlerts = 9005,

        // ============================================
        // ?? CONFIGURAÇÕES DA IGREJA
        // ============================================
        ViewChurchSettings = 10000,
        EditChurchSettings = 10001,
        ManageChurchPhotos = 10002,
        ManageChurchSchedules = 10003,
        ViewSubscriptionInfo = 10004,
        ManageSubscription = 10005,
        ViewOnboardingQRCode = 10006,
        ManageSocialMediaLinks = 10007,

        // ============================================
        // ?? APRESENTAÇÕES E MÍDIA
        // ============================================
        ViewPresentations = 11000,
        CreatePresentations = 11001,
        EditPresentations = 11002,
        DeletePresentations = 11003,
        ControlLivePresentations = 11004,

        // ============================================
        // ?? FEED SOCIAL
        // ============================================
        ViewFeed = 12000,
        CreatePosts = 12001,
        EditOwnPosts = 12002,
        EditAllPosts = 12003,
        DeleteOwnPosts = 12004,
        DeleteAllPosts = 12005,
        ManageFeedImages = 12006,

        // ============================================
        // ?? GAMIFICAÇÃO
        // ============================================
        ViewAchievements = 13000,
        ManageAchievements = 13001,
        ViewFaithLevels = 13002,
        ManageFaithLevels = 13003,
        ViewDailyChallenges = 13004,
        ManageDailyChallenges = 13005,

        // ============================================
        // ? AVALIAÇÕES
        // ============================================
        ViewReviews = 14000,
        CreateReviews = 14001,
        EditOwnReviews = 14002,
        DeleteOwnReviews = 14003,
        ModerateReviews = 14004,
        RespondToReviews = 14005,

        // ============================================
        // ???????? FAMÍLIAS
        // ============================================
        ViewFamilies = 15000,
        CreateFamilies = 15001,
        EditFamilies = 15002,
        DeleteFamilies = 15003,
        ManageFamilyMembers = 15004,
        ManageChildren = 15005,

        // ============================================
        // ?? ADMINISTRAÇÃO DE SISTEMA
        // ============================================
        ManageUserRoles = 16000,
        ManagePermissions = 16001,
        ViewAuditLogs = 16002,
        ManageIntegrations = 16003,
        AccessAdminPanel = 16004,
        
        // ============================================
        // ?? PLATAFORMA (apenas PlatformAdmin)
        // ============================================
        ManageAllChurches = 17000,
        ViewPlatformStatistics = 17001,
        ManagePlans = 17002,
        ManageGlobalSettings = 17003,
        ImpersonateChurch = 17004,
    }
}
