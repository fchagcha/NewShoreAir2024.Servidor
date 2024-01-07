using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace NewShoreAir.DataAccess.UnitOfWork
{
    public class UnitOfWorkProvider : IUnitOfWorkProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private Hashtable _unitOfWorkInstances;

        public UnitOfWorkProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T ObtenerServicio<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        public T ObtenerUnitOfWork<T>() where T : IUnitOfWork
        {
            _unitOfWorkInstances ??= new();

            Type unitOfWorkType = typeof(T);

            if (_unitOfWorkInstances.ContainsKey(unitOfWorkType))
                return (T)_unitOfWorkInstances[unitOfWorkType];

            T unitOfWork = _serviceProvider.GetRequiredService<T>();
            _unitOfWorkInstances.Add(unitOfWorkType, unitOfWork);

            return unitOfWork;
        }
    }
}