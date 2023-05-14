#include <stdio.h>

double calculate(double x)
{
    if (x <= 0)
    {
        return 1;
    }
    else
    {
        return x * calculate(x - 1) + calculate(x - 2);
    }
}

int main()
{
    double input;

    printf("Enter a number: ");
    scanf("%lf", &input);

    double result = calculate(input);

    printf("Result: %.2lf\n", result);

    return 0;
}
