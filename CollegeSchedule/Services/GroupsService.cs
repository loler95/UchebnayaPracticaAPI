using CollegeSchedule.Data;
using CollegeSchedule.DTO;
using CollegeSchedule.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeSchedule.Services
{
    public class GroupsService : IGroupsService
    {
        private readonly AppDbContext _db;

        public GroupsService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<GroupsDto>> GetGroups(int course, string? speciality)
        {
            var groupsQuery = _db.StudentGroups.AsQueryable();

            // Фильтрация по специальности, если указана
            if (!string.IsNullOrEmpty(speciality))
            {
                var specialtyEntity = await _db.Specialties
                    .FirstOrDefaultAsync(x => x.Name == speciality);

                if (specialtyEntity == null)
                {
                    return new List<GroupsDto>();
                }

                groupsQuery = groupsQuery.Where(x => x.SpecialtyId == specialtyEntity.Id);
            }

            // Фильтрация по курсу, если указан
            if (course > 0)
            {
                groupsQuery = groupsQuery.Where(x => x.Course == course);
            }

            // Получение групп с жадной загрузкой связанных данных
            var groups = await groupsQuery
                .Include(g => g.Specialty)
                .ToListAsync();

            // Преобразование в DTO
            var result = groups.Select(group => new GroupsDto
            {
                Speciality = group.Specialty?.Name ?? string.Empty,
                Course = group.Course,
                GroupName = group.GroupName
            }).ToList();

            return result;
        }
    }
}