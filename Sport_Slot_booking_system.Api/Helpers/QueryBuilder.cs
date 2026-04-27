using Sport_Slot_booking_system.Api.Helpers.Common.Models;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace Sport_Slot_booking_system.Api.Helpers
{
    public static class QueryBuilder
    {
        // Properties that require exact match instead of "contains" search
        private static readonly HashSet<string> ExactMatchProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "RequestStaus",
            "RequestStatus"
            // Add other properties here that need exact matching
        };

        public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, List<SearchDomain>? searchParameters)
        {
            if (searchParameters == null || searchParameters.Count == 0)
                return query;

            var entityParameter = Expression.Parameter(typeof(T), "x");
            Expression predicate = Expression.Constant(true);

            foreach (var searchParameter in searchParameters)
            {
                if (IsValidSearchParameter(searchParameter))
                {
                    var property = Expression.Property(entityParameter, searchParameter.PropertyName);
                    var propertyType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;

                    predicate = searchParameter.Value switch
                    {
                        string strValue when !string.IsNullOrEmpty(strValue) && propertyType != typeof(DateTime) =>
                            BuildStringOrEqualityPredicate(predicate, property, propertyType, searchParameter),

                        JsonElement jsonElement when jsonElement.ValueKind == JsonValueKind.String && (propertyType == typeof(DateTime) || property.Type == typeof(DateTime?)) =>
                            BuildDateSearchPredicate(predicate, property, jsonElement.GetString()!),

                        JsonElement jsonElement when jsonElement.ValueKind == JsonValueKind.String && (propertyType == typeof(DateOnly) || property.Type == typeof(DateOnly?)) =>
                            BuildDateOnlySearchPredicate(predicate, property, propertyType, searchParameter.Value),

                        JsonElement jsonElement when jsonElement.ValueKind == JsonValueKind.String =>
                            BuildStringOrEqualityPredicateFromJson(predicate, property, propertyType, searchParameter.PropertyName, jsonElement.GetString()!),

                        JsonElement jsonElement when jsonElement.ValueKind == JsonValueKind.Number =>
                            BuildNumberEqualityPredicate(predicate, property, propertyType, jsonElement.GetInt32()),

                        _ when searchParameter.Value != null =>
                            BuildEqualityPredicate(predicate, property, propertyType, searchParameter.Value),

                        _ => predicate
                    };
                }
            }

            var lambda = Expression.Lambda<Func<T, bool>>(predicate, entityParameter);
            return query.Where(lambda);
        }

        private static Expression BuildStringOrEqualityPredicate(Expression predicate, MemberExpression property, Type propertyType, SearchDomain searchParameter)
        {
            // Check if this property requires exact matching
            if (RequiresExactMatch(searchParameter.PropertyName))
            {
                return ApplyEqualitySearch(predicate, property, propertyType, searchParameter.Value);
            }
            else
            {
                return ApplyStringSearch(predicate, property, searchParameter.Value);
            }
        }

        private static Expression BuildStringOrEqualityPredicateFromJson(Expression predicate, MemberExpression property, Type propertyType, string propertyName, string value)
        {
            // Check if this property requires exact matching
            if (RequiresExactMatch(propertyName))
            {
                return ApplyEqualitySearch(predicate, property, propertyType, value);
            }
            else
            {
                return ApplyStringSearch(predicate, property, value);
            }
        }

        private static bool RequiresExactMatch(string propertyName)
        {
            return ExactMatchProperties.Contains(propertyName);
        }

        private static Expression BuildDateSearchPredicate(Expression predicate, MemberExpression property, string searchParameterValue)
        {
            if (DateTime.TryParse(searchParameterValue, out var parsedDate))
            {
                var propertyType = property.Type;
                Expression dateProperty;

                if (Nullable.GetUnderlyingType(propertyType) != null)
                {
                    // property is DateTime?
                    var hasValueProperty = Expression.Property(property, "HasValue");
                    var valueProperty = Expression.Property(property, "Value");
                    dateProperty = Expression.Property(valueProperty, nameof(DateTime.Date));
                    var dateConstant = Expression.Constant(parsedDate.Date);
                    var dateMatches = Expression.Equal(dateProperty, dateConstant);
                    var finalBody = Expression.AndAlso(hasValueProperty, dateMatches);
                    predicate = Expression.AndAlso(predicate, finalBody);
                }
                else
                {
                    // property is DateTime
                    dateProperty = Expression.Property(property, nameof(DateTime.Date));
                    var dateConstant = Expression.Constant(parsedDate.Date);
                    var dateMatches = Expression.Equal(dateProperty, dateConstant);
                    predicate = Expression.AndAlso(predicate, dateMatches);
                }
            }

            return predicate;
        }

        private static Expression BuildDateOnlySearchPredicate(Expression predicate, MemberExpression property, Type propertyType, object searchParameterValue)
        {
            Expression? body = null;
            if (DateOnly.TryParse(searchParameterValue.ToString(), out var parsedDate))
            {
                // Check if the property is DateOnly or nullable DateOnly
                if (propertyType == typeof(DateOnly))
                {
                    var hasValueProperty = Expression.Property(property, nameof(Nullable<DateOnly>.HasValue));

                    // Access the Value of DateProperty (x.DateProperty.Value)
                    var valueProperty = Expression.Property(property, nameof(Nullable<DateOnly>.Value));
                    var dateOnlyAsDateTime = Expression.Constant(parsedDate);
                    // Create comparisons between the DateTime version of the property and the DateTime constant
                    var lessThanExpression = Expression.LessThanOrEqual(valueProperty, dateOnlyAsDateTime);
                    body = lessThanExpression;
                    var greaterThanExpression = Expression.GreaterThanOrEqual(valueProperty, dateOnlyAsDateTime);
                    body = Expression.AndAlso(body, greaterThanExpression);
                    // Now combine with HasValue check to avoid exceptions
                    var finalBody = Expression.AndAlso(hasValueProperty, body);
                    predicate = finalBody;
                }
            }
            return predicate;
        }

        private static Expression BuildStringSearchPredicate(Expression existingPredicate, MemberExpression property, string value)
        {
            return ApplyStringSearch(existingPredicate, property, value);
        }

        private static Expression BuildEqualityPredicate(Expression existingPredicate, MemberExpression property, Type propertyType, object value)
        {
            return ApplyEqualitySearch(existingPredicate, property, propertyType, value);
        }

        private static Expression BuildNumberEqualityPredicate(Expression existingPredicate, MemberExpression property, Type propertyType, int value)
        {
            return ApplyEqualitySearch(existingPredicate, property, propertyType, value);
        }

        private static Expression ApplyStringSearch(Expression predicate, MemberExpression property, object value)
        {
            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            if (containsMethod != null)
            {
                var propertyToLower = Expression.Call(property, toLowerMethod);
                var constant = Expression.Constant(value.ToString()!.ToLower());
                var comparison = Expression.Call(propertyToLower, containsMethod, constant);
                return Expression.AndAlso(predicate, comparison);
            }
            return predicate;
        }

        private static Expression ApplyEqualitySearch(Expression predicate, MemberExpression property, Type propertyType, object value)
        {
            var constant = Expression.Constant(propertyType.GetInterfaces().Contains(typeof(IConvertible))
                ? Convert.ChangeType(value, propertyType)
                : value,
                propertyType);
            Expression comparison;

            if (Nullable.GetUnderlyingType(property.Type) != null)
            {
                var hasValueProperty = Expression.Property(property, "HasValue");
                var valueProperty = Expression.Property(property, "Value");
                var valueEquals = Expression.Equal(valueProperty, constant);
                comparison = Expression.AndAlso(hasValueProperty, valueEquals);
            }
            else
            {
                comparison = Expression.Equal(property, constant);
            }

            return Expression.AndAlso(predicate, comparison);
        }

        public static IQueryable<T> OrderByPropertyWaitingForApprovalGrid<T>(this IQueryable<T> source, string? propertyName, bool sortByDesc)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return source;

            var type = typeof(T);

            var property = type.GetProperty(
                propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
                return source;

            var parameter = Expression.Parameter(type, "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);

            var delegateType = typeof(Func<,>).MakeGenericType(type, property.PropertyType);
            var lambda = Expression.Lambda(delegateType, propertyAccess, parameter);

            var methodName = sortByDesc ? "OrderByDescending" : "OrderBy";

            var method = typeof(Queryable)
                .GetMethods()
                .Single(m =>
                    m.Name == methodName &&
                    m.GetParameters().Length == 2);

            var genericMethod = method.MakeGenericMethod(type, property.PropertyType);

            return (IQueryable<T>)genericMethod.Invoke(null, new object[] { source, lambda })!;
        }

        public static IQueryable<T> OrderByProperty<T>(this IQueryable<T> source, string? propertyName, bool sortByDesc)
        {
            var type = typeof(T);
            var propertyInfo = type.GetProperty(propertyName!);
            if (propertyInfo == null)
            {
                return source;
            }

            var parameter = Expression.Parameter(type, "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
            var converted = Expression.Convert(propertyAccess, typeof(object));
            var orderByExpression = Expression.Lambda<Func<T, object>>(converted, parameter);
            if (sortByDesc)
            {
                return source.AsQueryable().OrderByDescending(orderByExpression);
            }
            return source.AsQueryable().OrderBy(orderByExpression);

        }

        private static bool IsValidSearchParameter(SearchDomain searchParameter)
        {
            return !string.IsNullOrEmpty(searchParameter.PropertyName) && searchParameter.PropertyName != "string" && searchParameter.Value != null;
        }

        public static bool HasColumn(this IDataRecord reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }

}
