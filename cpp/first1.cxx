# 1 "first.cxx"
# 1 "<built-in>"
# 1 "<command-line>"
# 1 "/usr/include/stdc-predef.h" 1 3 4
# 1 "<command-line>" 2
# 1 "first.cxx"



void add();

int main()
{

 using namespace std;

 time_t tp;
 tp=time(NULL);
 struct tm* ptr=localtime(&tp);

 cout<<tp<<endl;
 cout<<"great!";
 add();
 return 0;
}

void add()
{
 printf("ha");
}
