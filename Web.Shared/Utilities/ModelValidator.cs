// C#

using System.Linq.Expressions;
using Microsoft.AspNetCore.Components.Forms;

namespace Web.Shared.Utilities
{
    public sealed class ModelValidator<TModel> where TModel : class
    {
        private readonly EditContext _editContext;
        
        public ModelValidator(IServiceProvider provider, TModel model)
        {
            _editContext = new EditContext(model ?? throw new ArgumentNullException(nameof(model)));
            _editContext.EnableDataAnnotationsValidation(provider);
        }

        public TModel Model => (TModel)_editContext.Model;
        public EditContext EditContext => _editContext;

        // Validates the entire model using DataAnnotations
        public bool ValidateAll(out IReadOnlyDictionary<string, string[]> errors)
        {
            var isValid = _editContext.Validate();
            errors = CollectAllMessages();
            return isValid;
        }

        public bool ValidateAll() => _editContext.Validate();

        // Validates only the provided fields (supports nested properties)
        public bool ValidateFields(params Expression<Func<TModel, object?>>[] fields) =>
            ValidateFields(out _, fields);

        public bool ValidateFields(out IReadOnlyDictionary<string, string[]> errors,
                                   params Expression<Func<TModel, object?>>[] fields)
        {
            if (fields == null || fields.Length == 0)
            {
                // If no fields provided, default to full validation
                return ValidateAll(out errors);
            }

            // Fire field-level validation for each requested field
            foreach (var accessor in fields)
            {
                var fieldId = new FieldIdentifier(_editContext.Model, GetPath(accessor));
                _editContext.NotifyFieldChanged(fieldId);
            }

            // Collect messages only for the requested fields
            var dict = new Dictionary<string, string[]>(StringComparer.Ordinal);
            foreach (var accessor in fields)
            {
                var path = GetPath(accessor);
                var fieldId = new FieldIdentifier(_editContext.Model, path);
                var messages = _editContext.GetValidationMessages(fieldId).ToArray();
                if (messages.Length > 0)
                    dict[path] = messages;
            }

            errors = dict;
            return errors.Count == 0;
        }

        private IReadOnlyDictionary<string, string[]> CollectAllMessages()
        {
            // Group messages by a stable "path" key similar to x => x.Prop or x => x.Nested.Prop
            var allMessages = new Dictionary<string, List<string>>(StringComparer.Ordinal);

            // Note: EditContext doesn’t expose all FieldIdentifiers; we rely on the
            // message enumeration and build a best-effort map keyed by field name.
            // For programmatic uses (like section checks), this is typically sufficient.
            foreach (var message in _editContext.GetValidationMessages())
            {
                // Without a direct FieldIdentifier here, we just bucket by message text occurrence.
                // If you need strict per-field aggregation, prefer ValidateFields(...) for targeted checks.
                const string catchAllKey = "__all__";
                if (!allMessages.TryGetValue(catchAllKey, out var list))
                {
                    list = new List<string>();
                    allMessages[catchAllKey] = list;
                }
                list.Add(message);
            }

            return allMessages.ToDictionary(k => k.Key, v => v.Value.ToArray(), StringComparer.Ordinal);
        }

        private static string GetPath(Expression<Func<TModel, object?>> expression)
        {
            // Build a "dotted" path from the expression, e.g., x => x.Contact.Name => "Contact.Name"
            Expression body = expression.Body;
            if (body is UnaryExpression u && u.NodeType == ExpressionType.Convert)
                body = u.Operand;

            var parts = new Stack<string>();
            var current = body as MemberExpression;
            while (current != null)
            {
                parts.Push(current.Member.Name);
                current = current.Expression as MemberExpression;
            }

            return string.Join(".", parts);
        }
    }
}