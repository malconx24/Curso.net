using WebApiPerson.Models;
namespace WebApiPerson.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string? Status { get; set; }
        public string? AdditionalData { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Propiedad derivada; no se asigna si se usa EF. Se calcula mediante Func en la capa de servicio.
        /// </summary>
        public int DaysRemaining { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.High;


    }
}
