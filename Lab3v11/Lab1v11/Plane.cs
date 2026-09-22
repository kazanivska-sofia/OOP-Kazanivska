namespace Lab1v11;
public class Plane 
{
    private string airline;
    private string model;

    public int Capacity { get; set; }

    public Plane(string airline, string model, int capacity)
    {
        this.airline = airline;
        this.model = model;
        Capacity = capacity;
    }

    ~Plane()
    {
        Console.WriteLine($"[Деструктор] Об'єкт {model} видалено.");
    }

    public void Fly()
    {
        Console.WriteLine($"Літак {airline}, {model}, (місткість: {Capacity}) вилітає у рейс!");
    }
}