using System.Collections.Generic;
using System.Threading.Tasks;
using DesafioKria.Domain.Entities;

namespace DesafioKria.Domain.Interfaces;

public interface ITollTransactionRepository
{
    Task<List<TollTransaction>> GetTransactionsAsync(int limit);
}
