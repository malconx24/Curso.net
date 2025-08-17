using WebApiPerson.Delegates;
using WebApiPerson.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApiPerson.Context;   // <- tu DbContext
using Microsoft.EntityFrameworkCore;

namespace WebApiPerson.Services
{
    public class TaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        // Hook points
        public TaskValidator? Validate { get; set; } = Validators.DefaultTaskValidator;
        public Action<TaskItem>? OnCreated { get; set; }
        public Action<TaskItem>? OnDeleted { get; set; }
        public Func<TaskItem, int>? ComputeDaysRemaining { get; set; } =
            (t) => (int)Math.Ceiling((t.DueDate - DateTime.UtcNow).TotalDays);

        public IEnumerable<TaskItem> Get(Func<TaskItem, bool>? filter = null)
        {
            var query = _context.Tasks.AsQueryable();

            if (filter != null)
                query = query.AsEnumerable().Where(filter).AsQueryable();

            var items = query.ToList();

            // Update derived field using Func
            foreach (var t in items)
                t.DaysRemaining = ComputeDaysRemaining?.Invoke(t) ?? 0;

            return items;
        }

        public (bool Ok, string Error, TaskItem? Item) Create(TaskItem item)
        {
            var result = Validate?.Invoke(item) ?? (true, string.Empty);
            if (!result.IsValid)
                return (false, result.Error, null);

            item.CreatedAt = DateTime.UtcNow;
            item.IsCompleted = false;
            item.DaysRemaining = ComputeDaysRemaining?.Invoke(item) ?? 0;

            _context.Tasks.Add(item);
            _context.SaveChanges();

            OnCreated?.Invoke(item);
            return (true, string.Empty, item);
        }

        public bool Delete(int id)
        {
            var item = _context.Tasks.FirstOrDefault(t => t.Id == id);
            if (item == null) return false;

            _context.Tasks.Remove(item);
            _context.SaveChanges();

            OnDeleted?.Invoke(item);
            return true;
        }

        public bool Complete(int id)
        {
            var item = _context.Tasks.FirstOrDefault(t => t.Id == id);
            if (item == null) return false;

            item.IsCompleted = true;
            _context.SaveChanges();
            return true;
        }
    }
}
