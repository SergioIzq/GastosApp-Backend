using AppG.Controllers;
using AppG.Entidades.BBDD;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppG.Servicio
{
    public interface IBaseServicio<T> where T : class
    {
        Task<ResponseList<T>> GetAllAsync<T>(int idUsuario) where T : class, IEntidad;
        Task<T> GetByIdAsync(int id);
        Task<T> CreateAsync(T entity);
        Task UpdateAsync(int id, T entity);
        Task DeleteAsync(int id);
        Task<ResponseList<T>> GetCantidadAsync<T>(int page, int size, int idUsuario) where T : class, IEntidad;
    }
}