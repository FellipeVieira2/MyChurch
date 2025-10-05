namespace MyChurch.Application.Dtos
{
    /// <summary>
    /// DTO com estatísticas gerais da igreja
    /// </summary>
    public class ChurchStatisticsDto
    {
        /// <summary>
        /// Total de membros cadastrados
        /// </summary>
        public int TotalMembers { get; set; }
        
        /// <summary>
        /// Total de membros ativos
        /// </summary>
        public int ActiveMembers { get; set; }
        
        /// <summary>
        /// Total de membros inativos
        /// </summary>
        public int InactiveMembers { get; set; }
        
        /// <summary>
        /// Novos membros no mês atual
        /// </summary>
        public int NewMembersThisMonth { get; set; }
        
        /// <summary>
        /// Novos membros no mês anterior
        /// </summary>
        public int NewMembersLastMonth { get; set; }
        
        /// <summary>
        /// Percentual de crescimento de membros
        /// </summary>
        public decimal MemberGrowthPercentage { get; set; }
        
        /// <summary>
        /// Total de membros batizados
        /// </summary>
        public int BaptizedMembers { get; set; }
        
        /// <summary>
        /// Percentual de membros batizados
        /// </summary>
        public decimal BaptizedPercentage { get; set; }
        
        /// <summary>
        /// Total de dizimistas
        /// </summary>
        public int Tithers { get; set; }
        
        /// <summary>
        /// Percentual de dizimistas
        /// </summary>
        public decimal TithersPercentage { get; set; }
        
        /// <summary>
        /// Média de presença em cultos (%)
        /// </summary>
        public decimal AverageAttendancePercentage { get; set; }
        
        /// <summary>
        /// Total de presenças registradas no mês
        /// </summary>
        public int TotalAttendancesThisMonth { get; set; }
        
        /// <summary>
        /// Receita total do mês atual
        /// </summary>
        public decimal RevenueThisMonth { get; set; }
        
        /// <summary>
        /// Receita total do mês anterior
        /// </summary>
        public decimal RevenueLastMonth { get; set; }
        
        /// <summary>
        /// Percentual de crescimento da receita
        /// </summary>
        public decimal RevenueGrowthPercentage { get; set; }
        
        /// <summary>
        /// Despesas totais do mês atual
        /// </summary>
        public decimal ExpensesThisMonth { get; set; }
        
        /// <summary>
        /// Saldo do mês (receita - despesas)
        /// </summary>
        public decimal BalanceThisMonth { get; set; }
        
        /// <summary>
        /// Total de eventos no mês
        /// </summary>
        public int EventsThisMonth { get; set; }
        
        /// <summary>
        /// Total de ministérios ativos
        /// </summary>
        public int ActiveMinistries { get; set; }
        
        /// <summary>
        /// Total de grupos/células ativos
        /// </summary>
        public int ActiveGroups { get; set; }
        
        /// <summary>
        /// Total de pedidos de oração ativos
        /// </summary>
        public int ActivePrayerRequests { get; set; }
        
        /// <summary>
        /// Taxa de engajamento média (0-100)
        /// </summary>
        public decimal AverageEngagementScore { get; set; }
        
        /// <summary>
        /// Total de visitantes no mês
        /// </summary>
        public int VisitorsThisMonth { get; set; }
        
        /// <summary>
        /// Taxa de conversão de visitantes em membros (%)
        /// </summary>
        public decimal VisitorConversionRate { get; set; }
    }
}
