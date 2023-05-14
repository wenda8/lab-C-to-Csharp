#include <stdio.h>


int main() {
    int x = 0;

    while (x < 5) {
        if (x < 3) {
            printf("x is less than 3\n");
        } else {
            printf("x is greater than or equal to 3\n");
        }

        x++;
    }

    return 0;
}


