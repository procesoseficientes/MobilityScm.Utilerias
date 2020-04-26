using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MobilityScm.Utilerias
{

    [Serializable]
    public class CMvx
    {
        private readonly IList<TypeRegister> Types = new List<TypeRegister>();

        /// <summary>
        /// Registrar un objeto en un contenedor IoC
        /// </summary>
        /// <typeparam name="TContract">Representa una interface</typeparam>
        /// <typeparam name="TImplementation">Representante de la ejecución del contrato</typeparam>
        public void Register<TContract, TImplementation>()
        {
            var typeFound = from t in Types
                            where t.Key == typeof(TContract) && t.Implementation == typeof(TImplementation)
                            select t;
            if (!typeFound.Any())
                Types.Add(new TypeRegister { Key = typeof(TContract), Implementation = typeof(TImplementation) });

        }

        public void RegisterType<TContract, TImplementation>()
        {
            Register<TContract, TImplementation>();
        }

        /// <summary>
        /// Registrar un objeto en un contenedor IoC
        /// </summary>
        /// <typeparam name="TContract">Representa una interface</typeparam>
        /// <typeparam name="TImplementation">Representa la implementacion del contracto</typeparam>
        public void Register<TContract, TImplementation>(string name)
        {
            var typeFound = from t in Types
                            where t.Key == typeof(TContract) && t.Implementation == typeof(TImplementation) && t.Name == name
                            select t;
            if (!typeFound.Any())
                Types.Add(new TypeRegister { Key = typeof(TContract), Implementation = typeof(TImplementation), Name = name });

        }

        /// <summary>
        /// Registrar un objeto en un contenedor IoC
        /// </summary>
        /// <typeparam name="TContract">Representa una interface</typeparam>
        /// <typeparam name="TImplementation">Representa la implementacion del contrato</typeparam>
        /// <param name="name"></param>
        /// <param name="instance">Establecer una instancia de una aplicación ya creada</param>
        public void Register<TContract, TImplementation>(string name, TImplementation instance)
        {
            Register<TContract, TImplementation>(name, instance, PropertiesTypeCondition.DisposeAll);
        }

        /// <summary>
        /// Registrar un objeto en un contenedor IoC
        /// </summary>
        /// <typeparam name="TContract">Represent an interface</typeparam>
        /// <typeparam name="TImplementation">Representa la implementacion del contrato</typeparam>
        /// <param name="name"></param>
        /// <param name="instance">Establecer una instancia de una aplicación ya creada</param>
        /// <param name="propertiesCondition"></param>
        public void Register<TContract, TImplementation>(string name, TImplementation instance, PropertiesTypeCondition propertiesCondition)
        {
            var typeFound = from t in Types
                            where t.Key == typeof(TContract) && t.Implementation == typeof(TImplementation)
                            select t;
            if (!typeFound.Any())
                Types.Add(new TypeRegister { Key = typeof(TContract), Implementation = typeof(TImplementation), Name = name, Instance = instance, PropertiesCondition = propertiesCondition });
        }

        /// <summary>
        /// Registrar un objeto en un contenedor IoC
        /// </summary>
        /// <typeparam name="TContract">Representa la implementacion del contrato</typeparam>
        /// <typeparam name="TImplementation">Representa the implementation of the contract</typeparam>
        /// <param name="name"></param>
        /// <param name="instance">Establecer una instancia de una aplicación ya creada</param>
        /// <param name="propertiesCondition"></param>
        /// <param name="instanceCondition">Share = Usa la misma instancia, NonShare = Crear una nueva instancia cada vez que</param>
        public void Register<TContract, TImplementation>(string name, TImplementation instance, PropertiesTypeCondition propertiesCondition, InstanceTypeCondition instanceCondition)
        {
            var typeFound = from t in Types
                            where t.Key == typeof(TContract) && t.Implementation == typeof(TImplementation)
                            select t;
            var typeRegisters = typeFound as TypeRegister[] ?? typeFound.ToArray();
            if (!typeRegisters.Any())
                Types.Add(new TypeRegister { Key = typeof(TContract), Implementation = typeof(TImplementation), Name = name, Instance = instance, PropertiesCondition = propertiesCondition, InstanceCondition = instanceCondition });
            else
            {
                typeRegisters.First().Instance = instance;
            }
        }


        public void LazyConstructAndRegisterSingleton<TContract, TImplementation>()
        {
            TImplementation obj = default(TImplementation);
            Register<TContract, TImplementation>(null, obj, PropertiesTypeCondition.DisposeAll, InstanceTypeCondition.Share);
        }

        public void ConstructAndRegisterSingleton<TContract, TImplementation>()
        {
            LazyConstructAndRegisterSingleton<TContract, TImplementation>();
            Resolve<TContract>();
        }

        public void RegisterSingleton<TContract, TImplementation>(TImplementation instance)
        {
            Register<TContract, TImplementation>(null, instance, PropertiesTypeCondition.DisposeAll, InstanceTypeCondition.Share);
        }


        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }
        public object Resolve(Type contract)
        {
            return Resolve(contract, null);
        }

        private object Resolve(Type contract, string name, PropertiesTypeCondition propertiesCondition = PropertiesTypeCondition.Search)
        {
            var typesFound = from t in Types
                             where t.Key == contract
                             select t;
            var typeRegisters = typesFound as IList<TypeRegister> ?? typesFound.ToList();

            ValidateTypeRegistersAndPropertiesCondition(typeRegisters, propertiesCondition, contract);

            var typeRegistered = ValidateTotalTypesFound(typeRegisters, name);

            if (typeRegistered == null) return null;

            var implementation = typeRegistered.Implementation;

            var constructor = implementation.GetConstructors()[0];
            var constructorParameters = constructor.GetParameters();
            var propertiesParameters = implementation.GetProperties();

            if (ValidateConstructorAndPropertiesParameters(constructorParameters, propertiesParameters))
            {
                if (ValidateInstance(typeRegistered, InstanceTypeCondition.Share))
                    typeRegistered.Instance = Activator.CreateInstance(implementation);
                return typeRegistered.Instance ?? Activator.CreateInstance(implementation);
            }

            var parameters = new List<object>(constructorParameters.Length);

            PopulateInstanceParameters(parameters, typeRegistered, constructorParameters);

            if (ValidateInstance(typeRegistered, InstanceTypeCondition.Share))
                typeRegistered.Instance = constructor.Invoke(parameters.ToArray());

            var newInstance = typeRegistered.Instance ?? constructor.Invoke(parameters.ToArray());

            if (typeRegistered.PropertiesCondition == PropertiesTypeCondition.DisposeAll) return newInstance;

            foreach (var propertyInfo in propertiesParameters)
                if (propertyInfo.PropertyType.IsInterface)
                {
                    var value = Resolve(propertyInfo.PropertyType, propertyInfo.Name, propertiesCondition);
                    if (value != null)
                        propertyInfo.SetValue(newInstance, value, null);
                }
            return newInstance;
        }

        private static void ValidateTypeRegistersAndPropertiesCondition(IList<TypeRegister> typeRegisters, PropertiesTypeCondition propertiesCondition, Type contract)
        {
            if (!typeRegisters.Any() && propertiesCondition != PropertiesTypeCondition.AllowNull)
                throw new ArgumentNullException($"Type {contract.Name} is not registered ");
        }

        private static TypeRegister ValidateTotalTypesFound(IList<TypeRegister> typeRegisters, string name)
        {
            var types = typeRegisters.Count();

            if (types.Equals(1))
            {
                return typeRegisters.First();
            }

            var typeChosen = from t in typeRegisters
                             where t.Name == name
                             select t;

            var enumerable = typeChosen as IList<TypeRegister> ?? typeChosen.ToList();

            if (enumerable.Any()) return enumerable.First();

            var typeNotNamed = from t in typeRegisters
                               where string.IsNullOrEmpty(t.Name)
                               select t;

            return typeNotNamed.First();
        }

        private static bool ValidateConstructorAndPropertiesParameters(ICollection<ParameterInfo> constructorParameters, ICollection<PropertyInfo> propertiesParameters)
        {
            return constructorParameters.Count == 0 && propertiesParameters.Count == 0;
        }

        private static bool ValidateInstance(TypeRegister typeRegistered, InstanceTypeCondition instanceTypeCondition)
        {
            return typeRegistered.Instance == null && typeRegistered.InstanceCondition == instanceTypeCondition;
        }

        private void PopulateInstanceParameters(List<object> parameters, TypeRegister typeRegistered, IEnumerable<ParameterInfo> constructorParameters)
        {
            if (ValidateInstance(typeRegistered, InstanceTypeCondition.NonShare))
                parameters.AddRange(constructorParameters.Select(parameterInfo => Resolve(parameterInfo.ParameterType)));
        }
    }
}
