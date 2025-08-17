using System;
using System.Threading.Tasks;
using WebApiPerson.Models;

namespace WebApiPerson.Factories
{
    public static class TaskItemFactory
    {
        // Método que retorna un objeto TaskItem envuelto en un Task
        public static TaskItem CreateHighPriorityTask(string description)
        {
            return new TaskItem
            {
                Description = description,
                DueDate = DateTime.Now.AddDays(1),
                Status = "Pending",
                AdditionalData = "High Priority"
            };

            //return Task.FromResult(taskItem); // Devuelve un Task completado
        }
        public static TaskItem CreateNormalPriorityTask(string description)
        {
            return new TaskItem
            {
                Description = description,
                DueDate = DateTime.Now.AddDays(3),
                Status = "Pending",
                AdditionalData = "Normal Priority"
            };
        }

        public static TaskItem CreateLowPriorityTask(string description)
        {
            return new TaskItem
            {
                Description = description,
                DueDate = DateTime.Now.AddDays(5),
                Status = "Pending",
                AdditionalData = "Low Priority"
            };
        }

        //internal static TaskItem CreateHighPriorityTask(string description)
        //{
        //    throw new NotImplementedException();
        //}
    }
}