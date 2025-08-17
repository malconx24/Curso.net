using System.Reactive.Linq;
using System.Reactive.Subjects;
using WebApiPerson.Models;


namespace WebApiPerson.Factories
{
    public class TaskQueue
    {
    private static readonly Queue<TaskItem> queue = new Queue<TaskItem>();
    private static bool isProcessing = false;

    // Subject para emitir eventos cuando se agrega una tarea
    private static readonly Subject<TaskItem> taskSubject = new Subject<TaskItem>();

    // Observable público para suscribirse a nuevas tareas
    public static IObservable<TaskItem> TaskStream => taskSubject.AsObservable();

    // Agregar tarea a la cola
    public static void Enqueue(TaskItem task)
    {
        queue.Enqueue(task);          // Se agrega al final de la cola
        taskSubject.OnNext(task);     // Notifica a los observadores
        ProcessQueue();               // Inicia el procesamiento si no hay tarea activa
    }

    // Procesar la cola secuencialmente
    private static async void ProcessQueue()
    {
        if (isProcessing || queue.Count == 0) return;

        isProcessing = true;

        while (queue.Count > 0)
        {
            var currentTask = queue.Dequeue();
            Console.WriteLine($"Procesando tarea: {currentTask.Description}");

            // Simulación de procesamiento (2 segundos)
            await Task.Delay(2000);

            Console.WriteLine($"Tarea completada: {currentTask.Description}");
        }

        isProcessing = false;
    }
}
}