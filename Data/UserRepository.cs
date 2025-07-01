using AutoMapper;
using AutoMapper.QueryableExtensions;
using Graduation.DTOs;
using Graduation.Entities;
using Graduation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Graduation.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        // ✅ جلب كل المراقبين (Sight)
        public async Task<IEnumerable<MemberDto>> GetSightMembersAsync()
        {
            return await _context.Users
                .Where(u => u.UserRoles.Any(r => r.Role.Name == "Sight"))
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // ✅ جلب كل المكفوفين (Blind)
        public async Task<IEnumerable<MemberDto>> GetBlindMembersAsync()
        {
        return await _context.Users
            .Where(u => u.Role == "blind") // ✅ هنا التعديل
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        }


        public async Task<AppUser?> GetLinkedBlindUser(int sightId)
        {
        return await _context.UserConnections
            .Where(c => c.SightId == sightId)
            .Select(c => c.Blind)
            .FirstOrDefaultAsync();
        }

        public async Task<AppUser> GetSightedByBlindId(int blindId)
        {
        return await _context.UserConnections
        .Where(c => c.BlindId == blindId)
        .Select(c => c.Sight)
        .FirstOrDefaultAsync();
        }



    }
}
