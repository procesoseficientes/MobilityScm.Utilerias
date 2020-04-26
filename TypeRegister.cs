using System;

namespace MobilityScm.Utilerias
{
    /// <summary>
    /// Condiciones de la propiedad: Search = significa buscar la implmentacion e injectarla, DisposeAll = No toma ninguna propiedad para inyectar la aplicación, AllowNull = Permite que algunas de las propiedades de ser nula, sin producir una excepción
    /// </summary>
    public enum PropertiesTypeCondition { Search, DisposeAll, AllowNull }
    /// <summary>
    /// InstanceTypeCondition: Compartir la misma instancia registrada o crear una nueva instancia en su lugar (Opcion NonShare)
    /// </summary>
    public enum InstanceTypeCondition { NonShare, Share }

    [Serializable]
    public class TypeRegister
    {
        /// <summary>
        /// Key = Representa a Contracto o Tipo de Interface 
        /// </summary>
        public Type Key { get; set; }
        /// <summary>
        /// Implementation = Tipo de ua clase en concreto
        /// </summary>
        public Type Implementation { get; set; }
        /// <summary>
        /// nombre de la propiedad que va ser injectada 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Instancia que ya fue  createda en  la memoria de la implementacion 
        /// </summary>
        public object Instance { get; set; }
        /// <summary>
        /// Condiciones de la propiedad: Search = significa buscar la implmentacion e injectarla, DisposeAll = No toma ninguna propiedad para inyectar la aplicación, AllowNull = Permite que algunas de las propiedades de ser nula, sin producir una excepción
        /// </summary>
        public PropertiesTypeCondition PropertiesCondition { get; set; }
        /// <summary>
        /// InstanceTypeCondition: Compartir la misma instancia registrada o crear una nueva instancia en su lugar (Opcion NonShare)
        /// </summary>
        public InstanceTypeCondition InstanceCondition { get; set; }
    }
}
