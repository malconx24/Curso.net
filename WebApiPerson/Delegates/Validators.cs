using WebApiPerson.Models;

namespace WebApiPerson.Delegates
{ /// <summary>
  /// Custom delegate to validate a TaskItem.
  /// Returns (isValid, errorMessage)
  /// </summary>
    public delegate (bool IsValid, string Error) TaskValidator(TaskItem task);

    public static class Validators
    {
        /// <summary>
        /// Default validation: description not empty and due date in the future.
        /// Demonstrates anonymous functions possibility: can be replaced at runtime.
        /// </summary>
        public static TaskValidator DefaultTaskValidator => (TaskItem t) =>
        {
            if (string.IsNullOrWhiteSpace(t.Description))
                return (false, "La descripción no puede estar vacía.");
            if (t.DueDate == default)
                return (false, "La fecha de vencimiento es requerida.");
            if (t.DueDate <= DateTime.UtcNow)
                return (false, "La fecha de vencimiento debe ser futura.");
            return (true, string.Empty);
        };
    }
}