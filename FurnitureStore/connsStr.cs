using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore
{
    public static class connStr
    {
        public static string ConnectionString { get; } = @"host=localhost;
                                                           uid=root;
                                                           pwd=root;
                                                           database=furniture_store;";
    }
}
