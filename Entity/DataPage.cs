using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Entity
{
    public class DataPage
    {
        public static List<T> GetPage<T>(IQueryable<T> source, int pageSize, int pageIndex, ref int pageCount, ref int total) where T : class
        {
            total = source.Count();

            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0) pageSize = total;
            if (pageSize <= 0) pageSize = 10;

            pageCount = total % pageSize == 0 ? (total / pageSize) : (total / pageSize + 1);
            return source.Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToList();
        }

        public static List<T> GetPage<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, int pageSize, int pageIndex, ref int pageCount, ref int total) where T : class
        {
            total = source.Count(predicate);
            pageCount = total % pageSize == 0 ? (total / pageSize) : (total / pageSize + 1);
            return source.Where(predicate).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToList();
        }
    }
}
