using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Application.Plans.Services;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using System.Text;

namespace MyChurch.Application.Departments.Queries.GetDepartmentPermissionOverview
{
    public class GetDepartmentPermissionOverviewQuery : JwtMemberDto, IRequest<DepartmentPermissionOverviewDto>
    {
        public int DepartmentId { get; set; }
    }

    public class DepartmentPermissionOverviewDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string? DepartmentDescription { get; set; }
        public bool AdvancedPermissionsEnabled { get; set; }
        public bool CurrentUserIsDepartmentMember { get; set; }
        public bool CurrentUserCanManagePermissions { get; set; }
        public List<DepartmentRoleRuleDto> RoleRules { get; set; } = new();
        public List<DepartmentMemberPermissionOverviewDto> Members { get; set; } = new();
    }

    public class DepartmentRoleRuleDto
    {
        public DepartmentMemberRole Role { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool CanViewDepartmentData { get; set; }
        public bool CanViewFinancialData { get; set; }
        public bool CanManageFinancialData { get; set; }
        public bool CanViewSensitiveData { get; set; }
        public bool CanSuperviseBranches { get; set; }
    }

    public class DepartmentMemberPermissionOverviewDto
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string? MemberEmail { get; set; }
        public UserRole SystemRole { get; set; }
        public DepartmentMemberRole DepartmentRole { get; set; }
        public bool IsActive { get; set; }
        public bool HasDepartmentScope { get; set; }
        public bool HasFinancialScope { get; set; }
        public bool HasCrossChurchScope { get; set; }
        public bool CanViewSensitiveData { get; set; }
        public bool HasCustomPermissionOverrides { get; set; }
        public List<PermissionModuleOverviewDto> Modules { get; set; } = new();
    }

    public class PermissionModuleOverviewDto
    {
        public string ModuleKey { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool CanView { get; set; }
        public bool CanManage { get; set; }
        public bool HasSensitiveDataAccess { get; set; }
        public List<PermissionItemDto> Permissions { get; set; } = new();
    }

    public class PermissionItemDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsSensitive { get; set; }
    }

    public class GetDepartmentPermissionOverviewQueryHandler : IRequestHandler<GetDepartmentPermissionOverviewQuery, DepartmentPermissionOverviewDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPlanLimitService _planLimitService;

        public GetDepartmentPermissionOverviewQueryHandler(IUnitOfWork uow, IPlanLimitService planLimitService)
        {
            _uow = uow;
            _planLimitService = planLimitService;
        }

        public async Task<DepartmentPermissionOverviewDto> Handle(GetDepartmentPermissionOverviewQuery request, CancellationToken cancellationToken)
        {
            var loggedMember = await _uow.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");

            var department = await _uow.Departments.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == request.DepartmentId && d.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (department == null)
                ValidationException.ThrowException("Department", "Departamento não encontrado.");

            var currentDepartmentMembership = await _uow.DepartmentMembers.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(dm => dm.DepartmentId == request.DepartmentId && dm.MemberId == loggedMember.Id && dm.IsActive, cancellationToken);

            if (loggedMember.Role != UserRole.Admin && currentDepartmentMembership == null)
                ValidationException.ThrowException("Department", "Sem permissão para visualizar as permissões deste ministério.");

            var plan = await _planLimitService.GetActivePlanForChurchAsync(loggedMember.ChurchId, cancellationToken);
            var advancedPermissionsEnabled = plan?.HasAdvancedPermissions == true;

            var departmentMembers = await _uow.DepartmentMembers.Query()
                .AsNoTracking()
                .Include(dm => dm.Member)
                .Where(dm => dm.DepartmentId == request.DepartmentId)
                .OrderBy(dm => dm.Role)
                .ThenBy(dm => dm.Member.Name)
                .ToListAsync(cancellationToken);

            var memberIds = departmentMembers
                .Select(dm => dm.MemberId)
                .Distinct()
                .ToList();

            var distinctRoles = departmentMembers
                .Select(dm => dm.Member.Role)
                .Distinct()
                .ToList();

            var rolePermissionCache = new Dictionary<UserRole, HashSet<Permission>>();

            foreach (var role in distinctRoles)
            {
                var rolePermissions = advancedPermissionsEnabled
                    ? await _uow.RolePermissions.GetByRoleAsync(role, loggedMember.ChurchId)
                    : await _uow.RolePermissions.GetByRoleAsync(role, null);

                rolePermissionCache[role] = rolePermissions
                    .Where(rp => rp.IsActive)
                    .Select(rp => rp.Permission)
                    .ToHashSet();
            }

            var customPermissionMap = advancedPermissionsEnabled
                ? await _uow.MemberCustomPermissions.Query()
                    .AsNoTracking()
                    .Where(cp => memberIds.Contains(cp.MemberId))
                    .Where(cp => !cp.ExpiresAt.HasValue || cp.ExpiresAt.Value > DateTime.UtcNow)
                    .GroupBy(cp => cp.MemberId)
                    .ToDictionaryAsync(g => g.Key, g => g.ToList(), cancellationToken)
                : new Dictionary<int, List<MemberCustomPermission>>();

            var crossChurchScopeMemberIds = await _uow.DepartmentGeneralLeaderScopes.Query()
                .AsNoTracking()
                .Where(s => s.IsActive && memberIds.Contains(s.LeaderMemberId))
                .Select(s => s.LeaderMemberId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var currentUserPermissions = BuildEffectivePermissions(
                loggedMember.Role,
                rolePermissionCache.ContainsKey(loggedMember.Role) ? rolePermissionCache[loggedMember.Role] : new HashSet<Permission>(),
                advancedPermissionsEnabled && customPermissionMap.TryGetValue(loggedMember.Id, out var currentUserCustomPermissions)
                    ? currentUserCustomPermissions
                    : new List<MemberCustomPermission>());

            var result = new DepartmentPermissionOverviewDto
            {
                DepartmentId = department.Id,
                DepartmentName = department.Name,
                DepartmentDescription = department.Description,
                AdvancedPermissionsEnabled = advancedPermissionsEnabled,
                CurrentUserIsDepartmentMember = currentDepartmentMembership != null,
                CurrentUserCanManagePermissions = loggedMember.Role == UserRole.Admin || currentUserPermissions.Contains(Permission.ManagePermissions) || currentUserPermissions.Contains(Permission.ManageUserRoles),
                RoleRules = BuildDepartmentRoleRules(),
                Members = departmentMembers.Select(dm => BuildMemberOverview(
                    dm,
                    rolePermissionCache[dm.Member.Role],
                    advancedPermissionsEnabled && customPermissionMap.TryGetValue(dm.MemberId, out var memberCustomPermissions)
                        ? memberCustomPermissions
                        : new List<MemberCustomPermission>(),
                    crossChurchScopeMemberIds.Contains(dm.MemberId)))
                    .ToList()
            };

            return result;
        }

        private static DepartmentMemberPermissionOverviewDto BuildMemberOverview(
            DepartmentMember departmentMember,
            HashSet<Permission> rolePermissions,
            List<MemberCustomPermission> customPermissions,
            bool hasCrossChurchScope)
        {
            var effectivePermissions = BuildEffectivePermissions(departmentMember.Member.Role, rolePermissions, customPermissions);
            var modules = effectivePermissions
                .GroupBy(GetModuleKey)
                .Select(group => BuildModule(group.Key, group.ToList()))
                .OrderBy(m => GetModuleOrder(m.ModuleKey))
                .ThenBy(m => m.ModuleName)
                .ToList();

            return new DepartmentMemberPermissionOverviewDto
            {
                MemberId = departmentMember.MemberId,
                MemberName = departmentMember.Member.Name,
                MemberEmail = departmentMember.Member.Email,
                SystemRole = departmentMember.Member.Role,
                DepartmentRole = departmentMember.Role,
                IsActive = departmentMember.IsActive,
                HasDepartmentScope = departmentMember.IsActive,
                HasFinancialScope = departmentMember.Role is DepartmentMemberRole.Financial or DepartmentMemberRole.Manager || departmentMember.Member.Role == UserRole.Admin,
                HasCrossChurchScope = hasCrossChurchScope,
                CanViewSensitiveData = effectivePermissions.Contains(Permission.ViewMemberSensitiveData),
                HasCustomPermissionOverrides = customPermissions.Count > 0,
                Modules = modules
            };
        }

        private static HashSet<Permission> BuildEffectivePermissions(UserRole role, HashSet<Permission> rolePermissions, List<MemberCustomPermission> customPermissions)
        {
            var permissions = rolePermissions.ToHashSet();

            foreach (var customPermission in customPermissions.Where(cp => cp.IsActive()))
            {
                if (customPermission.IsGranted)
                    permissions.Add(customPermission.Permission);
                else
                    permissions.Remove(customPermission.Permission);
            }

            if (role == UserRole.Admin)
            {
                permissions.Add(Permission.ManagePermissions);
                permissions.Add(Permission.ManageUserRoles);
            }

            return permissions;
        }

        private static List<DepartmentRoleRuleDto> BuildDepartmentRoleRules()
        {
            return new List<DepartmentRoleRuleDto>
            {
                new()
                {
                    Role = DepartmentMemberRole.Member,
                    Name = "Membro do ministério",
                    Description = "Acesso operacional ao próprio ministério, sem finanças nem dados sensíveis por padrão.",
                    CanViewDepartmentData = true,
                    CanViewFinancialData = false,
                    CanManageFinancialData = false,
                    CanViewSensitiveData = false,
                    CanSuperviseBranches = false
                },
                new()
                {
                    Role = DepartmentMemberRole.Financial,
                    Name = "Responsável financeiro",
                    Description = "Pode visualizar e operar dados financeiros do próprio ministério, sem ampliar acesso a dados sensíveis por padrão.",
                    CanViewDepartmentData = true,
                    CanViewFinancialData = true,
                    CanManageFinancialData = true,
                    CanViewSensitiveData = false,
                    CanSuperviseBranches = false
                },
                new()
                {
                    Role = DepartmentMemberRole.Manager,
                    Name = "Gestor do ministério",
                    Description = "Pode acompanhar operação e finanças do próprio ministério; acessos extras dependem da role do sistema.",
                    CanViewDepartmentData = true,
                    CanViewFinancialData = true,
                    CanManageFinancialData = true,
                    CanViewSensitiveData = false,
                    CanSuperviseBranches = false
                },
                new()
                {
                    Role = DepartmentMemberRole.GeneralLeader,
                    Name = "Líder geral",
                    Description = "Pode supervisionar o ministério e, quando houver escopo concedido, acompanhar filiais relacionadas.",
                    CanViewDepartmentData = true,
                    CanViewFinancialData = false,
                    CanManageFinancialData = false,
                    CanViewSensitiveData = false,
                    CanSuperviseBranches = true
                }
            };
        }

        private static PermissionModuleOverviewDto BuildModule(string moduleKey, List<Permission> permissions)
        {
            var items = permissions
                .OrderBy(p => p)
                .Select(permission => new PermissionItemDto
                {
                    Id = (int)permission,
                    Code = permission.ToString(),
                    Name = ToDisplayName(permission.ToString()),
                    IsSensitive = IsSensitivePermission(permission)
                })
                .ToList();

            return new PermissionModuleOverviewDto
            {
                ModuleKey = moduleKey,
                ModuleName = GetModuleName(moduleKey),
                Description = GetModuleDescription(moduleKey),
                CanView = permissions.Any(IsViewPermission),
                CanManage = permissions.Any(IsManagementPermission),
                HasSensitiveDataAccess = permissions.Any(IsSensitivePermission),
                Permissions = items
            };
        }

        private static string GetModuleKey(Permission permission)
        {
            var value = (int)permission;

            return value switch
            {
                >= 1000 and < 2000 => "members",
                >= 2000 and < 3000 => "finances",
                >= 3000 and < 4000 => "events",
                >= 4000 and < 5000 => "worship",
                >= 5000 and < 6000 => "groups",
                >= 6000 and < 7000 => "bible-reading",
                >= 7000 and < 8000 => "journeys",
                >= 8000 and < 9000 => "visitors",
                >= 9000 and < 10000 => "reports",
                >= 10000 and < 11000 => "church-settings",
                >= 11000 and < 12000 => "presentations",
                >= 12000 and < 13000 => "feed",
                >= 13000 and < 14000 => "gamification",
                >= 14000 and < 15000 => "reviews",
                >= 15000 and < 16000 => "families",
                >= 16000 and < 17000 => "system-admin",
                _ => "platform"
            };
        }

        private static string GetModuleName(string moduleKey)
        {
            return moduleKey switch
            {
                "members" => "Membros",
                "finances" => "Finanças",
                "events" => "Eventos",
                "worship" => "Cultos e atividades",
                "groups" => "Grupos",
                "bible-reading" => "Planos bíblicos",
                "journeys" => "Jornadas espirituais",
                "visitors" => "Visitantes",
                "reports" => "Relatórios",
                "church-settings" => "Configurações da igreja",
                "presentations" => "Apresentações e mídia",
                "feed" => "Feed social",
                "gamification" => "Gamificação",
                "reviews" => "Avaliações",
                "families" => "Famílias",
                "system-admin" => "Administração do sistema",
                _ => "Plataforma"
            };
        }

        private static string GetModuleDescription(string moduleKey)
        {
            return moduleKey switch
            {
                "members" => "Dados cadastrais, documentos e ações ligadas aos membros.",
                "finances" => "Doações, caixa, relatórios financeiros e dados bancários.",
                "events" => "Criação, edição e acompanhamento de eventos.",
                "worship" => "Cultos, cronogramas, presenças e pedidos de oração.",
                "groups" => "Gestão de grupos, encontros e recursos.",
                "bible-reading" => "Planos de leitura e progresso bíblico.",
                "journeys" => "Jornadas, trilhas e acompanhamento espiritual.",
                "visitors" => "Cadastro e acompanhamento de visitantes.",
                "reports" => "Dashboards, analytics e exportações.",
                "church-settings" => "Configurações institucionais e dados da igreja.",
                "presentations" => "Apresentações, slides e controle ao vivo.",
                "feed" => "Conteúdo social e publicações.",
                "gamification" => "Conquistas, níveis e desafios.",
                "reviews" => "Avaliações e respostas.",
                "families" => "Dados e gestão de famílias.",
                "system-admin" => "Papéis, permissões e integrações do sistema.",
                _ => "Recursos administrativos globais da plataforma."
            };
        }

        private static int GetModuleOrder(string moduleKey)
        {
            return moduleKey switch
            {
                "members" => 1,
                "finances" => 2,
                "events" => 3,
                "worship" => 4,
                "groups" => 5,
                "bible-reading" => 6,
                "journeys" => 7,
                "visitors" => 8,
                "reports" => 9,
                "church-settings" => 10,
                "presentations" => 11,
                "feed" => 12,
                "gamification" => 13,
                "reviews" => 14,
                "families" => 15,
                "system-admin" => 16,
                _ => 17
            };
        }

        private static bool IsSensitivePermission(Permission permission)
        {
            return permission is Permission.ViewMemberSensitiveData
                or Permission.ViewMemberDocuments
                or Permission.ViewBankingInfo
                or Permission.ViewPlatformFees
                or Permission.ViewAuditLogs;
        }

        private static bool IsViewPermission(Permission permission)
        {
            return permission.ToString().StartsWith("View", StringComparison.Ordinal);
        }

        private static bool IsManagementPermission(Permission permission)
        {
            return !IsViewPermission(permission);
        }

        private static string ToDisplayName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var builder = new StringBuilder();

            for (var i = 0; i < value.Length; i++)
            {
                var current = value[i];
                var previous = i > 0 ? value[i - 1] : '\0';

                if (i > 0 && char.IsUpper(current) && !char.IsUpper(previous))
                    builder.Append(' ');

                builder.Append(current);
            }

            return builder.ToString();
        }
    }
}
