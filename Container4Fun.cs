using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MobilityScm.Utilerias
{
    
    [Serializable]
    public  class CMvx
    {
        private  readonly IList<TypeRegister> Types = new List<TypeRegister>();

        /// <summary>
        /// Registrar un objeto en un contenedor IoC
        /// </summary>
        /// <typeparam name="TContract">Representa una interface</typeparam>
        /// <typeparam name="TImplementation">Representante de la ejecución del contrato</typeparam>
        public  void Register<TContract, TImplementation>()
        {
            var typeFound = from t in Types
                            where t.Key == typeof(TContract) && t.Implementation == typeof(TImplementation)
                            select t;
            if (!typeFound.Any())
                Types.Add(new TypeRegister { Key = typeof(TContract), Implementation = typeof(TImplementation) });

        }

        public  void RegisterType<TContract, TImplementation>()
        {
            Register<TContract, TImplementation>();
        }

        /// <summary>
        /// Registrar un objeto en un contenedor IoC
        /// </summary>
        /// <typeparam name="TContract">Representa una interface</typeparam>
        /// <typeparam name="TImplementation">Representa la implementacion del contracto</typeparam>
        public  void Register<TContract, TImplementation>(string name)
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
        public  void Register<TContract, TImplementation>(string name, TImplementation instance)
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
        public  void Register<TContract, TImplementation>(string name, TImplementation instance, PropertiesTypeCondition propertiesCondition)
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
        public  void Register<TContract, TImplementation>(string name, TImplementation instance, PropertiesTypeCondition propertiesCondition, InstanceTypeCondition instanceCondition)
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


        public  void LazyConstructAndRegisterSingleton<TContract, TImplementation>()
        {
            TImplementation obj = default(TImplementation);
            Register<TContract, TImplementation>(null, obj, PropertiesTypeCondition.DisposeAll, InstanceTypeCondition.Share);
        }

        public  void ConstructAndRegisterSingleton<TContract, TImplementation>()
        {
            LazyConstructAndRegisterSingleton<TContract, TImplementation>();
            Resolve<TContract>();
        }

        public  void RegisterSingleton<TContract, TImplementation>(TImplementation instance)
        {
            Register<TContract, TImplementation>(null, instance, PropertiesTypeCondition.DisposeAll, InstanceTypeCondition.Share);
        }


        public  T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }
        public  object Resolve(Type contract)
        {
            return Resolve(contract, null);
        }

        private  object Resolve(Type contract, string name, PropertiesTypeCondition propertiesCondition = PropertiesTypeCondition.Search)
        {
            object newInstance = null;
            TypeRegister typeRegistered = null;
            var typesFound = from t in Types
                             where t.Key == contract
                             select t;
            var typeRegisters = typesFound as IList<TypeRegister> ?? typesFound.ToList();
            int totalTypesFound = typeRegisters.Count();
            if (!typeRegisters.Any() && propertiesCondition != PropertiesTypeCondition.AllowNull)
            {
                throw new ArgumentNullException(string.Format("Type {0} is not registered ", contract.Name));
            }

            switch (totalTypesFound)
            {
                case 1:
                    typeRegistered = typeRegisters.First();
                    break;
                default:
                    var typeChosen = from t in typeRegisters
                                     where t.Name == name
                                     select t;
                    var enumerable = typeChosen as IList<TypeRegister> ?? typeChosen.ToList();
                    if (!enumerable.Any())
                    {
                        var typeNotNamed = from t in typeRegisters
                                           where string.IsNullOrEmpty(t.Name)
                                           select t;
                        typeRegistered = typeNotNamed.First();
                    }
                    else
                        typeRegistered = enumerable.First();
                    break; 
            }
            

            if (typeRegistered != null)
            {
                Type implementation = typeRegistered.Implementation;

                ConstructorInfo constructor = implementation.GetConstructors()[0];
                ParameterInfo[] constructorParameters = constructor.GetParameters();
                PropertyInfo[] propertiesParameters = implementation.GetProperties();
                if (constructorParameters.Length == 0 && propertiesParameters.Length == 0)
                {
                    if (typeRegistered.Instance == null && typeRegistered.InstanceCondition == InstanceTypeCondition.Share)
                        typeRegistered.Instance = Activator.CreateInstance(implementation);
                    return typeRegistered.Instance ?? Activator.CreateInstance(implementation);
                }

                var parameters = new List<object>(constructorParameters.Length);
                if (typeRegistered.Instance == null || typeRegistered.InstanceCondition == InstanceTypeCondition.NonShare)
                    parameters.AddRange(constructorParameters.Select(parameterInfo => Resolve(parameterInfo.ParameterType)));


                if (typeRegistered.Instance == null && typeRegistered.InstanceCondition == InstanceTypeCondition.Share)
                    typeRegistered.Instance = constructor.Invoke(parameters.ToArray());

                newInstance = typeRegistered.Instance ?? constructor.Invoke(parameters.ToArray());

                if (typeRegistered.PropertiesCondition != PropertiesTypeCondition.DisposeAll)
                    foreach (PropertyInfo propertyInfo in propertiesParameters)
                        if (propertyInfo.PropertyType.IsInterface)
                        {
                            object value = Resolve(propertyInfo.PropertyType, propertyInfo.Name, propertiesCondition);
                            if (value != null)
                                propertyInfo.SetValue(newInstance, value, null);
                        }
            }
            return newInstance;
        }
    }    
}
