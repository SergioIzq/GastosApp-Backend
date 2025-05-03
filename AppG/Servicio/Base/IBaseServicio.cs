using AppG.Controllers;
using AppG.Entidades.BBDD;

namespace AppG.Servicio
{
    public interface IBaseServicio<T> where T : Entidad
    {
        Task<ResponseList<T>> GetAllAsync(int idUsuario);
        Task<T> GetByIdAsync(int id);
        Task<T> CreateAsync(T entity);
        Task UpdateAsync(int id, T entity);
        Task DeleteAsync(int id);
        Task<ResponseList<T>> GetCantidadAsync(int page, int size, int idUsuario);
    }
}