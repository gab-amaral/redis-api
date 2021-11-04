using System;

namespace Redis.Domain.Models
{
    public class Cliente
    {
        public Guid Id { get; set; }
        public string PrimeiroNome { get; set; }
        public string UltimoNome { get; set; }
    }
}
