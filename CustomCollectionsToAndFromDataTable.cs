using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace MobilityScm.Utilerias
{
    public static class CustomCollectionsToAndFromDataTable
    {
        public static DataTable ConvertTo<T>(IList<T> list)
        {
            var table = CreateTable<T>();
            var entityType = typeof(T);
            var properties = TypeDescriptor.GetProperties(entityType);

            foreach (var item in list)
            {
                var row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item);
                }

                table.Rows.Add(row);
            }

            return table;
        }

        public static IList<T> ConvertTo<T>(IList<DataRow> rows)
        {
            IList<T> list = null;

            if (rows != null)
            {
                list = rows.Select(CreateItem<T>).ToList();
            }

            return list;
        }

        public static IList<T> ConvertTo<T>(DataTable table)
        {
            var rows = table.Rows.Cast<DataRow>().ToList();

            return ConvertTo<T>(rows);
        }

        public static T CreateItem<T>(DataRow row)
        {
            var obj = default(T);
            if (row == null) return obj;
            obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in row.Table.Columns)
            {
                var prop = obj.GetType().GetProperty(column.ColumnName);
                try
                {
                    var value = row[column.ColumnName];
                    if (value != DBNull.Value && prop != null)
                            prop.SetValue(obj, value, null);
                }
                catch (ArgumentException e)
                {
                    throw new ArgumentNullException(e.Message);
                }
            }

            return obj;
        }

        public static DataTable CreateTable<T>()
        {
            var entityType = typeof(T);
            var table = new DataTable(entityType.Name);
            var properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            return table;
        }
    }
}
