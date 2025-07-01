using Graduation.DTOs;
using Graduation.Entities;

namespace Graduation.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<IEnumerable<MemberDto>> GetMembersAsync();
        Task<MemberDto> GetMemberAsync(string username);

        Task<IEnumerable<MemberDto>> GetSightMembersAsync();
        Task<IEnumerable<MemberDto>> GetBlindMembersAsync();

        Task<AppUser?> GetLinkedBlindUser(int sightId);
        Task<AppUser> GetSightedByBlindId(int blindId);
    }
}
