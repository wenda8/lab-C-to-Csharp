using System; 
 class Program { 

static double calculate(double x){
    if (x <= 0)
    {
        return 1;
    }
    else
    {
        return x * calculate(x - 1) + calculate(x - 2);
    }
}

static void Main(){
    double input;

    Console.WriteLine("Enter a number: ");input = double.Parse(Console.ReadLine());double result = calculate(input);

    Console.WriteLine("Result: {0}\n", result);}
}
