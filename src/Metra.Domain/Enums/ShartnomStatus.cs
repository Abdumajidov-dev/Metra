namespace Metra.Domain.Enums;

/// <summary>
/// Shartnoma holati
/// </summary>
public enum ShartnomStatus
{
    /// <summary>
    /// Faol shartnoma
    /// </summary>
    Active = 1,

    /// <summary>
    /// Bekor qilingan shartnoma
    /// </summary>
    Cancelled = 2,

    /// <summary>
    /// Tugallangan shartnoma
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Kutilayotgan shartnoma
    /// </summary>
    Pending = 4
}

/// <summary>
/// To'lov turi
/// </summary>
public enum TolovTuri
{
    /// <summary>
    /// Naqd pul
    /// </summary>
    Naqd = 1,

    /// <summary>
    /// Plastik karta
    /// </summary>
    Karta = 2,

    /// <summary>
    /// Bank o'tkazmasi
    /// </summary>
    BankOtkazma = 3,

    /// <summary>
    /// Aralash to'lov
    /// </summary>
    Aralash = 4
}

/// <summary>
/// Foydalanuvchi roli
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Administrator
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Menejer
    /// </summary>
    Manager = 2,

    /// <summary>
    /// Kassir
    /// </summary>
    Cashier = 3,

    /// <summary>
    /// Omborchi
    /// </summary>
    Warehouseman = 4
}
