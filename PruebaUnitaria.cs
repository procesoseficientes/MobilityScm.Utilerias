using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobilityScm.Utilerias
{
    public static class PruebaUnitaria
    {
        static Random random = new Random();
        public static string ObtenerCadenaAleartoria(int stringLength)
        {
            const string caracteresPermitidos = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
            var caracteres = new char[stringLength];

            for (var i = 0; i < stringLength; i++)
            {
                caracteres[i] = caracteresPermitidos[random.Next(0, caracteresPermitidos.Length)];
            }

            return new string(caracteres);
        }

    }
}
