namespace NewShoreAir.DataAccess.UnitOfWork
{
    public class Provider(IServiceProvider serviceProvider) : IProvider
    {
        private Hashtable _unitOfWorkInstances;

        public T CrearNuevaInstancia<T>() where T : new()
        {
            return Activator.CreateInstance<T>();
        }

        public T ObtenerServicio<T>()
        {
            return serviceProvider.GetRequiredService<T>();
        }

        public T ObtenerUnitOfWork<T>() where T : IUnitOfWork
        {
            _unitOfWorkInstances ??= new();

            Type unitOfWorkType = typeof(T);

            if (_unitOfWorkInstances.ContainsKey(unitOfWorkType))
                return (T)_unitOfWorkInstances[unitOfWorkType];

            T unitOfWork = serviceProvider.GetRequiredService<T>();
            _unitOfWorkInstances.Add(unitOfWorkType, unitOfWork);

            return unitOfWork;
        }
    }
}