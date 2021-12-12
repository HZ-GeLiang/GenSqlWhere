using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToSqlWhereClause.SqlFun
{
 
 
    [AttributeUsage(AttributeTargets.Method)]
 
    public class DbFunctionAttribute : Attribute
    {
        private string? _name;
        private string? _schema;
        private bool _builtIn;
        private bool? _nullable;

 
        public DbFunctionAttribute()
        {
        }

   
        public DbFunctionAttribute(string name, string? schema = null)
        {
            _name = name;
            _schema = schema;
        }

        /// <summary>
        ///     The name of the function in the database.
        /// </summary>
  
        public virtual string? Name
        {
            get => _name;
            set
            {
                _name = value;
            }
        }

        /// <summary>
        ///     The schema of the function in the database.
        /// </summary>
        public virtual string? Schema
        {
            get => _schema;
            set => _schema = value;
        }

        /// <summary>
        ///     The value indicating whether the database function is built-in or not.
        /// </summary>
        public virtual bool IsBuiltIn
        {
            get => _builtIn;
            set => _builtIn = value;
        }

        /// <summary>
        ///     The value indicating whether the database function can return null result or not.
        /// </summary>
        public virtual bool IsNullable
        {
            get => _nullable ?? true;
            set => _nullable = value;
        }

        /// <summary>
        ///     Checks whether <see cref="IsNullable" /> has been explicitly set to a value.
        /// </summary>
        public bool IsNullableHasValue
            => _nullable.HasValue;
    }

}
