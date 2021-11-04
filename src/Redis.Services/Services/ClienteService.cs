using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Redis.Domain.Configuration;
using Redis.Domain.Interfaces.Services;
using Redis.Domain.Models;
using StackExchange.Redis;

namespace Redis.Domain.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IDatabase _dataBase;
        private readonly ConnectionMultiplexer _connection;
        private readonly RedisConfiguration _redisConfiguration;

        public ClienteService(ConnectionMultiplexer connection, ConfigurationManager configuration)
        {
            _connection = connection;
            _dataBase = connection.GetDatabase();
            _redisConfiguration = configuration.RedisConfiguration;
        }

        public async Task SetClienteAsync(Cliente cliente, TimeSpan? expire = null)
            => await _dataBase.StringSetAsync(cliente.Id.ToString(), JsonConvert.SerializeObject(cliente), expire);

        public async Task<Cliente> GetClienteAsync(Guid id)
        {
            var cliente = await _dataBase.StringGetAsync(id.ToString());

            if (cliente == RedisValue.Null)
                return null;

            return JsonConvert.DeserializeObject<Cliente>(cliente);
        }

        public async Task<bool> DeleteClienteAsync(Guid id) => await _dataBase.KeyDeleteAsync(id.ToString());

        public IEnumerable<string> GetClientes()
        {
            var keys = _connection.GetServer($"{_redisConfiguration.HostName}:{_redisConfiguration.Port}").Keys(_redisConfiguration.DefaultDatabase);

            var keysList = new List<string>();

            foreach (var key in keys)
                keysList.Add(key);

            return keysList;
        }
    }
}
