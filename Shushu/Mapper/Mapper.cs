using Shushu.Tokens;
using System;
using System.Reflection;
using Sushi2;
using Microsoft.Spatial;
using Microsoft.Azure.Search.Models;
using System.Linq;

namespace Shushu
{
    /// <summary>
    /// Mapper.
    /// </summary>
    public static class Mapper
    {
        /// <summary>
        /// Maps the object to Shushu index.
        /// </summary>
        /// <returns>The Shushu index.</returns>
        /// <param name="obj">The object.</param>
        /// <typeparam name="T">The type of object.</typeparam>
        public static ShushuIndex MapToIndex<T>(this T obj) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            // no mappings needed for Shushu objects
            if (typeof(T) == typeof(ShushuIndex))
                return obj as ShushuIndex;

            // get mappings
            var classMappings = MapperHelpers.GetClassMappings<T>();
            var propertyMappings = MapperHelpers.GetPropertyMappings<T>();

            // check if id mapping exists
            if (classMappings.All(x => x.IndexField != Enums.IndexField.Id) &&
               propertyMappings.All(x => x.IndexField != Enums.IndexField.Id))
            {
                throw new Exception($"You have to define a mapping for index field Id in {typeof(T)}.");
            }

            // create Shushu index instance
            var shushu = new ShushuIndex();

            // set class mappings
            foreach (var cm in classMappings)
            {
                shushu.GetType().GetTypeInfo().GetProperty(cm.IndexField.ToString())?.SetValue(shushu, cm.Value);
            }

            // set property mappings
            foreach (var pm in propertyMappings)
            {
                var @value = obj.GetPropertyValue(pm.Property);

                if (@value != null)
                {
                    if (pm.IndexField == Enums.IndexField.Point0 &&
                        @value is GeoPoint gp)
                    {
                        @value = GeographyPoint.Create(gp.Coordinates[0], gp.Coordinates[1]);
                        shushu.GetType().GetTypeInfo().GetProperty(pm.IndexField.ToString())?.SetValue(shushu, @value);
                    }
                    else if (pm.IndexField == Enums.IndexField.Date0 ||
                        pm.IndexField == Enums.IndexField.Date1 ||
                        pm.IndexField == Enums.IndexField.Date2 ||
                        pm.IndexField == Enums.IndexField.Date3 ||
                        pm.IndexField == Enums.IndexField.Date4)
                    {
                        var dt = value.ToDateTime();

                        if (dt.HasValue)
                        {
                            var dto = new DateTimeOffset(dt.Value);
                            shushu.GetType().GetTypeInfo().GetProperty(pm.IndexField.ToString())?.SetValue(shushu, dto);
                        }
                    }
                    else if (pm.IndexField == Enums.IndexField.Number0 ||
                             pm.IndexField == Enums.IndexField.Number1 ||
                             pm.IndexField == Enums.IndexField.Number2 ||
                             pm.IndexField == Enums.IndexField.Number3 ||
                             pm.IndexField == Enums.IndexField.Number4 ||
                             pm.IndexField == Enums.IndexField.Number5 ||
                             pm.IndexField == Enums.IndexField.Number6 ||
                             pm.IndexField == Enums.IndexField.Number7 ||
                             pm.IndexField == Enums.IndexField.Number8 ||
                             pm.IndexField == Enums.IndexField.Number9)
                    {
                        var n = @value.ToInt64();

                        if (n.HasValue)
                        {
                            shushu.GetType().GetTypeInfo().GetProperty(pm.IndexField.ToString())?.SetValue(shushu, n);
                        }
                        else if (@value.GetType().IsEnum &&
                            value != null)
                        {
                            var ty = @value.GetType();
                            var e = (Int32)Enum.Parse(ty, @value.ToString());
                            var e2 = e.ToInt64();

                            shushu.GetType().GetTypeInfo().GetProperty(pm.IndexField.ToString())?.SetValue(shushu, e2);
                        }
                    }
                    else
                    {
                        shushu.GetType().GetTypeInfo().GetProperty(pm.IndexField.ToString())?.SetValue(shushu, @value);
                    }
                }
            }

            return shushu;
        }

        /// <summary>
        /// Maps the Shushu index to object.
        /// </summary>
        /// <returns>The object index.</returns>
        /// <param name="index">The Shushu index.</param>
        /// <typeparam name="T">The type of object.</typeparam>
        public static T MapFromIndex<T>(this ShushuIndex index) where T : class, new()
        {
            var obj = new T();

            if (index == null)
                throw new ArgumentNullException(nameof(index));

            // no mappings needed for Shushu objects
            if (typeof(T) == typeof(ShushuIndex))
                return obj;

            // get mappings
            var classMappings = MapperHelpers.GetClassMappings<T>();
            var propertyMappings = MapperHelpers.GetPropertyMappings<T>();

            // check if id mapping exists
            if (classMappings.All(x => x.IndexField != Enums.IndexField.Id) &&
               propertyMappings.All(x => x.IndexField != Enums.IndexField.Id))
            {
                throw new Exception($"You have to define a mapping for index field Id in {typeof(T)}.");
            }

            // set class mappings
            foreach (var cm in classMappings)
            {
                var prop = typeof(T).GetTypeInfo().GetProperty(cm.IndexField.ToString());

                if (prop != null)
                    prop.SetValue(obj, cm.Value);
            }

            // set property mappings
            foreach (var pm in propertyMappings)
            {
                var @value = index.GetPropertyValue(pm.IndexField.ToString());

                if (@value != null)
                {
                    if (pm.IndexField == Enums.IndexField.Point0 &&
                        @value is GeoPoint gp)
                    {
                        @value = GeographyPoint.Create(gp.Coordinates[0], gp.Coordinates[1]);
                        obj.GetType().GetTypeInfo().GetProperty(pm.Property)?.SetValue(obj, @value);
                    }
                    else if (pm.IndexField == Enums.IndexField.Date0 ||
                        pm.IndexField == Enums.IndexField.Date1 ||
                        pm.IndexField == Enums.IndexField.Date2 ||
                        pm.IndexField == Enums.IndexField.Date3 ||
                        pm.IndexField == Enums.IndexField.Date4)
                    {
                        var dt = value.ToDateTime();

                        if (dt.HasValue)
                        {
                            var dto = new DateTime?(dt.Value);
                            obj.GetType().GetTypeInfo().GetProperty(pm.Property)?.SetValue(obj, dto);
                        }
                    }
                    else if (pm.IndexField == Enums.IndexField.Number0 ||
                             pm.IndexField == Enums.IndexField.Number1 ||
                             pm.IndexField == Enums.IndexField.Number2 ||
                             pm.IndexField == Enums.IndexField.Number3 ||
                             pm.IndexField == Enums.IndexField.Number4 ||
                             pm.IndexField == Enums.IndexField.Number5 ||
                             pm.IndexField == Enums.IndexField.Number6 ||
                             pm.IndexField == Enums.IndexField.Number7 ||
                             pm.IndexField == Enums.IndexField.Number8 ||
                             pm.IndexField == Enums.IndexField.Number9)
                    {
                        var pi = obj.GetType().GetTypeInfo().GetProperty(pm.Property);

                        if (pi != null)
                        {
                            Type t = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;

                            if (t.IsEnum)
                            {
                                var e = Enum.Parse(t, @value.ToString());
                                pi.SetValue(obj, e);
                            }
                            else
                            {
                                object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                                pi.SetValue(obj, safeValue);
                            }
                        }
                    }
                    else if (pm.IndexField == Enums.IndexField.Point0)
                    {
                        var p = value as dynamic;
                        if (p != null)
                        {
                            var np = new GeoPoint(p.Latitude, p.Longitude);
                            obj.GetType().GetTypeInfo().GetProperty(pm.Property)?.SetValue(obj, np);
                        }
                    }
                    else
                    {
                        var prop = obj.GetType().GetTypeInfo().GetProperty(pm.Property);

                        if (prop != null &&
                            prop.GetSetMethod() != null)
                        {
                            obj.GetType().GetTypeInfo().GetProperty(pm.Property)?.SetValue(obj, @value);
                        }
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// Maps search parameters.
        /// </summary>
        /// <returns>New search parameters.</returns>
        /// <param name="searchParameters">Original search parameters.</param>
        /// <typeparam name="T">Type of class used for mapping.</typeparam>
        public static SearchParameters MapSearchParameters<T>(this SearchParameters searchParameters) where T : class
        {
            var propertyMappings = MapperHelpers.GetPropertyMappings<T>();
            var classMappings = MapperHelpers.GetClassMappings<T>();

            searchParameters.Select = searchParameters.Select.ToParameters().ToQuery(searchParameters.Select, propertyMappings);
            searchParameters.SearchFields = searchParameters.SearchFields.ToParameters().ToQuery(searchParameters.SearchFields, propertyMappings);
            searchParameters.HighlightFields = searchParameters.HighlightFields.ToParameters().ToQuery(searchParameters.HighlightFields, propertyMappings);
            searchParameters.Facets = searchParameters.Facets.ToParameters().ToQuery(searchParameters.Facets, propertyMappings);
            searchParameters.Filter = searchParameters.Filter.ToParameters().ToQuery(searchParameters.Filter, propertyMappings);
            searchParameters.OrderBy = searchParameters.OrderBy.ToParameters().ToQuery(searchParameters.OrderBy, propertyMappings);
            searchParameters.ScoringParameters = searchParameters.ScoringParameters.ToParameters().ToQuery(searchParameters.ScoringParameters, propertyMappings);

            return searchParameters;
        }
    }
}