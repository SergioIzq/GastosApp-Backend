using AhorroLand.Shared.Application.Abstractions.Servicies;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AhorroLand.Infrastructure.Servicies
{
    // Clase de implementación en la capa de Infraestructura
    public class DistributedCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        // La dependencia IDistributedCache se inyecta desde el framework
        public DistributedCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        // --- 1. Lectura de Cache (Velocidad) ---

        public async Task<T?> GetAsync<T>(string key)
        {
            // 1. Obtener la cadena de bytes serializada de la caché
            var cachedValue = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(cachedValue))
            {
                return default;
            }

            // 2. Deserializar la cadena a nuestro tipo T
            try
            {
                return JsonSerializer.Deserialize<T>(cachedValue);
            }
            catch
            {
                return default;
            }
        }

        // --- 2. Escritura de Cache (Control de Memoria) ---

        public async Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpiration = null)
        {
            if (value == null) return;

            // 1. Crear las opciones de expiración
            var options = new DistributedCacheEntryOptions();

            if (slidingExpiration.HasValue)
            {
                options.SlidingExpiration = slidingExpiration.Value;
            }

            if (absoluteExpiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = absoluteExpiration.Value;
            }

            // Si no se especifica ninguna, se establece un valor por defecto seguro (ej: 1 hora de Sliding)
            if (!slidingExpiration.HasValue && !absoluteExpiration.HasValue)
            {
                options.SlidingExpiration = TimeSpan.FromHours(1);
            }

            // 2. Serializar el objeto a una cadena JSON
            var jsonValue = JsonSerializer.Serialize(value);

            // 3. Guardar en la caché
            await _cache.SetStringAsync(key, jsonValue, options);
        }

        // --- 3. Validación y Liberación de Cache ---

        public async Task RemoveAsync(string key)
        {
            // Simplemente llama al método de eliminación de la abstracción de .NET
            await _cache.RemoveAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            // Intenta obtener la clave. Si no existe, GetStringAsync retorna null.
            var value = await _cache.GetStringAsync(key);
            return !string.IsNullOrEmpty(value);
        }
    }
}