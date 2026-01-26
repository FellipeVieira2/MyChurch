namespace MyChurch.Domain.Enum;

public enum PlanTier
{
    // Nomes padrão
    Starter = 1,
    Essentials = 2,
    Plus = 3,
    Premium = 4,

    // Aliases para compatibilidade com nomes antigos no código
    Free = Starter,
    ProSmall = Essentials,
    Pro = Plus,
    ProPlus = Premium
}
