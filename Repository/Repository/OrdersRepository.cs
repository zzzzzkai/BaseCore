
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.IRepository;

namespace Repository.Repository
{
	/// <summary>
	/// IOrderRepository
	/// </summary>	
    public class OrdersRepository : BaseRepository<Orders>,IOrdersRepository
    {

    }
}

	