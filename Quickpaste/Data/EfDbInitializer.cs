using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Data
{
    public class EfDbInitializer
    {

        public static void Initialize(AppDbContext context)
        {
            context.Database.Migrate();
        }
        
    }
}
