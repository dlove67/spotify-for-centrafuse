using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace Spotify
{
    internal static class Utility
    {
        public static string GetDefaultLocationPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CFSpotify");
        }

        public static void DisposeTableDisposables(DataTable table)
        {
            var disposableTypeColumns = table.Columns.OfType<DataColumn>().Where(column => column.DataType.GetInterfaces().Any(i => i.Equals(typeof(IDisposable))));

            var disposables = table.AsEnumerable().SelectMany(row =>
            {
                List<IDisposable> rowDisposables = new List<IDisposable>();
                foreach (var column in disposableTypeColumns)
                {
                    rowDisposables.Add((IDisposable)row[column]);
                }
                return rowDisposables;
            });

            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
