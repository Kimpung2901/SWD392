using DAL.Models; // ✅ THÊM DÒNG NÀY

namespace BLL.IService;

public interface IUserCharacterManager
{
    /// <summary>
    /// Ensures UserCharacter is created for a completed CharacterOrder
    /// </summary>
    Task<bool> EnsureUserCharacterForOrderAsync(CharacterOrder characterOrder, string? contextTag = null);
}