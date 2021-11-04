using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Redis.Domain.Models;

namespace Redis.Domain.Interfaces.Services
{
    public interface IClienteService
    {
        Task<Cliente> GetClienteAsync(Guid id);
        IEnumerable<string> GetClientes();
        Task SetClienteAsync(Cliente cliente, TimeSpan? expire = null);
        Task<bool> DeleteClienteAsync(Guid id);
    }
}
