using DataModel;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Repository
{
   public class PayRepository : BaseRepository<PayRecord>, IPayRepository
    {
    }
}
