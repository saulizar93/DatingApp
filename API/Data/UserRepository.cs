using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        // we need a constructor to inject the DbContext
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == username );
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            // add an Include to include photos with our response
            return await _context.Users.Include(p => p.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            // if something saves successfully, it will return a value greater than 0
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            // adds a flag to the entity to show it's been modified
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users
                    .AsQueryable(); 
            //AsNoTracking turns off tracking in EntityFramework, all we need to do is read this, nothing else
            //AsQueryable lets us do something with the query to decide what to filter by
            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);

            var minDoD = DateTime.Today.AddYears(-userParams.MaxAge -1);
            var maxDoD = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(u => u.DateOfBirth >= minDoD && u.DateOfBirth <= maxDoD);
            

            //default filter option is labeled "_"
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u=>u.LastActive),
            };

            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(), userParams.PageNumber, userParams.PageSize);
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            // return await _context.Users.Where(x => x.UserName == username)
            //     .Select(user => new MemberDto
            //                         {
            //                             Id = user.Id,
            //                             Username = user.UserName,
            //                             .... etc
            //                         }).SingleOrDefaultAsync();
            return await _context.Users.Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }
    }
}