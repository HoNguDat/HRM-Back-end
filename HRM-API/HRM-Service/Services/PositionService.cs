using HRM_Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HRM_Service.Services
{
    public interface IPositionService
    {
        Task<List<Position>> GetAllPositions();
        Task<Position> AddPosition(Position position);
        Task<Position> UpdatePosition(Guid id, Position position);
        Task<Position> GetPositionByName(string name);
        Task<Position> GetPositionById(Guid id);
        void DeletePosition(Guid id);
    }
    public class PositionService : IPositionService
    {
        private readonly ApplicationDbContext _context;
        public PositionService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Position>> GetAllPositions()
        {
            var query = await _context.Positions.Include(p => p.ApplicationUsers).ToListAsync();
            return query;
        }

        public async Task<Position> GetPositionByName(string name)
        {
            var position = await _context.Positions.FirstOrDefaultAsync(p => p.Name == name);
            return position;
        }

        public async Task<Position> GetPositionById(Guid id)
        {
            var position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
            return position;
        }

        public async Task<Position> AddPosition(Position position)
        {
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            return position;
        }

        public async Task<Position> UpdatePosition(Guid id, Position position)
        {
            position.Id = id;
            _context.Entry(position).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return position;
        }

        public void DeletePosition(Guid id)
        {
            var position = _context.Positions.Find(id);

            if (position == null)
            {
                throw new Exception("Id không tồn tại");
            }

            _context.Entry(position).State = EntityState.Deleted;
            _context.SaveChanges();
        }
    }
}
