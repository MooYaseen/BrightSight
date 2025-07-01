using AutoMapper;
using Graduation.Data;
using Graduation.DTOs;
using Graduation.Entities;
using Graduation.Extensions;
using Graduation.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Graduation.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly DataContext _context;

        public UsersController(IUserRepository userRepository, IMapper mapper,
            IPhotoService photoService, DataContext context)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user);

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
{
    var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

    if (user == null) return NotFound();

    // حذف أي صورة موجودة قبل إضافة الجديدة
    var existingPhoto = user.Photos.FirstOrDefault();
    if (existingPhoto != null)
    {
        await _photoService.DeletePhotoAsync(existingPhoto.PublicId);
        user.Photos.Remove(existingPhoto);
    }

    var result = await _photoService.AddPhotoAsync(file);
    if (result.Error != null) return BadRequest(result.Error.Message);

    var photo = new Photo
    {
        Url = result.SecureUrl.AbsoluteUri,
        PublicId = result.PublicId,
        IsMain = true
    };

    user.Photos.Add(photo);

    if (await _userRepository.SaveAllAsync())
    {
        return CreatedAtAction(nameof(GetUser),
            new { username = user.UserName },
            _mapper.Map<PhotoDto>(photo));
    }

    return BadRequest("Problem adding photo");
}


        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("this is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Problem setting the main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting photo");
        }

        // ✅ الربط من Blind إلى Sight
        [HttpPost("link-to-sight")]
        public async Task<ActionResult> LinkToSight([FromBody] LinkRequestDto dto)
        {
        var username = User.GetUsername(); // ده الكفيف اللي سجل دخول
        var blindUser = await _userRepository.GetUserByUsernameAsync(username);
        if (blindUser == null) return Unauthorized("Blind user not found");

        var sightUser = await _userRepository.GetUserByUsernameAsync(dto.SightUsername);
        if (sightUser == null) return NotFound("Sight user not found");

        var connection = new UserConnection
        {
        BlindId = blindUser.Id,
        SightId = sightUser.Id
        };

        _context.UserConnections.Add(connection);
        await _context.SaveChangesAsync();

        return Ok("Linked successfully");
        }


        // ✅ الربط من Sight إلى Blind (العكسي)
        [HttpPost("link-to-blind")]
        public async Task<ActionResult> LinkToBlind([FromBody] LinkRequestDto dto)
        {
        var username = User.GetUsername(); // ده الـ Sight اللي سجل دخول
        var sightUser = await _userRepository.GetUserByUsernameAsync(username);
        if (sightUser == null) return Unauthorized("Sight user not found");

        var blindUser = await _userRepository.GetUserByUsernameAsync(dto.BlindUsername);
        if (blindUser == null) return NotFound("Blind user not found");

        var connection = new UserConnection
        {
        BlindId = blindUser.Id,
        SightId = sightUser.Id
        };

        _context.UserConnections.Add(connection);
        await _context.SaveChangesAsync();

        // ✅ رجّع اسم الكفيف كـ JSON
        return Ok(new { blindName = blindUser.KnownAs });
        }


        // ✅ جلب كل الـ Blind
        [HttpGet("blinds")]
        public async Task<ActionResult> GetBlindUsers()
        {
            var blinds = await _userRepository.GetBlindMembersAsync();
            return Ok(new
            {
                count = blinds.Count(),
                data = blinds
            });
        }

        [HttpGet("linked-blind")]
        public async Task<ActionResult<SimpleUserDto>> GetLinkedBlind()
        {
        var username = User.GetUsername();
         var sightUser = await _userRepository.GetUserByUsernameAsync(username);
         if (sightUser == null) return Unauthorized();

        var connection = await _context.UserConnections
        .Include(c => c.Blind)
        .ThenInclude(u => u.Photos)
        .FirstOrDefaultAsync(c => c.SightId == sightUser.Id);

         if (connection == null || connection.Blind == null)
        return NotFound("No linked blind user");

        return new SimpleUserDto
        {
        Id = connection.Blind.Id,
        Username = connection.Blind.UserName,
        DisplayName = connection.Blind.KnownAs,
        LastActive = connection.Blind.LastActive,
        PhotoUrl = connection.Blind.Photos.FirstOrDefault(p => p.IsMain)?.Url

        };
        }

        [HttpGet("linked-sight")]
        public async Task<ActionResult<SimpleUserDto>> GetLinkedSighted()
        {
        var username = User.GetUsername();
        var blindUser = await _userRepository.GetUserByUsernameAsync(username);
        if (blindUser == null) return Unauthorized();


        var connection = await _context.UserConnections
        .Include(c => c.Sight)
        .ThenInclude(u => u.Photos)
        .FirstOrDefaultAsync(c => c.BlindId == blindUser.Id);

        if (connection == null || connection.Sight == null)
        return NotFound("No linked sight user");

        return new SimpleUserDto
        {
        Id = connection.Sight.Id,
        Username = connection.Sight.UserName,
        DisplayName = connection.Sight.KnownAs,
        LastActive = connection.Sight.LastActive,
        PhotoUrl = connection.Sight.Photos.FirstOrDefault(p => p.IsMain)?.Url
        };
        }




        [HttpPost("update-location")]
        public async Task<ActionResult> UpdateLocation([FromBody] LocationDto locationDto)
        {
        var username = User.GetUsername();
        var user = await _userRepository.GetUserByUsernameAsync(username);

        if (user == null) return NotFound();

        user.Latitude = locationDto.Latitude;
        user.Longitude = locationDto.Longitude;

        if (await _userRepository.SaveAllAsync()) return Ok("Location updated");

        return BadRequest("Failed to update location");
        }


        [HttpGet("blind-location")]
        public async Task<ActionResult<LocationDto>> GetBlindLocation()
        {
        var sightUsername = User.GetUsername();
        var connection = await _context.UserConnections
        .FirstOrDefaultAsync(c => c.Sight.UserName == sightUsername);

        if (connection == null) return NotFound("No linked blind user found");

        var blind = await _userRepository.GetUserByIdAsync(connection.BlindId);

        if (blind == null) return NotFound("Blind user not found");

        return Ok(new LocationDto
        {
        Latitude = blind.Latitude ?? 0.0,
        Longitude = blind.Longitude ?? 0.0
        });
        }
        
    }
}
