using Biograf_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DataContext _context;

        public GenericRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                var entities = await _context.Set<T>().ToListAsync();
              
                if (entities == null || !entities.Any())
                {
                    throw new Exception($"No records found for the entity type {typeof(T).Name}.");
                }

                return entities;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving records for the entity type {typeof(T).Name}: {ex.Message}");
            }
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Invalid ID value.");
                }

                var entity = await _context.Set<T>().FindAsync(id);
                
                if (entity == null)
                {
                    throw new Exception($"Entity of type {typeof(T).Name} with ID {id} not found.");
                }

                return entity;
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the entity with ID {id}: {ex.Message}");
            }
        }

        public async Task<T> GetByNameAsync(string name)
        {
            try
            {          
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Name cannot be null or empty.");
                }

                var entity = await _context.Set<T>()
                    .FirstOrDefaultAsync(e => EF.Property<string>(e, "Name") == name);

                if (entity == null)
                {
                    throw new Exception($"Entity of type {typeof(T).Name} with the name '{name}' not found.");
                }

                return entity;
            }
            catch (ArgumentException ex)
            {              
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {              
                throw new Exception($"An error occurred while retrieving the entity by name: {ex.Message}");
            }
        }

        public async Task AddAsync(T entity)
        {
            try
            {                
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
                }

                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding the entity: {ex.Message}");
            }
        }

        public async Task UpdateAsync(T entity)
        {
            try
            {           
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
                }

                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentNullException ex)
            {              
                throw new Exception($"Error: {ex.Message}");
            }          
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the entity: {ex.Message}");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Invalid ID value.");
                }

                var entity = await GetByIdAsync(id);
                
                if (entity == null)
                {
                    throw new Exception($"Entity of type {typeof(T).Name} with ID {id} not found.");
                }

                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting the entity with ID {id}: {ex.Message}");
            }
        }

        public async Task DeleteByNameAsync(string name)
        {
            try
            {               
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Name cannot be null or empty.");
                }

                var entity = await _context.Set<T>().FirstOrDefaultAsync(e => EF.Property<string>(e, "Name") == name);
                
                if (entity == null)
                {
                    throw new Exception($"Entity of type {typeof(T).Name} with the name '{name}' not found.");
                }

                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting the entity with the name '{name}': {ex.Message}");
            }
        }

        public async Task UpdateByNameAsync(string name, T entity)
        {
            try
            {             
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Name cannot be null or empty.");
                }

                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
                }

                var existingEntity = await _context.Set<T>().FirstOrDefaultAsync(e => EF.Property<string>(e, "Name") == name);
              
                if (existingEntity == null)
                {
                    throw new ArgumentException($"Entity with Name '{name}' not found.");
                }

                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }          
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the entity with Name '{name}': {ex.Message}");
            }
        }
    }
}
