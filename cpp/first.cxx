#include<ctime>
#include<iostream>
#define kkk
void add();
#ifdef kkk
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
#endif
void add()
{
	printf("ha");
}
	