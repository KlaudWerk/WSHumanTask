using System;
using System.Collections.Generic;

namespace HumanTask.ValueSet
{
    /// <summary>
    /// Simple default Value Schema factory class
    /// </summary>
    public class PropertySchemaFactory:IPropertySchemaFactory
    {
        private static readonly Dictionary<Type,Func<IValueSchema<object>>> 
            _defaultSchemas=new Dictionary<Type, Func<IValueSchema<object>>>
         {
            {typeof(string),()=>new StringSchema().Wrap()},                                                                                  
            {typeof(int),()=>new IntSchema().Wrap()},
            {typeof(int?),()=>new IntSchema().Wrap()},
            {typeof(long),()=>new LongSchema().Wrap()},
            {typeof(long?),()=>new LongSchema().Wrap()},
            {typeof(bool),()=>new BoolSchema().Wrap()},
            {typeof(bool?),()=>new BoolSchema().Wrap()},
            {typeof(DateTime),()=>new DateTimeSchema().Wrap()},
            {typeof(DateTime?),()=>new DateTimeSchema().Wrap()},
         };

        /// <summary>
        /// Creates new schema for a property of type t
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public IValueSchema<object> Create(Type t)
        {
            Func<IValueSchema<object>> schema;
            if (!_defaultSchemas.TryGetValue(t, out schema))
                return new ObjectSchema<object>().Wrap();
            return schema.Invoke();
        }

        /// <summary>
        /// Registers the schema for this factory.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="schema">The schema.</param>
        public void RegisterSchema(Type t, Func<IValueSchema<object>> schema)
        {
            _defaultSchemas[t] = schema;
        }

    }
}
