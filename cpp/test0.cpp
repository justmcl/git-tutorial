#include<iostream>
#include<string>
#include"add.h"
typedef struct
{
    std::string name;
    int age;

    /* data */
}son;

int main(){
    printf("great!");
    son son1;
    son1.age=18;
    son1.name="kitty";
    std::cout<<(son1.name);
    std::cout<<add(1,3);
    return 0;
}
