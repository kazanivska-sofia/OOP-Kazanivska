namespace Lab1v11;

class Program
{
    static void Main(string[] args)
    {
        Plane plane1 = new Plane("МАУ", "Boeing 737", 186);
        Plane plane2 = new Plane("Ryanair", "Boeing 737-800", 189);
        Plane plane3 = new Plane("Wizz Air", "Airbus A321", 239);
        
            plane1.Fly();
            plane2.Fly();
            plane3.Fly();
        
    }
}