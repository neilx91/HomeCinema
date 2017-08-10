using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCinema.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        HomeCinemaContext _dbContext;

        public HomeCinemaContext Init()
        {
            return _dbContext ?? (_dbContext = new HomeCinemaContext());
        }

        protected override void DisposeCore()
        {
            if(_dbContext != null)
                _dbContext.Dispose();
        }
    }

    public interface IDbFactory : IDisposable
    {
        HomeCinemaContext Init();
    }
}
